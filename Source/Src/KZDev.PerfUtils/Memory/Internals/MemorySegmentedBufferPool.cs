// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// An implementation of a buffer pool that is used for the fixed,
/// standard sized buffers in the <see cref="SegmentMemoryStreamSlim"/> class.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
internal class MemorySegmentedBufferPool : IDisposable
{
    /// <summary>
    /// The timer frequency to trim the buffer groups.
    /// </summary>
    private const int TrimTimerIntervalMs =
#if DEBUG
            20_000;
#else
        600_000;
#endif

    /// <summary>
    /// A flag indicating if the zero processing operation is currently active.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be done using the Volatile class, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private /* volatile */ int _zeroProcessingActive;

    /// <summary>
    /// Debug helper to display the state of the pool.
    /// </summary>
    [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
    private string DebugDisplayValue => $"Native Memory = {_useNativeMemory}, Zero Pending = {_pendingZeroList.Count}, Group = {_arrayGroup.DebugDisplayValue}";
#pragma warning restore HAA0601

    /// <summary>
    /// A flag indicating if we should use native memory for the buffers.
    /// </summary>
    private readonly bool _useNativeMemory;

    /// <summary>
    /// A list of returned buffers that are awaiting zeroing.
    /// </summary>
    private readonly BlockingCollection<SegmentBuffer> _pendingZeroList = new(100);

    /// <summary>
    /// A timer that is used to trim the buffer pool groups in the generation array.
    /// </summary>
    private readonly Timer _trimTimer;

    /// <summary>
    /// When this value is non-zero, it indicates that the callback for the 
    /// trimming timer is active.
    /// </summary>
    private int _trimmingActive;

    /// <summary>
    /// The current array of buffer groups that are available.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be done using the Volatile class, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private /* volatile */ MemorySegmentedGroupGenerationArray _arrayGroup;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Delegate run in the thread pool to zero out the buffers in the pending list.
    /// </summary>
    /// <param name="poolInstance">
    /// The instance of the pool that is running the zeroing operation.
    /// </param>
    private static void DoZeroProcessing (MemorySegmentedBufferPool poolInstance)
    {
        try
        {
            // Get the local reference to the list.
            BlockingCollection<SegmentBuffer> pendingZeroList = poolInstance._pendingZeroList;
            try
            {
                // Since this is a blocking collection, there will be a limit to the number of items
                // in the list, so we can just use a local variable to track the count. We utilize
                // this limit to not only keep the producer from getting too far ahead of this consumer,
                // but also to limit the number of zeroing operations we do in a single thread-pool delegate execution.
                // Therefore, we grab the current count once and use it to limit the number of zeroing
                // operations we do here, but when we're done, check if there is more work to do
                // and trigger another zeroing operation if needed.
                int runLoops = pendingZeroList.Count;
                // Volatile read of the array group generation to get our local reference.
                MemorySegmentedGroupGenerationArray generationArray = Volatile.Read(ref poolInstance._arrayGroup);

                // Now, loop through the pending list and zero out the buffers.
                for (int loopIndex = 0; loopIndex < runLoops; loopIndex++)
                {
                    // Since we got the count before we started processing, this call should never return
                    // false, but we need to be safe.
                    if (!pendingZeroList.TryTake(out SegmentBuffer releaseBuffer))
                        break;
                    releaseBuffer.Clear();
                    // Store the buffer back in the group.
                    if (generationArray.ReleaseBuffer(releaseBuffer, true))
                    {
                        continue;
                    }

                    // If we get here, this means that the buffer actually came from a generation that is newer
                    // than the one we grabbed a reference to. So, the buffer group was not found.
                    // We could try to get the most recent generation and just loop again,
                    // but we might just as well break out and let the next round just pick
                    // up from the beginning, since this could possibly indicate things are pretty busy.
                    // We are going to take a bit of a performance hit here because we will zero the buffer more
                    // than once, but we need to make sure this segment gets properly released, and we need to
                    // put it back into the pending zero list to be zeroed out again on the next pass.
                    // This is the safest this to do, and this condition will be extremely rare.
                    pendingZeroList.Add(releaseBuffer);
                    break;
                }
            }
            finally
            {
                // Indicate that we are done processing.
                Volatile.Write(ref poolInstance._zeroProcessingActive, 0);
                // If there is new work to do, trigger another zeroing operation.
                if (pendingZeroList.Count > 0)
                    poolInstance.TriggerZeroProcessing();
            }
        }
        catch
        {
            // Ignored - there really isn't anything we can do here, but we want to 
            // keep the exception from propagating.
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Timer callback to trim the buffer groups from the specified generation array.
    /// </summary>
    /// <param name="state">
    /// The timer state object - passes the buffer pool instance
    /// </param>
    private static void TrimBufferGroups (object? state)
    {
        // Get the pool being trimmed.
        MemorySegmentedBufferPool trimPool = (MemorySegmentedBufferPool)state!;

        // Avoid running this if it is already running.
        if (Interlocked.CompareExchange(ref trimPool._trimmingActive, 1, 0) != 0)
            return;
        try
        {
            // Get the generation array to trim.
            MemorySegmentedGroupGenerationArray generationArray = trimPool._arrayGroup;
            if (!generationArray.TrimGroups())
                return;
            // If we trimmed groups, we should contract the array group.
            trimPool.ContractArrayGroup(generationArray);
        }
        finally
        {
            // Indicate that we are done processing.
            trimPool._trimmingActive = 0;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// When there is at least one buffer group that has been released, then we should
    /// actually contract the array group to remove the empty groups. This helper method
    /// creates a new generation with the released groups removed and then tries to set
    /// that as the current generation.
    /// </summary>
    /// <param name="currentGeneration">
    /// The generation instance that was processed to trim groups from, and should have
    /// those groups removed by creating a contracted copy.
    /// </param>
    private void ContractArrayGroup (MemorySegmentedGroupGenerationArray currentGeneration)
    {
        // We will try to contract the array group if the current generation is the 
        // generation passed in. If it is not, then we will just return.
        MemorySegmentedGroupGenerationArray activeGeneration = _arrayGroup;
        if (activeGeneration.GenerationId != currentGeneration.GenerationId)
            return;
        // We don't want to affect the current generation array directly, 
        // so we will create a new one with the contract helper method.
        MemorySegmentedGroupGenerationArray newGeneration = currentGeneration.ContractArray();
        // Now, try to set the new generation as the current generation, and just to minimize the
        // chance of missing a new generation, we will confirm that the current generation is still
        // what we expect it to be, and otherwise assume that the current generation has been updated,
        // and we should return that instead.
        Interlocked.CompareExchange(ref _arrayGroup, newGeneration, activeGeneration);
        // We either assigned our newly created generation as the current generation, or we let it
        // get GC collected
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Expands the array group by creating a new array group generation and assigning it
    /// as the current group.
    /// </summary>
    /// <param name="currentGeneration">
    /// The current generation of the array group that we want to expand from.
    /// </param>
    /// <param name="neededBufferSize">
    /// The size that is needed for the buffer that we are trying to rent. We use this
    /// to minimize the number of array generations that we create if we know that the
    /// size of the generation array we would otherwise be creating is going to be too
    /// small for our immediate needs.
    /// </param>
    private MemorySegmentedGroupGenerationArray ExpandArrayGroup (MemorySegmentedGroupGenerationArray currentGeneration,
        long neededBufferSize)
    {
        /*
         * Imagine the race condition here and why it doesn't matter.
         * 1) Thread A is processing a request to rent a buffer and locally grabs a reference
         *    to the current generation (_arrayGroup) and uses it to enumerate the buffer groups
         *    looking for a buffer to use.
         * 2) Thread B is processing a return operation and also grabs a local reference to the
         *    current generation (_arrayGroup) and uses it to enumerate the buffer groups
         *    looking for group to store the buffer in.
         * 3) While Thread A searches for a buffer, it will either find one and return it, updating
         *    the 'current' index value from a specific group (in group locked isolation), or it will
         *    not find one and will create a new buffer to return, leaving the current generation
         *    not impacted.
         * 4) While Thread B searches for a group to store the buffer in, it will either find one and
         *    store the buffer, updating the 'current' index value from a specific group (in group locked
         *    isolation), or it will not find one and will create a new group to store the buffer in,
         *    thereby creating a new generation of the array group, and all the existing BufferGroup
         *    just get copied over to the new generation.
         *  Therefore, there is no real race condition.
         *
         * In the case of two threads processing a Return operation at the same time, they may both
         * find the need to create a new generation, but that is not a problem we really need to worry
         * about. The only thing that would happen is that we would have two (or more, if there are more
         * than two threads doing this processing at the same time) new generations created, and
         * the last one in will win. The net effect of this is that we may have created a
         * GroupGenerationArray that gets 'lost' (containing only one buffer) and is eventually GC collected.
         * So, we will have lost out on some processing to create that new generation, and we will lose
         * the buffer that was stored in it, instead of caching it for reuse, but we can live with that
         * while gaining the benefit of not having to lock the entire array group while processing for
         * the more likely use case of this race condition not happening.
         *
         */
        MemorySegmentedGroupGenerationArray newGeneration =
            new MemorySegmentedGroupGenerationArray(currentGeneration, neededBufferSize, _useNativeMemory);
        // Now, try to set the new generation as the current generation, and just to minimize the
        // chance of missing a new generation, we will confirm that the current generation is still
        // what we expect it to be, and otherwise assume that the current generation has been updated,
        // and we should return that instead.
        MemorySegmentedGroupGenerationArray activeGeneration = Interlocked.CompareExchange(ref _arrayGroup, newGeneration, currentGeneration);
        // We either assigned our newly created generation as the current generation, or we let it
        // get GC collected, and we will return the active generation.
        return (activeGeneration.GenerationId == currentGeneration.GenerationId) ? newGeneration : activeGeneration;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Checks if we need to start a zeroing operation on the pending list and then triggers
    /// it to run.
    /// </summary>
    private void TriggerZeroProcessing ()
    {
        if (Interlocked.CompareExchange(ref _zeroProcessingActive, 1, 0) != 0) return;
        try
        {
#if CONCURRENCY_TESTING   // Concurrency testing does not currently support async/await and/or ThreadPool.QueueUserWorkItem
                Task.Run(() => DoZeroProcessing(this));
#else
            ThreadPool.QueueUserWorkItem(static @this => DoZeroProcessing((MemorySegmentedBufferPool)@this!), this);
#endif
        }
        catch
        {
            _zeroProcessingActive = 0;
            throw;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to do the actual rent operation.
    /// </summary>
    /// <param name="requestedBufferSize">
    /// The desired size of the buffer to rent.
    /// </param>
    /// <param name="clearNewAllocations">
    /// Indicates whether we need new allocations to be cleared.
    /// </param>
    /// <param name="generationArray">
    /// The generation array that we are renting from.
    /// </param>
    /// <param name="preferredBlockInfo">
    /// Information about the last block that was used to allocate a buffer. This is used
    /// to try and allocate from the same block and next segment if possible
    /// </param>
    /// <returns>
    /// A buffer that is the size of the buffer size for this pool and a flag indicating 
    /// if the returned buffer is the next segment from the preferred block.
    /// </returns>
    private (SegmentBuffer Buffer, bool IsNextBlockSegment)
        RentFromGeneration (long requestedBufferSize, MemorySegmentedGroupGenerationArray generationArray,
        bool clearNewAllocations, in SegmentBufferInfo preferredBlockInfo)
    {
        // Try to rent from the preferred group first.
        MemorySegmentedBufferGroup[] segmentGroupArray = generationArray.Groups;
        MemorySegmentedBufferGroup? preferredGroup = null;
        for (int groupIndex = 0; groupIndex < segmentGroupArray.Length; groupIndex++)
        {
            MemorySegmentedBufferGroup group = segmentGroupArray[groupIndex];
            if (group.Id != preferredBlockInfo.BlockId)
                continue;
            preferredGroup = group;
            break;
        }
        // If we can't find that group for some reason, then just rent without the preferred block info.
        if (preferredGroup is null)
            return (RentFromGeneration(requestedBufferSize, generationArray, clearNewAllocations), false);

        // Try to get the buffer from the preferred group.
        (SegmentBuffer buffer, GetBufferResult getResult, bool segmentIsPreferredInBlock) =
            preferredGroup.GetBuffer(requestedBufferSize, clearNewAllocations, this, preferredBlockInfo.SegmentId + preferredBlockInfo.SegmentCount);
        return (getResult == GetBufferResult.Available) ? (buffer, segmentIsPreferredInBlock) :
            // The group is either locked, released, full, or the preferred segment was not available,
            // so we will fall back to the normal rent operation.
            (RentFromGeneration(requestedBufferSize, generationArray, clearNewAllocations), false);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to do the actual rent operation.
    /// </summary>
    /// <param name="requestedBufferSize">
    /// The desired size of the buffer to rent.
    /// </param>
    /// <param name="clearNewAllocations">
    /// Indicates whether we need new allocations to be cleared.
    /// </param>
    /// <param name="generationArray">
    /// The generation array that we are renting from.
    /// </param>
    /// <returns>
    /// A buffer that is the size of the buffer size for this pool.
    /// </returns>
    private SegmentBuffer RentFromGeneration (long requestedBufferSize,
        MemorySegmentedGroupGenerationArray generationArray, bool clearNewAllocations)
    {
        int lockedLoops = 0;

        int groupCount = generationArray.Groups.Length;
        while (true)
        {
            bool anyGroupsLocked = false;
            // The larger blocks are later in the array, so we will start at the end.
            for (int groupIndex = groupCount - 1; groupIndex >= 0; groupIndex--)
            {
                (SegmentBuffer buffer, GetBufferResult getResult) =
                    generationArray.Groups[groupIndex].GetBuffer(requestedBufferSize, clearNewAllocations, this);
                switch (getResult)
                {
                    case GetBufferResult.Available:
                        return buffer;

                    case GetBufferResult.GroupLocked:
                        anyGroupsLocked = true;
                        break;
                }
                // Try the next group.
            }

            // If any groups are locked, we will try a few times before expanding the array group.
            if (anyGroupsLocked)
            {
                if (++lockedLoops < 4)
                    continue;
            }

            // Reset the locked loop count because we are going to expand the array group.
            lockedLoops = 0;

            // We will try to expand the array group if we didn't find a buffer.
            // Quick check to see if the reference is still the same.
            MemorySegmentedGroupGenerationArray currentGenerationArray = Volatile.Read(ref _arrayGroup);
            if (currentGenerationArray.GenerationId == generationArray.GenerationId)
            {
                // We don't want to affect the current generation array directly, 
                // so we will create a new one.
                generationArray = ExpandArrayGroup(generationArray, requestedBufferSize);
                groupCount = generationArray.Groups.Length;
                continue;
            }
            // If the reference has changed, then we need to get the new reference and try again.
            // Someone else already expanded the array group, so we will just try again with that
            // generation.
            generationArray = currentGenerationArray;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to do the actual rent operation.
    /// </summary>
    /// <param name="requestedBufferSize">
    /// The desired size of the buffer to rent.
    /// </param>
    /// <param name="clearNewAllocations">
    /// Indicates whether we need new allocations to be cleared.
    /// </param>
    /// <returns>
    /// A buffer that is the size of the buffer size for this pool.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SegmentBuffer RentInternal (long requestedBufferSize, bool clearNewAllocations) =>
        RentFromGeneration(requestedBufferSize, Volatile.Read(ref _arrayGroup), clearNewAllocations);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to do the actual rent operation.
    /// </summary>
    /// <param name="requestedBufferSize">
    /// The desired size of the buffer to rent.
    /// </param>
    /// <param name="clearNewAllocations">
    /// Indicates whether we need new allocations to be cleared.
    /// </param>
    /// <param name="preferredBlockInfo">
    /// Information about the last block that was used to allocate a buffer. This is used
    /// to try and allocate from the same block and next segment if possible
    /// </param>
    /// <returns>
    /// A buffer that is the size of the buffer size for this pool and a flag indicating 
    /// if the returned buffer is the next segment from the preferred block.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (SegmentBuffer Buffer, bool IsNextBlockSegment) RentInternal (long requestedBufferSize,
        bool clearNewAllocations, in SegmentBufferInfo preferredBlockInfo) =>
        RentFromGeneration(requestedBufferSize, Volatile.Read(ref _arrayGroup), clearNewAllocations, preferredBlockInfo);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySegmentedBufferPool"/> class.
    /// </summary>
    /// <param name="useNativeMemory">
    /// Indicates if we should use native memory for the buffers.
    /// </param>
    public MemorySegmentedBufferPool (bool useNativeMemory)
    {
        _useNativeMemory = useNativeMemory;
        _arrayGroup = MemorySegmentedGroupGenerationArray.GetInitialArray(useNativeMemory);
        _trimTimer = new Timer(TrimBufferGroups, this, TrimTimerIntervalMs, TrimTimerIntervalMs);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a buffer that is the size of the buffer size for this pool.
    /// </summary>
    /// <param name="requestedBufferSize">
    /// The desired size of the buffer to rent.
    /// </param>
    /// <param name="clearNewAllocations">
    /// Indicates whether we need new allocations to be cleared.
    /// </param>
    /// <returns>
    /// A buffer that is the size of the buffer size for this pool.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SegmentBuffer Rent (long requestedBufferSize, bool clearNewAllocations)
    {
        Debug.Assert(requestedBufferSize > 0, "requestedBufferSize <= 0");
        Debug.Assert(0 == (requestedBufferSize % MemorySegmentedBufferGroup.StandardBufferSegmentSize), "requestedBufferSize is not a multiple of the segment size");
        return RentInternal(requestedBufferSize, clearNewAllocations);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a buffer that is the size of the buffer size for this pool.
    /// </summary>
    /// <param name="requestedBufferSize">
    /// The desired size of the buffer to rent.
    /// </param>
    /// <param name="clearNewAllocations">
    /// Indicates whether we need new allocations to be cleared.
    /// </param>
    /// <param name="preferredBlockInfo">
    /// Information about the last block that was used to allocate a buffer. This is used
    /// to try and allocate from the same block and next segment if possible
    /// </param>
    /// <returns>
    /// A buffer that is the size of the buffer size for this pool and a flag indicating 
    /// if the returned buffer is the next segment from the preferred block.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (SegmentBuffer Buffer, bool IsNextBlockSegment) RentFromPreferredBlock
        (long requestedBufferSize, bool clearNewAllocations, in SegmentBufferInfo preferredBlockInfo)
    {
        Debug.Assert(requestedBufferSize > 0, "requestedBufferSize <= 0");
        Debug.Assert(0 == (requestedBufferSize % MemorySegmentedBufferGroup.StandardBufferSegmentSize), "requestedBufferSize is not a multiple of the segment size");
        return RentInternal(requestedBufferSize, clearNewAllocations, preferredBlockInfo);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a buffer to the pool.
    /// </summary>
    /// <param name="buffer">
    /// The buffer to be returned to the pool.
    /// </param>
    /// <param name="clearArraySetting">
    /// Indicates whether the buffer should be cleared before being stored.
    /// </param>
    public void Return (in SegmentBuffer buffer, MemoryStreamSlimZeroBufferOption clearArraySetting)
    {
        Debug.Assert(!buffer.IsEmpty, "buffer.IsEmpty");

        do
        {
            switch (clearArraySetting)
            {
                case MemoryStreamSlimZeroBufferOption.OutOfBand:
                {
                    _pendingZeroList.Add(buffer);
                    TriggerZeroProcessing();
                    return;
                }

                case MemoryStreamSlimZeroBufferOption.OnRelease:
                    buffer.Clear();
                    break;
            }

            // Volatile read of the array group generation to get our local reference.
            MemorySegmentedGroupGenerationArray generationArray = Volatile.Read(ref _arrayGroup);
            // Store the buffer back in the group.
            if (generationArray.ReleaseBuffer(buffer, clearArraySetting == MemoryStreamSlimZeroBufferOption.OnRelease))
                // If the release was not done, that will mean that we have the wrong generation, which
                // in this case should be impossible, because having the wrong generation would mean that
                // the buffer comes from a later generation, and we should have the most recent generation 
                // here, or certainly a generation that contains a buffer being released because it would
                // be impossible for the buffer to be fetched AFTER we get the reference to the generation.
                // This check is really more appropriate for the out-of-band zeroing operation, where a 
                // reference to the generation is grabbed before a series of buffers are zeroed out.
                return;
        } while (true);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Reduces the use of the buffer by releasing some of the segments and returning 
    /// the buffer segments to the pool.
    /// </summary>
    /// <param name="buffer">
    /// The buffer to be reduced.
    /// </param>
    /// <param name="newSegmentCount">
    /// The new number of segments that the buffer should hold.
    /// </param>
    /// <param name="clearReleasedArraySetting">
    /// Indicates whether the buffer should be cleared before being stored.
    /// </param>
    public SegmentBuffer Reduce (in SegmentBuffer buffer, int newSegmentCount,
        MemoryStreamSlimZeroBufferOption clearReleasedArraySetting)
    {
        Debug.Assert(newSegmentCount < buffer.SegmentCount, "newSegmentCount >= buffer.SegmentCount");

        (SegmentBuffer firstSegment, SegmentBuffer secondSegment) = buffer.Split(newSegmentCount);
        Return(secondSegment, clearReleasedArraySetting);
        return firstSegment;
    }
    //--------------------------------------------------------------------------------

    #region Implementation of IDisposable

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public void Dispose ()
    {
        _trimTimer.Dispose();
    }
    //--------------------------------------------------------------------------------

    #endregion
}
//################################################################################
