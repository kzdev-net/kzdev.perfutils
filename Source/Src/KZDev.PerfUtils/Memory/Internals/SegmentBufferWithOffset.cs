// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// A pairing of a <see cref="SegmentBuffer"/> with virtual stream space metadata 
    /// about the end of the stream space where the segment buffer is located.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
    internal readonly struct SegmentBufferVirtualInfo (in SegmentBuffer segmentBuffer, long segmentEndOffset, int segmentEndCount)
    {
        /// <summary>
        /// Debug helper to display the state of the group.
        /// </summary>
        private string DebugDisplayValue => $"{SegmentBuffer.DebugDisplayValue}, EndOffset = {SegmentEndOffset}, EndCount = {SegmentEndCount}";

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
