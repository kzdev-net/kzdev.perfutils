// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// Holds an array of buffer groups with a generation number that are accessed as
    /// a unit.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
    internal class MemorySegmentedGroupGenerationArray
    {
        /// <summary>
        /// The number of segments for the initial group.
        /// </summary>
#if NOT_PACKAGING
        internal
#else
        private
#endif
        const int InitialSegmentCount = 4;

        /// <summary>
        /// The maximum number of segments for a group that will be added to the array when it grows.
        /// </summary>
#if NOT_PACKAGING
        internal
#else
        private
#endif
        const int MaxAllowedGroupSegmentCount = 512;

#if !CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
        /// <summary>
        /// The last generation number used.
        /// </summary>
        private static int _lastGeneration;
#endif

        /// <summary>
        /// The array of buffer groups.
        /// </summary>
#if CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
        // For testing builds, we will have to JIT the initial groups.
        private MemorySegmentedBufferGroup[]? _bufferGroups = null;
        private readonly object _groupLock;
        private readonly bool _useNativeMemory;
        public MemorySegmentedBufferGroup[] Groups
        {
            [DebuggerStepThrough]
            get
            {
                MemorySegmentedBufferGroup[]? returnGroups = Volatile.Read(ref _bufferGroups);
                if (returnGroups is not null)
                    return returnGroups;
                lock (_groupLock)
                {
                    if (_bufferGroups is not null)
                        return _bufferGroups;
                    _bufferGroups = [new MemorySegmentedBufferGroup(InitialSegmentCount, _useNativeMemory)];
                }
                return _bufferGroups;
            }
        }
#else
        public MemorySegmentedBufferGroup[] Groups { [DebuggerStepThrough] get; }
#endif

        /// <summary>
        /// The generation ID of this array.
        /// </summary>
#if CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
        public Guid GenerationId { [DebuggerStepThrough] get; }
#else
        public int GenerationId { [DebuggerStepThrough] get; }
#endif

        /// <summary>
        /// Gets the number of segments in the largest group.
        /// </summary>
        public int MaxGroupSegmentCount { [DebuggerStepThrough] get; } = InitialSegmentCount;  // We initialize to the initial segment count.

        /// <summary>
        /// Debug helper to display the state of the group.
        /// </summary>
        [ExcludeFromCodeCoverage]
        internal string DebugDisplayValue => $@"ID {GenerationId}, Groups = {Groups.Length}";

        /// <summary>
        /// Gets the initial array for the segmented memory.
        /// </summary>
        /// <returns>
        /// Returns the initial array for the segmented memory.
        /// </returns>
        public static MemorySegmentedGroupGenerationArray GetInitialArray (bool useNativeMemory) => new(useNativeMemory);

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegmentedGroupGenerationArray"/> class.
        /// </summary>
        /// <param name="useNativeMemory">
        /// Indicates if native memory should be used for the buffers.
        /// </param>
        private MemorySegmentedGroupGenerationArray (bool useNativeMemory)
        {
#if CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
            _useNativeMemory = useNativeMemory;
            _groupLock = new object();
            GenerationId = Guid.NewGuid();
            // For testing builds, we will have to JIT get the initial groups.
#else
            GenerationId = Interlocked.Increment(ref _lastGeneration);
            Groups = [new MemorySegmentedBufferGroup(InitialSegmentCount, useNativeMemory)];
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegmentedGroupGenerationArray"/> class. This is the
        /// constructor used to create a new generation array from an existing array - to basically
        /// expand the arrays available.
        /// </summary>
        /// <param name="sourceArray">
        /// The source array to copy existing groups from.
        /// </param>
        /// <param name="useNativeMemory">
        /// Indicates if native memory should be used for the buffers.
        /// </param>
        public MemorySegmentedGroupGenerationArray (MemorySegmentedGroupGenerationArray sourceArray, bool useNativeMemory)
        {
            Debug.Assert(sourceArray is not null, $"Passed {nameof(sourceArray)} is null!");
#if CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
            _useNativeMemory = useNativeMemory;
            _groupLock = new object();
            GenerationId = Guid.NewGuid();
            // Allow for one more group than the source array.
            _bufferGroups = new MemorySegmentedBufferGroup[sourceArray.Groups.Length + 1];
#else
            GenerationId = Interlocked.Increment(ref _lastGeneration);
            // Allow for one more group than the source array.
            Groups = new MemorySegmentedBufferGroup[sourceArray.Groups.Length + 1];
#endif
            // Copy the groups from the source array.
            Array.Copy(sourceArray.Groups, Groups, sourceArray.Groups.Length);
            // Create a new group for the last group. We will double the segment count of the last group
            // to a point, then we will simply add 32 segments to the last group size.
            int lastGroupSegmentCount = sourceArray.Groups[^1].SegmentCount;
            int nextGroupShiftSegmentCount = lastGroupSegmentCount << 1;
            int nextGroupLinearSegmentCount = lastGroupSegmentCount + 32;
            int nextGroupSegmentCount = Math.Min(MaxAllowedGroupSegmentCount, Math.Min(nextGroupLinearSegmentCount, nextGroupShiftSegmentCount));
            Groups[sourceArray.Groups.Length] = new MemorySegmentedBufferGroup(MaxGroupSegmentCount = nextGroupSegmentCount, useNativeMemory);
        }

        /// <summary>
        /// Release the buffer back into its group.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to be released.
        /// </param>
        /// <param name="segmentIsZeroed">
        /// Indicates if the memory space associated with the segment has been zeroed.
        /// </param>
        public bool ReleaseBuffer (in SegmentBuffer buffer, bool segmentIsZeroed)
        {
            for (int groupIndex = 0; groupIndex < Groups.Length; groupIndex++)
            {
                MemorySegmentedBufferGroup group = Groups[groupIndex];
                if (group.Id != buffer.BufferInfo.BlockId)
                    continue;
                group.ReleaseBuffer(buffer, segmentIsZeroed);
                return true;
            }
            // We may return false if this generation is being referenced but there 
            // has been a newer generation created, but who ever is calling this, hasn't 
            // yet gotten the new reference to that generation.
            return false;
        }
    }
    //################################################################################
}
