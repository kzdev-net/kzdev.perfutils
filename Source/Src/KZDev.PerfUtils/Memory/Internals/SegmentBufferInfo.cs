﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// Contains information about a specific <see cref="SegmentBuffer"/> instance.
    /// </summary>
    /// <param name="blockId">
    /// The identifier of the block that this segment references.
    /// </param>
    /// <param name="segmentId">
    /// The identifier of the block segment that this segment references.
    /// </param>
    /// <param name="segmentCount">
    /// The number of segments that the buffer holds. If the buffer is 
    /// a raw buffer, then this will return 0;
    /// </param>
    /// <param name="bufferPool">
    /// The buffer array pool that this segment was rented from, if any.
    /// </param>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
    internal readonly struct SegmentBufferInfo(int blockId, int segmentId, int segmentCount, MemorySegmentedBufferPool? bufferPool)
    {
        /// <summary>
        /// Debug helper to display the state of the group.
        /// </summary>
        internal string DebugDisplayValue => (BlockId < 0) ? "NoBlock" : $"Block {BlockId}, Segment {SegmentId}, Count {SegmentCount}";

        /// <summary>
        /// A special value that indicates that this segment does not reference a block. 
        /// This is a 'default' value.
        /// </summary>
        public static SegmentBufferInfo NoBlock { [DebuggerStepThrough] get; } = new(-1, -1, 0, null);

        /// <summary>
        /// Identifies the block that this segment references.
        /// </summary>
        public int BlockId { [DebuggerStepThrough] get; } = blockId;

        /// <summary>
        /// Identifies the block segment index that this segment references.
        /// </summary>
        public int SegmentId { [DebuggerStepThrough] get; } = segmentId;

        /// <summary>
        /// The number of standard sized segments that this buffer holds. If this is a 
        /// raw buffer, then this will return 0;
        /// </summary>
        public int SegmentCount { [DebuggerStepThrough] get; } = segmentCount;

        /// <summary>
        /// The buffer array pool that this segment was rented from, if any.
        /// </summary>
        public MemorySegmentedBufferPool? BufferPool { [DebuggerStepThrough] get; } = bufferPool;
    }
    //################################################################################
}