﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KZDev.PerfUtils.Observability;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// Indicates the result of the get buffer operation.
/// </summary>
internal enum GetBufferResult
{
    /// <summary>
    /// A buffer was available and returned.
    /// </summary>
    Available,
    /// <summary>
    /// The group is currently locked.
    /// </summary>
    GroupLocked,
    /// <summary>
    /// The group is full.
    /// </summary>
    GroupFull,
    /// <summary>
    /// The group has been released (no longer available)
    /// </summary>
    Released
}
//================================================================================
/// <summary>
/// This manages all the segmented memory buffers used for standard size buffer segments
/// by the <see cref="SegmentMemoryStreamSlim"/> class. This is done by managing larger
/// chunks of memory allocated from the GC and then segmenting that memory into the
/// different buffer sizes that are needed by capturing 1..n segments in a single 
/// virtual buffer.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
internal unsafe class MemorySegmentedBufferGroup
{
    /// <summary>
    /// The maximum size of an individual standard buffer we will use. This allows us
    /// to think of the memory as a series of segments that are all the same size.
    /// </summary>
    protected internal const int StandardBufferSegmentSize = 0x1_0000;

    //================================================================================
    /// <summary>
    /// Represents a range of segments that are unused.
    /// </summary>
    private readonly struct UnusedSegmentRange (int segmentIndex, int segmentCount, bool zeroed, int flagIndex, ulong flagMask)
    {
        /// <summary>
        /// The first index in the range.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public readonly int SegmentIndex = segmentIndex;

        /// <summary>
        /// The contiguous number of segments in the range.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public readonly int SegmentCount = segmentCount;

        /// <summary>
        /// Indicates if the segments are zeroed.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public readonly bool Zeroed = zeroed;

        /// <summary>
        /// The index of the flag set for the range.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public readonly int FlagIndex = flagIndex;

        /// <summary>
        /// The mask to use for the flag set.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public readonly ulong FlagMask = flagMask;

        /// <summary>
        /// Deconstructs the range into its parts.
        /// </summary>
        /// <param name="segmentIndex">
        /// The first index in the range.
        /// </param>
        /// <param name="segmentCount">
        /// The contiguous number of segments in the range.
        /// </param>
        /// <param name="zeroed">
        /// Indicates if the segments are zeroed.
        /// </param>
        /// <param name="flagIndex">
        /// The index of the flag set for the range.
        /// </param>
        /// <param name="flagMask">
        /// The mask to use for the flag set.
        /// </param>
        public void Deconstruct (out int segmentIndex, out int segmentCount, out bool zeroed, out int flagIndex, out ulong flagMask)
        {
            segmentIndex = SegmentIndex;
            segmentCount = SegmentCount;
            zeroed = Zeroed;
            flagIndex = FlagIndex;
            flagMask = FlagMask;
        }
    }
    //================================================================================

