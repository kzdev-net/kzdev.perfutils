// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// An implementation of a buffer pool that is used for the fixed,
    /// standard sized buffers in the <see cref="SegmentMemoryStreamSlim"/> class.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
    internal class MemorySegmentedBufferPool
    {
        /// <summary>
        /// The maximum number of milliseconds that we will spend zeroing out buffers because we 
        /// process in a thread pool thread. This is a balance between zeroing out buffers quickly
        /// and not taking too much time in the thread pool.
        /// </summary>
        private const int MaxZeroProcessingMs = 400;

        /// <summary>
        /// We would prefer to process no more than this number of zero operations per thread-pool
        /// delegate cycle. But, if the pending list is getting too large, we will process at least
        /// half of the pending list.
        /// </summary>
        private const int PreferMaximumZeroOperationsPerCycle = 100;

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
        /// A flag indicating if the zero processing operation is currently active.
        /// </summary>
        /// <remarks>
        /// The /* volatile */ comment is here to remind us that all accesses to this field
        /// should be done using the Volatile class, but we want to be explicit about it in the
        /// code, so we don't actually use the volatile keyword here.
        /// </remarks>
        private /* volatile */ int _zeroProcessingActive;

        /// <summary>
        /// A list of returned buffers that are awaiting zeroing.
        /// </summary>
        private readonly ConcurrentBag<SegmentBuffer> _pendingZeroList = [];

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
            // Track if there is more work to do - assume there is
            bool moreWork = true;
            try
            {
                try
                {
                    // We are running on a thread pool thread, so be mindful of the time we are taking.
                    Stopwatch runningTime = Stopwatch.StartNew();
                    // Get the reference to the lists.
                    ConcurrentBag<SegmentBuffer> pendingZeroList = poolInstance._pendingZeroList;
#if FALSE
                    // Track the number of zero operations we have done and how many were pending when we started.
                    int startPendingCount = pendingZeroList.Count;
#endif
                    int completedCount = 0;
                    // Determine how many loops we will run.
                    int runLoops = Math.Max(pendingZeroList.Count / 2, PreferMaximumZeroOperationsPerCycle);
                    // Volatile read of the array group generation to get our local reference.
                    MemorySegmentedGroupGenerationArray generationArray = Volatile.Read(ref poolInstance._arrayGroup);

                    // Now, loop through the pending list and zero out the buffers.
                    for (int loopIndex = 0; loopIndex < runLoops; loopIndex++)
                    {
                        if (!pendingZeroList.TryTake(out SegmentBuffer releaseBuffer))
                            break;
                        releaseBuffer.Clear();
                        // Store the buffer back in the group.
                        if (!generationArray.ReleaseBuffer(releaseBuffer, true))
                        {
                            // If we get here, this means that the buffer actually came from a generation that is newer
                            // than the one we grabbed a reference to. So, the buffer group was not found.
                            // We could try to get the most recent generation and just loop again,
                            // but we might just as well break out and let the next round just pick
                            // up from the beginning, since could possibly indicate things are pretty busy.
                            // We are going to take a bit of a performance hit here because we will zero the buffer more
                            // than once, but we need to make sure this segment gets properly released, and we need to
                            // put it back into the pending zero list to be zeroed out again on the next pass.
                            // This is the safest this to do, and this condition will be extremely rare.
                            pendingZeroList.Add(releaseBuffer);
                            break;
                        }

                        // Occasionally check the time we have been running.
                        if (0 != (++completedCount % 20) || (runningTime.ElapsedMilliseconds <= MaxZeroProcessingMs))
                            continue;

                        // We have been running for too long, so we should stop and return the thread to the pool.

#if FALSE
                        // Check if we should dedicate a thread to this operation.
                        if (pendingZeroList.Count > startPendingCount)
                        {
                            // We are falling behind, so we should consider dedicating a thread to this operation.
                            // TODO: Consider dedicating a thread to this operation.
                        }
#endif
                        break;
                    }
                    // Now, determine the actual state of if there is more work to do.
                    // If there are still items in the pending list, trigger another zeroing operation.
                    moreWork = pendingZeroList.Count > 0;
                }
                finally
                {
                    // Indicate that we are done processing.
                    Volatile.Write(ref poolInstance._zeroProcessingActive, 0);
                    // If there is more work to do, trigger another zeroing operation.
                    if (moreWork)
                        poolInstance.TriggerZeroProcessing();
                }
            }
            catch
            {
                // Ignored - there really isn't anything we can do here.
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Expands the array group by creating a new array group generation and assigning it
        /// as the current group.
        /// </summary>
        /// <param name="currentGeneration">
        /// The current generation of the array group that we want to expand from.
        /// </param>
        private MemorySegmentedGroupGenerationArray ExpandArrayGroup (MemorySegmentedGroupGenerationArray currentGeneration)
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
            MemorySegmentedGroupGenerationArray newGeneration = new MemorySegmentedGroupGenerationArray(currentGeneration, _useNativeMemory);
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
        /// <param name="requestedSegmentCount">
        /// The number of segments that are being requested.
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
            RentFromGeneration (int requestedBufferSize, int requestedSegmentCount,
            MemorySegmentedGroupGenerationArray generationArray, bool clearNewAllocations, 
            in SegmentBufferInfo preferredBlockInfo)
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
                return (RentFromGeneration(requestedBufferSize, requestedSegmentCount, generationArray, clearNewAllocations), false);

            // Try to get the buffer from the preferred group.
            (SegmentBuffer buffer, GetBufferResult getResult, bool segmentIsPreferredInBlock) =
                preferredGroup.GetBuffer(requestedBufferSize, clearNewAllocations, this, preferredBlockInfo.SegmentId + preferredBlockInfo.SegmentCount);
            return (getResult == GetBufferResult.Available) ? (buffer, segmentIsPreferredInBlock) :
                // The group is either locked or the preferred segment was not available, so we will fall
                // back to the normal rent operation.
                (RentFromGeneration(requestedBufferSize, requestedSegmentCount, generationArray, clearNewAllocations), false);
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
        /// <param name="requestedSegmentCount">
        /// The number of segments that are being requested.
        /// </param>
        /// <param name="generationArray">
        /// The generation array that we are renting from.
        /// </param>
        /// <returns>
        /// A buffer that is the size of the buffer size for this pool.
        /// </returns>
        private SegmentBuffer RentFromGeneration (int requestedBufferSize, int requestedSegmentCount,
            MemorySegmentedGroupGenerationArray generationArray, bool clearNewAllocations)
        {
            int lockedLoops = 0;
            // For larger segment counts, we will look backwards in the group array since the latter groups are likely larger.
            bool lookBackwards = requestedSegmentCount >= (generationArray.MaxGroupSegmentCount / 2);

            int groupCount = generationArray.Groups.Length;
            while (true)
            {
                bool anyGroupsLocked = false;
                // ** NOTE: The code in the loops below are duplicated to avoid the overhead of a method call and/or checking
                // the lookBackwards flag on each iteration. Not ideal, but it is a performance optimization.
                // Any change to one loop should be reflected in the other loop.
                if (lookBackwards)
                {
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
                }
                else
                {
                    for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
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
                    generationArray = ExpandArrayGroup(generationArray);
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
        private SegmentBuffer RentInternal (int requestedBufferSize, bool clearNewAllocations) =>
            RentFromGeneration(requestedBufferSize, requestedBufferSize / MemorySegmentedBufferGroup.StandardBufferSegmentSize,
                Volatile.Read(ref _arrayGroup), clearNewAllocations);
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
        private (SegmentBuffer Buffer, bool IsNextBlockSegment) RentInternal (int requestedBufferSize, 
            bool clearNewAllocations, in SegmentBufferInfo preferredBlockInfo) =>
            RentFromGeneration(requestedBufferSize, requestedBufferSize / MemorySegmentedBufferGroup.StandardBufferSegmentSize,
                Volatile.Read(ref _arrayGroup), clearNewAllocations, preferredBlockInfo);
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
        public SegmentBuffer Rent (int requestedBufferSize, bool clearNewAllocations)
        {
            Debug.Assert(requestedBufferSize > 0, "requestedBufferSize <= 0");
            Debug.Assert(0 == (requestedBufferSize % MemorySegmentedBufferGroup.StandardBufferSegmentSize), "requestedBufferSize is not a multiple of the segment size");
            return RentInternal (requestedBufferSize, clearNewAllocations);
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
            (int requestedBufferSize, bool clearNewAllocations, in SegmentBufferInfo preferredBlockInfo)
        {
            Debug.Assert(requestedBufferSize > 0, "requestedBufferSize <= 0");
            Debug.Assert(0 == (requestedBufferSize % MemorySegmentedBufferGroup.StandardBufferSegmentSize), "requestedBufferSize is not a multiple of the segment size");
            return RentInternal (requestedBufferSize, clearNewAllocations, preferredBlockInfo);
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
    }
    //################################################################################
}
