// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
        /// constructor used to create a new generation array from an existing array - to contract the arrays available.
        /// </summary>
        /// <param name="sourceArray">
        /// The source array to copy existing active groups from.
        /// </param>
        private MemorySegmentedGroupGenerationArray (MemorySegmentedGroupGenerationArray sourceArray)
        {
            Debug.Assert(sourceArray is not null, $"Passed {nameof(sourceArray)} is null!");
#if CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
            _useNativeMemory = sourceArray._useNativeMemory;
            _groupLock = new object();
            GenerationId = Guid.NewGuid();
#else
            GenerationId = Interlocked.Increment(ref _lastGeneration);
#endif

            // We are going to copy the non-released groups from the source array to the new array.
            // Note, the set of groups that are released can actually grow while we are working through
            // the list (but it will never shrink). So...we may have to do this copy more than once
            // to get the proper array size. Also note that the first group is never released, so we don't
            // need to check it.
            MemorySegmentedBufferGroup[] sourceArrayGroups = sourceArray.Groups;
            int sourceArrayGroupCount = sourceArrayGroups.Length;
            while (true)
            {
                // Count the released groups
                int releasedGroupCount = 0;
                for (int groupIndex = 1; groupIndex < sourceArrayGroupCount; groupIndex++)
                {
                    if (sourceArrayGroups[groupIndex].IsReleased)
                        releasedGroupCount++;
                }
#if CONCURRENCY_TESTING 
                // Allocate the proper array size for the groups that we'll keep
                _bufferGroups = new MemorySegmentedBufferGroup[sourceArrayGroupCount - releasedGroupCount];
#else
                // Allocate the proper array size for the groups that we'll keep
                Groups = new MemorySegmentedBufferGroup[sourceArrayGroupCount - releasedGroupCount];
#endif
                // Note - we should never have this called if there isn't at least one released group
                Debug.Assert(releasedGroupCount > 0, "There are no released groups to remove from the array!");
                // Copy the groups from the source array.
                int copyGroupCount = 1;
                Groups[0] = sourceArrayGroups[0];  // The first group is never released.
                for (int sourceGroupIndex = 1; sourceGroupIndex < sourceArrayGroupCount; sourceGroupIndex++)
                {
                    MemorySegmentedBufferGroup group = sourceArrayGroups[sourceGroupIndex];
                    if (group.IsReleased)
                        continue;
                    Groups[copyGroupCount++] = group;
                }

                if (copyGroupCount != (sourceArrayGroupCount - releasedGroupCount))
                {
                    // We unfortunately need to allocate another group array, and try this again; because it's 
                    // likely that an additional group was released while we were copying the groups or while
                    // we were counting the groups to copy.
                    continue;
                }

                Debug.Assert(copyGroupCount == Groups.Length, "The copied group count is not correct!");
                return;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegmentedGroupGenerationArray"/> class. This is the
        /// constructor used to create a new generation array from an existing array - to basically
        /// expand the arrays available.
        /// </summary>
        /// <param name="sourceArray">
        /// The source array to copy existing groups from.
        /// </param>
        /// <param name="neededBufferSize">
        /// The size that is needed for the buffer that we are trying to rent. We use this
        /// to minimize the number of array generations that we create if we know that the
        /// size of the generation array we would otherwise be creating is going to be too
        /// small for our immediate needs.
        /// </param>
        /// <param name="useNativeMemory">
        /// Indicates if native memory should be used for the buffers.
        /// </param>
        public MemorySegmentedGroupGenerationArray (MemorySegmentedGroupGenerationArray sourceArray,
            int neededBufferSize, bool useNativeMemory)
        {
            Debug.Assert(sourceArray is not null, $"Passed {nameof(sourceArray)} is null!");
#if CONCURRENCY_TESTING  // For the concurrency testing builds, we use GUID IDs - because using Interlocked operations in static construction makes Concura deadlock
            _useNativeMemory = sourceArray._useNativeMemory;
            _groupLock = new object();
            GenerationId = Guid.NewGuid();
#else
            GenerationId = Interlocked.Increment(ref _lastGeneration);
#endif

            // We are going to copy the non-released groups from the source array to the new array.
            // Note, the set of groups that are released can actually grow while we are working through
            // the list (but it will never shrink). So...we may have to do this copy more than once
            // to get the proper array size. Also note that the first group is never released, so we don't
            // need to check it.
            MemorySegmentedBufferGroup[] sourceArrayGroups = sourceArray.Groups;
            int sourceArrayGroupCount = sourceArrayGroups.Length;
            while (true)
            {
                // Count the released groups
                int releasedGroupCount = 0;
                // The first group should never be released.
                for (int groupIndex = 1; groupIndex < sourceArrayGroupCount; groupIndex++)
                {
                    if (sourceArrayGroups[groupIndex].IsReleased)
                        releasedGroupCount++;
                }
#if CONCURRENCY_TESTING 
                // Allow for one more group than the source array.
                _bufferGroups = new MemorySegmentedBufferGroup[sourceArrayGroupCount + 1 - releasedGroupCount];
#else
                // Allow for one more group than the source array.
                Groups = new MemorySegmentedBufferGroup[sourceArrayGroupCount + 1 - releasedGroupCount];
#endif
                // Copy the groups from the source array.
                if (releasedGroupCount == 0)
                    Array.Copy(sourceArrayGroups, Groups, sourceArrayGroupCount);
                else
                {
                    // Copy the groups from the source array.
                    int copyGroupCount = 1;
                    Groups[0] = sourceArrayGroups[0];  // The first group is never released.
                    for (int sourceGroupIndex = 1; sourceGroupIndex < sourceArrayGroupCount; sourceGroupIndex++)
                    {
                        MemorySegmentedBufferGroup group = sourceArrayGroups[sourceGroupIndex];
                        if (group.IsReleased)
                            continue;
                        Groups[copyGroupCount++] = group;
                    }

                    if (copyGroupCount != (sourceArrayGroupCount - releasedGroupCount))
                    {
                        // We unfortunately need to allocate another group array, and try this again; because it's 
                        // likely that an additional group was released while we were copying the groups or while
                        // we were counting the groups to copy.
                        continue;
                    }

                    Debug.Assert(copyGroupCount == (Groups.Length - 1), "The copied group count is not correct!");
                }
                break;
            }
            // Create a new group for the last group. We will double the segment count of the last group
            // to a point, then we will simply add 32 segments to the last group size.
            int lastGroupSegmentCount = Groups[^2].SegmentCount;
            int nextGroupShiftSegmentCount = Math.Max(lastGroupSegmentCount << 1, (neededBufferSize / MemorySegmentedBufferGroup.StandardBufferSegmentSize));
            int nextGroupLinearSegmentCount = lastGroupSegmentCount + 32;
            int nextGroupSegmentCount = Math.Min(MaxAllowedGroupSegmentCount, Math.Min(nextGroupLinearSegmentCount, nextGroupShiftSegmentCount));
            Groups[^1] = new MemorySegmentedBufferGroup(MaxGroupSegmentCount = nextGroupSegmentCount, useNativeMemory);
        }

        /// <summary>
        /// Contracts the array by removing any released groups.
        /// </summary>
        /// <returns>
        /// A new array generation with the released groups removed.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemorySegmentedGroupGenerationArray ContractArray () => new(this);

        /// <summary>
        /// Checks the groups in the array to see if any of them can be trimmed. If this is true,
        /// then at least one group has been marked for release, so a new generation array should
        /// be created.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if at least one group has been marked for release; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TrimGroups ()
        {
            // For the very first (smallest and initial) group, we check if we can release
            // the associated memory, btu we don't fully release the group.
            Groups[0].ReleaseUnusedMemory();
            // For the rest of the groups, we check if we can release the group entirely
            bool anyGroupsTrimmed = false;
            for (int groupIndex = 1; groupIndex < Groups.Length; groupIndex++)
            {
                MemorySegmentedBufferGroup group = Groups[groupIndex];
                if (group.ReleaseGroup())
                    anyGroupsTrimmed = true;
            }
            return anyGroupsTrimmed;
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