    /// <summary>
    /// Debug helper to display the state of the group.
    /// </summary>
    [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
    private string DebugDisplayValue => $"ID {Id}, Count = {_segmentCount}, InUse = {_segmentsInUse}";
#pragma warning restore HAA0601

    /// <summary>
    /// The high bit mask for an unsigned long.
    /// </summary>
    internal const ulong HighBitMask = 0x8000_0000_0000_0000;

    /// <summary>
    /// The size of the block flag set/group (contained in an unsigned long)
    /// </summary>
    internal const int BlockFlagSetSize = 64;  // 64 bits in a ulong

    /// <summary>
    /// The total number of GC Heap segments allocated.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be done using the Volatile class, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private static /* volatile */ int _totalGcSegmentsAllocated;

    /// <summary>
    /// The total number of Native Heap segments allocated.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be done using the Volatile class, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private static /* volatile */ int _totalNativeSegmentsAllocated;

    /// <summary>
    /// The last ID used for a group.
    /// </summary>
    private static int _lastGroupId;

    /// <summary>
    /// The total number of segments that we have in this block that can be used for buffers.
    /// </summary>
    private readonly int _segmentCount;

    /// <summary>
    /// A flag indicating if the group is locked for use.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be treated as Volatile, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private /* volatile */ int _locked;

    /// <summary>
    /// A running count of the number of segments in use.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be treated as Volatile, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private /* volatile */ int _segmentsInUse;

    /// <summary>
    /// Tracks a count of every time the group has been emptied. This is used as a 
    /// sort of 'version' for determining when the group can be trimmed/released
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be treated as Volatile, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private /* volatile */ uint _emptiedCount;

    /// <summary>
    /// The value of _emptiedCount the last time the trim operation was executed to
    /// check for whether this group can be released or not.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be treated as Volatile, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private /* volatile */ uint _lastTrimCheckEmptiedCount = uint.MaxValue;

    /// <summary>
    /// A bitmask of flags indicating which buffers are in use.
    /// </summary>
    private readonly ulong[] _blockUsedFlags;

    /// <summary>
    /// A bitmask of flags indicating which buffers are zeroed.
    /// </summary>
    private readonly ulong[] _blockZeroFlags;

    /// <summary>
    /// The chunk of memory that we segment out for the buffers in this group.
    /// </summary>
    private byte[]? _bufferBlock;

    /// <summary>
    /// The pointer to the buffer block if we are using native memory.
    /// </summary>
    private byte* _bufferBlockPtr;

    /// <summary>
    /// Indicates if we are using native memory for the buffers instead of GC memory.
    /// </summary>
    private readonly bool _useNativeMemory;

    /// <summary>
    /// Indicates whether this group has been released.
    /// </summary>
    private bool _released;

    /// <summary>
    /// Updates the total number of allocated segments. The update amount can be negative.
    /// </summary>
    /// <param name="updateAmount">
    /// The amount to update the total number of allocated segments by.
    /// </param>
    /// <param name="nativeSegments">
    /// If <c>true</c> then the update is for native segments, otherwise it is for GC segments.
    /// </param>
    private static void UpdateAllocatedSegmentCount (int updateAmount, bool nativeSegments)
    {
        if (nativeSegments)
            Interlocked.Add(ref _totalNativeSegmentsAllocated, updateAmount);
        else
            Interlocked.Add(ref _totalGcSegmentsAllocated, updateAmount);
    }

    /// <summary>
    /// Indicates if the group is full.
    /// </summary>
#if NOT_PACKAGING
    internal
#else
        private
#endif
        bool IsFull
    {
        [DebuggerStepThrough]
        get => Volatile.Read(ref _segmentsInUse) == _segmentCount;
    }

    /// <summary>
    /// Returns whether this buffer group has been marked as released.
    /// </summary>
    internal bool IsReleased => Volatile.Read(ref _released);

    /// <summary>
    /// Gets the flag index and mask to use for the specified segment index.
    /// </summary>
    /// <param name="segmentIndex">
    /// The index of the segment of the block we want to get the flags for.
    /// </param>
    /// <returns>
    /// The index of the flag set instance and the mask to use for the segment index.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int Index, ulong Mask) GetFlagIndexAndMask (int segmentIndex) =>
        (segmentIndex >> 6  /* segmentIndex / BlockFlagSetSize */, 1UL << (segmentIndex & 0x3F /* segmentIndex % BlockFlagSetSize */));

    /// <summary>
    /// Clears the used flag bit for the specified segment index and range.
    /// </summary>
    /// <param name="segmentIndex">
    /// The first segment within the buffer block that we want to clear the flag for.
    /// </param>
    /// <param name="segmentCount">
    /// The number of contiguous segments to clear the flag for.
    /// </param>
    /// <param name="segmentsAreZeroed">
    /// Indicates if the memory space associated with the segments has been zeroed.
    /// </param>
    private void ReleaseSegments (int segmentIndex, int segmentCount, bool segmentsAreZeroed)
    {
        Debug.Assert(segmentCount > 0, "segmentCount <= 0");
        // Get the index of the flag set and the mask for the segment index.
        (int flagIndex, ulong flagMask) = GetFlagIndexAndMask(segmentIndex);
        int segmentsLeft = segmentCount;
        // Clear the used flag bit for the segment indexes.
        ulong usedFlagGroup = _blockUsedFlags[flagIndex];
        ulong zeroFlagGroup = _blockZeroFlags[flagIndex];
        // Given how we are using and updating the flag information as well as tracking
        // how many segments are in use, we can't simply check while (segmentsLeft > 0) so
        // we simply break when we are done.
        while (true)
        {
            Debug.Assert((usedFlagGroup & flagMask) != 0, "(usedFlagGroup & mask) == 0");
            // Clear the used flag bit for the segment index.
            usedFlagGroup &= ~flagMask;
            // Set the zeroed flag bit for the segment index.
            if (segmentsAreZeroed)
                zeroFlagGroup |= flagMask;
            else
                zeroFlagGroup &= ~flagMask;
            // Move to the next segment.
            if (--segmentsLeft == 0) break;
            // Are we at the end of the flag group?
            if (flagMask == HighBitMask)
            {
                // Store our current updates.
                _blockUsedFlags[flagIndex] = usedFlagGroup;
                _blockZeroFlags[flagIndex] = zeroFlagGroup;
                // Reset the mask
                flagMask = 1;
                ++flagIndex;
                // We don't ever use a group of segments as contiguous segments that actually loop around
                // the memory block space, so we should never be at the end of the flag groups.
                Debug.Assert(flagIndex < _blockUsedFlags.Length, "flagIndex >= _blockUsedFlags.Length");
                // Capture the next flag group.
                usedFlagGroup = _blockUsedFlags[flagIndex];
                zeroFlagGroup = _blockZeroFlags[flagIndex];
            }
            else
            {
                flagMask <<= 1;
            }
        }
        // Store the final updates.
        _blockUsedFlags[flagIndex] = usedFlagGroup;
        _blockZeroFlags[flagIndex] = zeroFlagGroup;
        // Update the number of segments in use.
        int newSegmentsInUse = (segmentCount == 1) ?
            Interlocked.Decrement(ref _segmentsInUse) :
            Interlocked.Add(ref _segmentsInUse, segmentCount * -1);
        if (newSegmentsInUse > 0)
            return;
        Interlocked.Increment(ref _emptiedCount);
    }

    /// <summary>
    /// Returns information about the series of segments that are all free
    /// starting at the specified segment index using the flag index and mask.
    /// </summary>
    /// <returns>
    /// The number of contiguous free segments (up to the maximum) and whether all the segments are zeroed.
    /// </returns>
    private (int SegmentCount, bool AllZeroed)
        GetAllFreeSegmentSeriesFromIndex (int firstSegmentIndex, int maxSegments, int flagIndex, ulong flagMask)
    {
        Debug.Assert(maxSegments > 1, "maxSegments <= 1");
        Debug.Assert(firstSegmentIndex >= 0, "firstSegmentIndex < 0");

        // Get a working index for the segment index.
        int workingSegmentIndex = firstSegmentIndex;
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        ulong zeroFlagGroup = _blockZeroFlags[flagIndex];
        // Get the in use status of this segment.
        bool segmentIsUsed = (flagGroup & flagMask) != 0;
        // Track the number of contiguous segments we find.
        int foundSegmentCount = 0;
        // Track if all the segments are zeroed.
        bool allZeroed = true;

        // Looking for a series of free segments
        while (!segmentIsUsed)
        {
            // Check the zero state of the segment.
            if (allZeroed && ((zeroFlagGroup & flagMask) == 0))
                allZeroed = false;
            // Update the flag group that we are using this segment now.
            flagGroup |= flagMask;
            // Increment the number we have
            if (++foundSegmentCount == maxSegments)
                break;
            // Check the next segment.
            if (++workingSegmentIndex == _segmentCount)
                break;
            // If we are at the end of the flag group, then move to the next flag group.
            if (flagMask == HighBitMask)
            {
                // Save the current flag group.
                _blockUsedFlags[flagIndex++] = flagGroup;
                // The working segment index comparison to the segment count above should
                // prevent us from ever going past the end of the flag groups.
                Debug.Assert(flagIndex < _blockUsedFlags.Length, "flagIndex >= _blockUsedFlags.Length");
                // Get the next flag groups and mask.
                flagGroup = _blockUsedFlags[flagIndex];
                zeroFlagGroup = _blockZeroFlags[flagIndex];
                // Reset the mask
                flagMask = 1;
            }
            else
            {
                // Otherwise check the next segment.
                flagMask <<= 1;
            }
            segmentIsUsed = (flagGroup & flagMask) != 0;
        }
        // Store the final updates to the used flags.
        _blockUsedFlags[flagIndex] = flagGroup;
        return (foundSegmentCount, allZeroed);
    }

    /// <summary>
    /// Returns information about the series of segments that are all used
    /// starting at the specified segment index using the flag index and mask.
    /// </summary>
    /// <returns>
    /// The number of contiguous used segments as well as the next flag 
    /// index and mask to continuously check for the next similar series of segments.
    /// </returns>
    private (int SegmentCount, int NextFlagIndex, ulong NextFlagMask)
        GetUsedSegmentSeriesFromIndex (int checkingSegmentIndex, int flagIndex, ulong flagMask)
    {
        Debug.Assert(checkingSegmentIndex >= 0, "checkingSegmentIndex < 0");
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        // Get the in use status of this segment.
        bool segmentIsUsed = (flagGroup & flagMask) != 0;
        // Track the number of contiguous segments we find.
        int foundSegmentCount = 0;

        // Looking for a series of in use segments
        while (segmentIsUsed)
        {
            // Increment the number we have
            foundSegmentCount++;
            // Check the next segment. This MUST be done first to 
            // ensure that when we break, the flag group and mask are correct
            // for returning the next flag group and mask from this method.
            if (++checkingSegmentIndex == _segmentCount)
                break;
            // If we are at the end of the flag group, then move to the next flag group.
            if (flagMask == HighBitMask)
            {
                // If we are at the end of the flag groups, then we are done.
                if (++flagIndex == _blockUsedFlags.Length)
                    break;
                flagGroup = _blockUsedFlags[flagIndex];
                flagMask = 1;
            }
            else
            {
                // Otherwise check the next segment.
                flagMask <<= 1;
            }
            segmentIsUsed = (flagGroup & flagMask) != 0;
        }
        return (foundSegmentCount, flagIndex, flagMask);
    }

    /// <summary>
    /// Returns information about the series of segments that are all free 
    /// starting at the specified segment index and using the provided flag index and mask.
    /// </summary>
    /// <returns>
    /// The number of contiguous free segments with the next flag index and mask to 
    /// continuously check for the next similar series of
    /// segments.
    /// </returns>
    private (int SegmentCount, bool Zeroed, int NextFlagIndex, ulong NextFlagMask)
        GetFreeSegmentSeriesFromIndex (int checkingSegmentIndex, int flagIndex, ulong flagMask, int maxCount)
    {
        Debug.Assert(checkingSegmentIndex >= 0, "checkingSegmentIndex < 0");
        Debug.Assert(maxCount > 0, "maxCount <= 0");
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        ulong zeroFlagGroup = _blockZeroFlags[flagIndex];
        // Get the free status of this segment.
        bool segmentIsFree = (flagGroup & flagMask) == 0;
        // Get the initial zero state of the segment.
        bool allSegmentsZeroed = (zeroFlagGroup & flagMask) != 0;
        int foundSegmentCount = 0;

        while (segmentIsFree)
        {
            // Increment the number we have
            foundSegmentCount++;
            // Check the next segment. This MUST be done first to 
            // ensure that when we break, the flag group and mask are correct
            // for returning the next flag group and mask from this method.
            if (++checkingSegmentIndex == _segmentCount)
                break;
            // If we are at the end of the flag group, then move to the next flag group.
            if (flagMask == HighBitMask)
            {
                if (++flagIndex == _blockUsedFlags.Length)
                    break;
                // Get the next flag groups and mask.
                flagGroup = _blockUsedFlags[flagIndex];
                zeroFlagGroup = _blockZeroFlags[flagIndex];
                flagMask = 1;
            }
            else
            {
                // Otherwise check the next segment.
                flagMask <<= 1;
            }
            segmentIsFree = (flagGroup & flagMask) == 0;
            if ((zeroFlagGroup & flagMask) == 0)
                allSegmentsZeroed = false;
            if (foundSegmentCount >= maxCount)
                break;
        }
        return (foundSegmentCount, allSegmentsZeroed, flagIndex, flagMask);
    }

    /// <summary>
    /// Returns information about the series of segments that are of the same used/free state
    /// starting at the specified segment index and using the provided flag index and mask.
    /// </summary>
    /// <returns>
    /// The series number of segments, if they are used, and if they are all zeroed 
    /// as well as the next flag index and mask to continuously check for the next 
    /// similar series of segments.
    /// </returns>
    private (int SegmentCount, bool Used, bool Zeroed, int NextFlagIndex, ulong NextFlagMask)
        GetSimilarSegmentSeriesFrom (int startingSegmentIndex, int flagIndex, ulong flagMask, int maxFreeSegmentCount)
    {
        Debug.Assert(startingSegmentIndex >= 0, "startingSegmentIndex < 0");
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        // Determine what type of series we're looking for at this segment index.
        bool segmentIsUsed = (flagGroup & flagMask) != 0;
        if (segmentIsUsed)
        {
            (int usedSegmentCount, int usedNextFlagIndex, ulong usedNextFlagMask) =
                GetUsedSegmentSeriesFromIndex(startingSegmentIndex, flagIndex, flagMask);
            return (usedSegmentCount, true, false, usedNextFlagIndex, usedNextFlagMask);
        }
        (int freeSegmentCount, bool freeZeroed, int freeNextFlagIndex, ulong freeNextFlagMask) =
            GetFreeSegmentSeriesFromIndex(startingSegmentIndex, flagIndex, flagMask, maxFreeSegmentCount);
        return (freeSegmentCount, false, freeZeroed, freeNextFlagIndex, freeNextFlagMask);
    }

    /// <summary>
    /// Looks for the series of segments that are unused together with a size at least as
    /// large as the requested number of segments, or the next largest we can find.
    /// </summary>
    /// <returns>
    /// Values indicating the first segment in the series, the number of contiguous segments,
    /// whether all the segments are zeroed, as well as the starting flag index and mask 
    /// for the series of segments.
    /// </returns>
    private UnusedSegmentRange GetFirstUnusedSegmentSeries (int requestedSegments)
    {
        // Get the starting segment index and the flag index and mask for the segment index.
        int flagIndex = 0;
        ulong flagMask = 1;
        int currentSegmentIndex = 0;
        // Set the current 'best match'
        UnusedSegmentRange bestMatch = new(segmentIndex: 0, segmentCount: 0, zeroed: false, flagIndex: 0, flagMask: 0);
        int totalUnusedSegmentCount = _segmentCount - Volatile.Read(ref _segmentsInUse);
        int observedUnusedSegmentCount = 0;

        // Check every segment for the first or best match.
        while (currentSegmentIndex < _segmentCount)
        {
            // Get the next series of segments that are in the same state.
            (int seriesSegmentCount, bool seriesSegmentsUsed, bool seriesSegmentsZeroed, int nextFlagIndex, ulong nextFlagMask) =
                GetSimilarSegmentSeriesFrom(currentSegmentIndex, flagIndex, flagMask, requestedSegments);
            // If we found no matches, then we are done.
            if (seriesSegmentCount == 0)
                break;
            // If the series is a used series, then we can skip over it - we are looking for unused segments.
            if (seriesSegmentsUsed)
            {
                currentSegmentIndex += seriesSegmentCount;
                flagIndex = nextFlagIndex;
                flagMask = nextFlagMask;
                continue;
            }
            // If we have the exact number of segments we want, this is a perfect match, then we are done.
            if (seriesSegmentCount >= requestedSegments)
                return new(segmentIndex: currentSegmentIndex, segmentCount: requestedSegments, zeroed: seriesSegmentsZeroed, flagIndex: flagIndex, flagMask: flagMask);

            // If we have a better match than the current best match, then update the best match.
            if ((bestMatch.SegmentCount == 0) /* No match yet */ ||
                ((seriesSegmentCount > bestMatch.SegmentCount) &&
                 (bestMatch.SegmentCount < requestedSegments)) /* Larger series but still smaller than requested */ ||
                ((seriesSegmentCount < bestMatch.SegmentCount) &&
                 (bestMatch.SegmentCount > requestedSegments) &&
                 (seriesSegmentCount > requestedSegments)) /* Smaller series that is closer to requested */)
                bestMatch = new(segmentIndex: currentSegmentIndex, segmentCount: seriesSegmentCount, zeroed: seriesSegmentsZeroed, flagIndex: flagIndex, flagMask: flagMask);

            // Update the number of unused segments we have observed, and check if we have seen them all. If so, we are done.
            if ((observedUnusedSegmentCount += seriesSegmentCount) >= totalUnusedSegmentCount)
                break;
            // Move on to the next series of segments.
            currentSegmentIndex += seriesSegmentCount;
            flagIndex = nextFlagIndex;
            flagMask = nextFlagMask;
        }
        // Return the best match we found.
        return new(segmentIndex: bestMatch.SegmentIndex, segmentCount: Math.Min(requestedSegments, bestMatch.SegmentCount), zeroed: bestMatch.Zeroed, flagIndex: bestMatch.FlagIndex, flagMask: bestMatch.FlagMask);
    }

    /// <summary>
    /// Reserves a set of segments for use and returns the segment index that was reserved.
    /// </summary>
    /// <returns>
    /// Values indicating the segment reserved, which includes the first segment index,
    /// the number of contiguous segments reserved, and if the segment is zeroed.
    /// </returns>
    private (int SegmentIndex, int SegmentCount, bool Zeroed) ReserveSegments (int requestedSegments)
    {
        Debug.Assert(requestedSegments > 1, "requestedSegments <= 1");
        // Find a series of segments that are unused that are closest to the requested number of segments.
        (int startingSegmentIndex, int segmentCount, bool zeroed, int flagIndex, ulong flagMask) =
            GetFirstUnusedSegmentSeries(requestedSegments);
        Debug.Assert(segmentCount > 0, "segmentCount <= 0");
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];

        // We have the series that we will return, now mark them as in use, and determine the 
        // zeroed state of the segments.
        int markSegmentIndex = 0;
        while (markSegmentIndex < segmentCount)
        {
            markSegmentIndex++;
            // Update the flag group.
            flagGroup |= flagMask;
            // If we are at the end of the flag group, then move to the next flag group.
            if (flagMask == HighBitMask)
            {
                // Save the current flag group.
                _blockUsedFlags[flagIndex] = flagGroup;
                // This check MUST happen before the increment of the flagIndex.
                if (markSegmentIndex == segmentCount)
                    // Break out of the loop early so that we don't increment the flagIndex.
                    break;
                // Reset the mask
                flagMask = 1;
                // Move to the next flag group.
                flagGroup = _blockUsedFlags[++flagIndex];
            }
            else
            {
                flagMask <<= 1;
            }
        }
        // Store the final updates.
        _blockUsedFlags[flagIndex] = flagGroup;
        // Update the number of segments in use.
        Interlocked.Add(ref _segmentsInUse, segmentCount);
        return (startingSegmentIndex, segmentCount, zeroed);
    }

