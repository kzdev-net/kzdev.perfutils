// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// A pairing of a <see cref="SegmentBuffer"/> with virtual stream space metadata 
    /// about the end of the stream space where the segment buffer is located.
    /// </summary>
    /// <param name="segmentBuffer">
    /// The segment buffer information that describes this segment.
    /// </param>
    /// <param name="segmentEndCount">
    /// The virtual end position of this segment in the segment list.
    /// </param>
    /// <param name="segmentEndOffset">
    /// The number of total standard segments through this segment in the segment list.
    /// </param>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
    internal readonly struct SegmentBufferVirtualInfo (in SegmentBuffer segmentBuffer, long segmentEndOffset, int segmentEndCount)
    {
        /// <summary>
        /// Helper to create a new segment buffer virtual info from the previous segment buffer virtual info and the new segment buffer.
        /// </summary>
        /// <param name="segmentBuffer">
        /// The segment buffer to create the virtual info for.
        /// </param>
        /// <param name="previous">
        /// The previous segment buffer virtual info that we will use to derive the new segment buffer virtual info.
        /// </param>
        /// <returns>
        /// The new segment buffer virtual info that should follow the previous segment buffer virtual info.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SegmentBufferVirtualInfo FromPrevious (in SegmentBuffer segmentBuffer, in SegmentBufferVirtualInfo previous) => 
            new(segmentBuffer, segmentBuffer.Length + previous.SegmentEndOffset, segmentBuffer.SegmentCount + previous.SegmentEndCount);

        /// <summary>
        /// Debug helper to display the state of the group.
        /// </summary>
#pragma warning disable HAA0601
        private string DebugDisplayValue => $"{SegmentBuffer.DebugDisplayValue}, EndOffset = {SegmentEndOffset}, EndCount = {SegmentEndCount}";
#pragma warning restore HAA0601

        /// <summary>
        /// The segment buffer information that describes this segment.
        /// </summary>
        public SegmentBuffer SegmentBuffer { [DebuggerStepThrough] get; } = segmentBuffer;

        /// <summary>
        /// The virtual end position of this segment in the segment list.
        /// </summary>
        public long SegmentEndOffset { [DebuggerStepThrough] get; } = segmentEndOffset;

        /// <summary>
        /// The number of total standard segments through this segment in the segment list.
        /// </summary>
        public int SegmentEndCount{ [DebuggerStepThrough] get; } = segmentEndCount;
    }
    //################################################################################
}