    /// <summary>
    /// Tries to reserve a set of segments for use starting at a specific index and returns whether
    /// the segments were reserved, the number of segments reserved, and if the segments are zeroed.
    /// </summary>
    /// <returns>
    /// Values indicating the segment reserved, which includes the first segment index,
    /// the number of contiguous segments reserved, and if the segment is zeroed. 
    /// </returns>
    private (int SegmentCount, bool Zeroed) TryReserveSegments (int requestedSegments,
        int firstSegmentIndex)
    {
        Debug.Assert(requestedSegments > 1, "requestedSegments <= 1");
        Debug.Assert(firstSegmentIndex > 0, "firstSegmentIndex <= 0");
        // Check if we can even use this segment.
        if (firstSegmentIndex >= _segmentCount)
            return (0, false);
        // Get the index of the flag set and the mask for the segment index.
        (int flagIndex, ulong flagMask) = GetFlagIndexAndMask(firstSegmentIndex);
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        // Check if the segment is in use.
        if ((flagGroup & flagMask) != 0)
        {
            // The first segment is in use.
            return (0, false);
        }

        // Find a series of segments that are unused and treat that series as zeroed ONLY if all the
        // segments in the series are zeroed.
        (int segmentCount, bool zeroed) =
            GetAllFreeSegmentSeriesFromIndex(firstSegmentIndex, requestedSegments, flagIndex, flagMask);
        // Update the number of segments in use.
        Interlocked.Add(ref _segmentsInUse, segmentCount);
        return (segmentCount, zeroed);
    }

    /// <summary>
    /// Reserves a segment index for use and returns the segment index that was reserved.
    /// and if the segment is zeroed.
    /// </summary>
    /// <returns>
    /// Values indicating the segment reserved, which includes the first segment index,
    /// the number of contiguous segments reserved, and if the segment is zeroed.
    /// </returns>
    private (int SegmentIndex, bool Zeroed) ReserveSegment ()
    {
        // Move forward from the start of the block to find the first unused segment.
        int checkSegmentIndex = 0;
        // Get the index of the flag set and the mask for the segment index.
        (int flagIndex, ulong flagMask) = GetFlagIndexAndMask(checkSegmentIndex);
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        while (true)
        {
            // Check if the segment is in use.
            if ((flagGroup & flagMask) == 0)
            {
                // Mark the segment as in use.
                _blockUsedFlags[flagIndex] = flagGroup | flagMask;
                Interlocked.Increment(ref _segmentsInUse);
                // Return the index and the zeroed state of the segment.
                return (checkSegmentIndex, (_blockZeroFlags[flagIndex] & flagMask) != 0);
            }
            // Check the next segment.
            checkSegmentIndex++;
            Debug.Assert(checkSegmentIndex < _segmentCount, "checkSegmentIndex >= _segmentCount");
            // If we are at the end of the flag group, then move to the next flag group.
            if (flagMask == HighBitMask)
            {
                if (++flagIndex == _blockUsedFlags.Length)
                    break;
                // Get the next flag group and mask.
                flagGroup = _blockUsedFlags[flagIndex];
                flagMask = 1;
                continue;
            }
            // Otherwise move to the next segment
            flagMask <<= 1;
        }
        // We should never get here because we should always have a single free segment if 
        // this is called, because the full state should have already been checked.
        Debug.Fail("No free segments found.");
        return (-1, false);
    }

    /// <summary>
    /// Tries to reserve a specific segment index for use and returns whether that segment index was reserved.
    /// and if the segment is zeroed.
    /// </summary>
    /// <param name="segmentIndex">
    /// The specific segment index to try and reserve.
    /// </param>
    /// <returns>
    /// Values indicating the segment reserved, and if the segment is zeroed.
    /// </returns>
    private (bool SegmentReserved, bool Zeroed) TryReserveSegment (int segmentIndex)
    {
        Debug.Assert(segmentIndex > 0, "segmentIndex <= 0");
        // Check if we can even use this segment.
        if (segmentIndex >= _segmentCount)
            return (false, false);
        // Get the index of the flag set and the mask for the segment index.
        (int flagIndex, ulong flagMask) = GetFlagIndexAndMask(segmentIndex);
        // Get the flag set for the segment index.
        ulong flagGroup = _blockUsedFlags[flagIndex];
        // Check if the segment is in use.
        if ((flagGroup & flagMask) != 0)
        {
            // The segment is in use.
            return (false, false);
        }

        // Mark the segment as in use.
        _blockUsedFlags[flagIndex] = flagGroup | flagMask;
        Interlocked.Increment(ref _segmentsInUse);
        // Return the index and the zeroed state of the segment.
        return (true, (_blockZeroFlags[flagIndex] & flagMask) != 0);
    }

    /// <summary>
    /// Checks if the buffer block has been allocated and allocates it if needed.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the buffer block has been allocated, <c>false</c> if it has not been allocated.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool AllocateHeapBufferIfNeeded ()
    {
        if (_bufferBlock is not null) return false;

        int allocationSize = _segmentCount * StandardBufferSegmentSize;
        _bufferBlock = GC.AllocateUninitializedArray<byte>(allocationSize);
        UpdateAllocatedSegmentCount(_segmentCount, false);
        UtilsEventSource.Log.BufferGcMemoryAllocated(allocationSize);
        return true;
    }

    /// <summary>
    /// Checks if the native memory buffer block has been allocated and allocates it if needed.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the native memory buffer block has been allocated, <c>false</c> if it has not been allocated.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool AllocateNativeBufferIfNeeded ()
    {
        if (_bufferBlockPtr is not null) return false;

        int allocationSize = _segmentCount * StandardBufferSegmentSize;
        _bufferBlockPtr = (byte*)NativeMemory.Alloc((nuint)allocationSize);
        UpdateAllocatedSegmentCount(_segmentCount, true);
        UtilsEventSource.Log.BufferNativeMemoryAllocated(allocationSize);
        return true;
    }

    /// <summary>
    /// Checks if the buffer block has been allocated and allocates it if needed.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the buffer block has been allocated, <c>false</c> if it has not been allocated.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool AllocateBufferIfNeeded () => _useNativeMemory ? AllocateNativeBufferIfNeeded() : AllocateHeapBufferIfNeeded();

    /// <summary>
    /// Performs the operation of releasing the internal memory used by the buffer group.
    /// </summary>
    private void ReleaseInternalMemory ()
    {
        if (_bufferBlock is not null)
        {
            // Reduce the total number of allocated segments.
            UpdateAllocatedSegmentCount(-_segmentCount, false);
            _bufferBlock = null;
            UtilsEventSource.Log.BufferGcMemoryReleased(_segmentCount * StandardBufferSegmentSize);
            return;
        }
        // If we have nothing to clean up, then we are done.
        if (_bufferBlockPtr is null)
            return;
        // Reduce the total number of allocated segments.
        UpdateAllocatedSegmentCount(-_segmentCount, true);
        NativeMemory.Free(_bufferBlockPtr);
        _bufferBlockPtr = null;
        UtilsEventSource.Log.BufferNativeMemoryReleased(_segmentCount * StandardBufferSegmentSize);
    }
    /// <summary>
    /// Checks if this group is currently available for release and whether the group
    /// is empty, and has been empty for the last two consecutive checks of emptiness.
    /// </summary>
    /// <param name="releaseMemoryOnly">
    /// If this is <see langword="true"/>, then we will only release the memory associated 
    /// with this group if we would have otherwise fully released the group. This allows
    /// us to have the group available for use again, but we can at least release the
    /// memory associated with this instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the group is available for release, <see langword="false"/> otherwise.
    /// </returns>
    private bool ReleaseGroup (bool releaseMemoryOnly)
    {
        if (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
            return false;
        try
        {
            // If this is already released, just return
            if (_released)
                return true;
            // If we have segments in use, then we can't release the group.
            if (_segmentsInUse > 0)
                return false;
            // If the tracking emptied count is the same as the last time we checked,
            // then we can release the group, otherwise return false.
            if (_emptiedCount != _lastTrimCheckEmptiedCount)
            {
                _lastTrimCheckEmptiedCount = _emptiedCount;
                return false;
            }
            // Now, we are ready to release the group, but check whether
            // we should simply release the memory associated with the group
            // or if we should fully release the group.
            if (releaseMemoryOnly)
            {
                ReleaseInternalMemory();
                // Reset our tracking values
                _emptiedCount = 0;
                _lastTrimCheckEmptiedCount = uint.MaxValue;
                // We may have released memory, but we didn't mark the group as released,
                // so return false.
                return false;
            }
            _released = true;
            ReleaseInternalMemory();
            GC.SuppressFinalize(this);
            return true;
        }
        finally
        {
            Volatile.Write(ref _locked, 0);
        }
    }

    /// <summary>
    /// Creates and initializes a new instance of the <see cref="MemorySegmentedBufferGroup"/> class
    /// that has the internal memory buffer allocated and ready for use.
    /// </summary>
    /// <param name="bufferSegmentCount">
    /// The number of standard size buffer blocks that will be stored in this group.
    /// </param>
    /// <param name="useNativeMemory">
    /// Indicates if we should use native memory for the buffers instead of GC memory.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="MemorySegmentedBufferGroup"/> class that has 
    /// the internal memory buffer allocated.
    /// </returns>
    internal static MemorySegmentedBufferGroup 
        GetMemorySegmentedBufferGroupWithAllocatedBuffer (int bufferSegmentCount, bool useNativeMemory)
    {
        MemorySegmentedBufferGroup returnBufferGroup = new MemorySegmentedBufferGroup(bufferSegmentCount, useNativeMemory);
        returnBufferGroup.AllocateBufferIfNeeded();
        return returnBufferGroup;
    }

    /// <summary>
    /// Static constructor for the <see cref="MemorySegmentedBufferGroup"/> class.
    /// </summary>
    static MemorySegmentedBufferGroup ()
    {
        // Create the observable gauges for the total allocated memory.
#pragma warning disable HAA0601
        MemoryMeter.Meter.CreateObservableGauge("segment_memory.gc_allocated", static () => _totalGcSegmentsAllocated,
            unit: "{segments}", description: $"The total number of GC heap segments (of {StandardBufferSegmentSize:N0} bytes) allocated for the segmented memory buffers");
        MemoryMeter.Meter.CreateObservableGauge("segment_memory.native_allocated", static () => _totalNativeSegmentsAllocated,
            unit: "{segments}", description: $"The total number of native heap segments (of {StandardBufferSegmentSize:N0} bytes) allocated for the segmented memory buffers");
#pragma warning restore HAA0601
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySegmentedBufferGroup"/> class.
    /// </summary>
    /// <param name="bufferSegmentCount">
    /// The number of standard size buffer blocks that will be stored in this group.
    /// </param>
    /// <param name="useNativeMemory">
    /// Indicates if we should use native memory for the buffers instead of GC memory.
    /// </param>
    public MemorySegmentedBufferGroup (int bufferSegmentCount, bool useNativeMemory)
    {
        _useNativeMemory = useNativeMemory;
        _segmentCount = bufferSegmentCount;
        // How many longs do we need to store the flags for the buffers.
        int flagArraySize = (bufferSegmentCount + BlockFlagSetSize - 1) / BlockFlagSetSize;
        // Set up the block flags and the buffer block.
        _blockUsedFlags = new ulong[flagArraySize];
        _blockZeroFlags = new ulong[flagArraySize];
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MemorySegmentedBufferGroup"/> class.
    /// </summary>
    ~MemorySegmentedBufferGroup ()
    {
        ReleaseInternalMemory();
    }

    /// <summary>
    /// Checks if this group is currently available for release and whether the group
    /// is empty, and has been empty for the last two consecutive checks of emptiness.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the group is available for release, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReleaseGroup () => ReleaseGroup(false);

    /// <summary>
    /// Checks if this group is currently available for release and whether the group
    /// is empty, and has been empty for the last two consecutive checks of emptiness.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the group is available for release, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReleaseUnusedMemory () => ReleaseGroup(true);

    /// <summary>
    /// Attempts to store a buffer in the group.
    /// </summary>
    /// <param name="buffer">
    /// The buffer to be stored in this group.
    /// </param>
    /// <param name="segmentIsZeroed">
    /// Indicates if the memory space associated with the segment has been zeroed.
    /// </param>
    /// <returns>
    /// <c>true</c> if the buffer was stored, <c>false</c> if the group is full or
    /// if we weren't able to get a lock on the group. This also returns an indicator
    /// why the buffer was not stored.
    /// </returns>
    public void ReleaseBuffer (in SegmentBuffer buffer, bool segmentIsZeroed)
    {
        Debug.Assert(buffer.BufferInfo.BlockId == Id, "buffer.Id.BlockId != Id");
        Debug.Assert(buffer.BufferInfo.SegmentId < _segmentCount && buffer.BufferInfo.SegmentId >= 0, "buffer.Id.SegmentId < 0 OR buffer.Id.SegmentId >= _segmentCount");

        // Lock access to the group.
        if (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
        {
            SpinWait spinner = new();
            while (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
            {
                spinner.SpinOnce();
            }
        }
        try
        {
            // Release the segments.
            ReleaseSegments(buffer.BufferInfo.SegmentId, buffer.BufferInfo.SegmentCount, segmentIsZeroed);
        }
        finally
        {
            Volatile.Write(ref _locked, 0);
        }
    }

    /// <summary>
    /// Attempts to get a buffer from the group.
    /// </summary>
    /// <param name="bufferSize">
    /// The size of the buffer that we need.
    /// </param>
    /// <param name="requireZeroed">
    /// Indicates if we need to force the buffer to be zeroed.
    /// </param>
    /// <param name="bufferPool">
    /// The buffer pool that is requesting the buffer.
    /// </param>
    /// <param name="preferredFirstSegmentIndex">
    /// The index of the first segment to try and allocate from. If this is -1, then we will
    /// not try to allocate from a specific segment.
    /// </param>
    /// <returns>
    /// The buffer to use from this group if available, and a result value for the operation.
    /// </returns>
    public (SegmentBuffer Buffer, GetBufferResult Result, bool SegmentIsPreferred)
        GetBuffer (long bufferSize, bool requireZeroed, MemorySegmentedBufferPool bufferPool, int preferredFirstSegmentIndex)
    {
        Debug.Assert(0 == (bufferSize % StandardBufferSegmentSize), "bufferSize is not an even multiple of StandardBufferSegmentSize");
        int segmentsNeeded = (int)(bufferSize / StandardBufferSegmentSize);
        // The index of the first segment we have reserved. This will be
        // -1 if we don't have a segment reserved yet.
        int reservedSegmentIndex = -1;
        // The number of segments we have reserved.
        int reservedSegmentCount = 0;
        // Indicates if the segments we have are zeroed.
        bool segmentsZeroed = false;
        // Flag that indicates the segment we have is the preferred segment.
        bool segmentIsPreferred = false;

        if (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
            return (SegmentBuffer.Empty, GetBufferResult.GroupLocked, false);
        try
        {
            // Check if this group has been trimmed/released
            if (_released)
                return (SegmentBuffer.Empty, GetBufferResult.Released, false);
            // We allocate the block when needed. It is not ideal that we are doing an allocation while holding the lock, but
            // we hope that with it being uninitialized, it will be fast enough, plus the return code of any race condition callers
            // will allow them to handle the lock externally. If we didn't hold the lock and just did the allocation, then we could
            // possibly allocate a buffer segment and then immediately release it, which is very likely even a worse condition.
            if ((!AllocateBufferIfNeeded()) && IsFull)
                return (SegmentBuffer.Empty, GetBufferResult.GroupFull, false);

            // Reserve the segments
            if (segmentsNeeded == 1)
            {
                // Try to get the preferred segment first.
                if (preferredFirstSegmentIndex >=0)
                {
                    // Since we are asking for a preferred segment, we can assign the returned value
                    // of "SegmentReserved" to the "segmentIsPreferred" variable.
                    (segmentIsPreferred, segmentsZeroed) = TryReserveSegment(preferredFirstSegmentIndex);
                    if (segmentIsPreferred)
                        reservedSegmentIndex = preferredFirstSegmentIndex;
                }
                // If we didn't get the preferred segment, then try to get any segment.
                if (reservedSegmentIndex < 0)
                {
                    (reservedSegmentIndex, segmentsZeroed) = ReserveSegment();
                }
                reservedSegmentCount = 1;
            }
            else
            {
                // We need multiple segments
                // Try to get the series at the preferred segment first.
                if (preferredFirstSegmentIndex >=0)
                {
                    (reservedSegmentCount, segmentsZeroed) = TryReserveSegments(segmentsNeeded, preferredFirstSegmentIndex);
                    if (reservedSegmentCount > 0)
                    {
                        // We got the preferred segment, so we know that the first
                        // segment index is the preferred segment.
                        reservedSegmentIndex = preferredFirstSegmentIndex;
                        segmentIsPreferred = true;
                    }
                }
                // If we didn't get segments from the preferred segment, then try to get any series of segments.
                if (reservedSegmentIndex < 0)
                {
                    (reservedSegmentIndex, reservedSegmentCount, segmentsZeroed) = ReserveSegments(segmentsNeeded);
                }
            }
        }
        finally
        {
            Volatile.Write(ref _locked, 0);
        }
        Debug.Assert(reservedSegmentIndex >= 0, "reservedSegmentIndex < 0");
        Debug.Assert(reservedSegmentCount > 0, "reservedSegmentCount <= 0");
        // Track where the buffer is from, so we can release it later.
        SegmentBufferInfo bufferInfo = new(Id, reservedSegmentIndex, reservedSegmentCount, bufferPool);
        if (_useNativeMemory)
        {
            MemorySegment memorySegment = new(_bufferBlockPtr, (reservedSegmentIndex * StandardBufferSegmentSize),
                reservedSegmentCount * StandardBufferSegmentSize);
            if (requireZeroed && (!segmentsZeroed))
            {
                memorySegment.Clear();
            }
            return (new SegmentBuffer(memorySegment, bufferInfo), GetBufferResult.Available, segmentIsPreferred);
        }
        MemorySegment arraySegment = new(_bufferBlock!, (reservedSegmentIndex * StandardBufferSegmentSize),
            reservedSegmentCount * StandardBufferSegmentSize);
        if (requireZeroed && (!segmentsZeroed))
        {
            arraySegment.Clear();
        }
        return (new SegmentBuffer(arraySegment, bufferInfo), GetBufferResult.Available, segmentIsPreferred);
    }

    /// <summary>
    /// Attempts to get a buffer from the group.
    /// </summary>
    /// <param name="bufferSize">
    /// The size of the buffer that we need.
    /// </param>
    /// <param name="requireZeroed">
    /// Indicates if we need to force the buffer to be zeroed.
    /// </param>
    /// <param name="bufferPool">
    /// The buffer pool that is requesting the buffer.
    /// </param>
    /// <returns>
    /// The buffer to use from this group if available, and a result value for the operation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (SegmentBuffer Buffer, GetBufferResult Result)
        GetBuffer (long bufferSize, bool requireZeroed, MemorySegmentedBufferPool bufferPool)
    {
        (SegmentBuffer buffer, GetBufferResult result, bool _) = GetBuffer(bufferSize, requireZeroed, bufferPool, -1);
        return (buffer, result);
    }

    /// <summary>
    /// The ID for this group instance.
    /// </summary>
    public int Id { [DebuggerStepThrough] get; } = Interlocked.Increment(ref _lastGroupId);

    /// <summary>
    /// The number of segments that we have in this block that can be used for buffers.
    /// </summary>
    public int SegmentCount
    {
        [DebuggerStepThrough]
        get => _segmentCount;
    }
}
//################################################################################
