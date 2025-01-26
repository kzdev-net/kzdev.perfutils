// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;

using FluentAssertions;

using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemorySegmentedBufferGroup"/> class where the 
    /// state of the group is set to specific patterns before the tests are run.
    /// </summary>
    public class UsingMemorySegmentedBufferGroupWithUsedPatterns : MemorySegmentedBufferGroupUnitTestBase
    {
        /// <summary>
        /// The size of the block flag set/group (contained in a ulong)
        /// </summary>
        internal const int BlockFlagSetSize = 64;  // 64 bits in a ulong

        /// <summary>
        /// The field information for the _segmentCount field in the buffer group.
        /// </summary>
        private static readonly FieldInfo BufferGroupSegmentCountField =
            typeof(MemorySegmentedBufferGroup)
                .GetField("_segmentCount",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

        /// <summary>
        /// The field information for the _segmentsInUse field in the buffer group.
        /// </summary>
        private static readonly FieldInfo BufferGroupSegmentsInUseField =
            typeof(MemorySegmentedBufferGroup)
                .GetField("_segmentsInUse",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

        /// <summary>
        /// The field information for the _blockUsedFlags field in the buffer group.
        /// </summary>
        private static readonly FieldInfo BlockUsedFlagsField =
            typeof(MemorySegmentedBufferGroup)
                .GetField("_blockUsedFlags",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

        /// <summary>
        /// The field information for the _blockZeroFlags field in the buffer group.
        /// </summary>
        private static readonly FieldInfo BlockZeroFlagsField =
            typeof(MemorySegmentedBufferGroup)
                .GetField("_blockZeroFlags",
                BindingFlags.NonPublic | BindingFlags.Instance)!;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// For a given segment index, returns the index of the ulong in the block flag
        /// array as well as the mask to use to set or check the flag for the segment.
        /// </summary>
        /// <param name="segmentIndex">
        /// The segment index number to get the flag index and mask for.
        /// </param>
        /// <returns>
        /// The index of the ulong in the block flag array and the mask to use for the
        /// </returns>
        private static (int Index, ulong Mask) GetFlagIndexAndMask (int segmentIndex)
        {
            int index = Math.DivRem(segmentIndex, BlockFlagSetSize, out int offset);
            return (index, 1UL << offset);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Uses reflection to get the count of segments in the buffer group.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to get the segment count for.
        /// </param>
        /// <returns>
        /// The value of the _segmentCount field in the buffer group.
        /// </returns>
        private static int GetSegmentCount (MemorySegmentedBufferGroup bufferGroup) =>
            (int)BufferGroupSegmentCountField.GetValue(bufferGroup)!;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the count of segments in the buffer group using reflection.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to set the segment count for.
        /// </param>
        /// <param name="segmentCount">
        /// The value of the _segmentCount field to set in the buffer group.
        /// </param>
        private static void SetSegmentCount (MemorySegmentedBufferGroup bufferGroup, int segmentCount)
        {
#pragma warning disable HAA0601
            BufferGroupSegmentCountField.SetValue(bufferGroup, segmentCount);
#pragma warning restore HAA0601
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the number of block flag ulong values in the block flag array that are 
        /// needed to store the block flags for the specified buffer group based on the
        /// number of segments in the group.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to get the block flag array size needed for.
        /// </param>
        /// <returns>
        /// The number of block flag ulong values needed to store the block flags for the
        /// </returns>
        private static int GetBlockFlagArraySizeNeeded (MemorySegmentedBufferGroup bufferGroup)
        {
            int segmentCount = GetSegmentCount(bufferGroup);
            return (segmentCount + BlockFlagSetSize - 1) / BlockFlagSetSize;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the number of segments in the buffer group that are in use.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to get the number of segments in use for.
        /// </param>
        /// <returns>
        /// The number of segments in the buffer group that are in use.
        /// </returns>
        private static int GetSegmentsInUse (MemorySegmentedBufferGroup bufferGroup) =>
            (int)BufferGroupSegmentsInUseField.GetValue(bufferGroup);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the number of segments in the buffer group that are in use.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to set the number of segments in use for.
        /// </param>
        /// <param name="segmentsInUse">
        /// The number of segments in the buffer group that are in use.
        /// </param>
        private static void SetSegmentsInUse (MemorySegmentedBufferGroup bufferGroup, int segmentsInUse)
        {
            BufferGroupSegmentsInUseField.SetValue(bufferGroup, segmentsInUse);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the block used flags for the buffer group to the specified ulong array.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to set the block used flags for.
        /// </param>
        /// <param name="blockUsedFlags">
        /// The ulong array of block used flags to set in the buffer group.
        /// </param>
        /// <param name="blockZeroFlags">
        /// The ulong array of block zero flags to set in the buffer group.
        /// </param>
        private static (int availableSegmentCount, Dictionary<int, List<int>> availableSegmentMap)
            SetBlockUsedFlags (MemorySegmentedBufferGroup bufferGroup, ulong[] blockUsedFlags, ulong[] blockZeroFlags)
        {
            // Get the number of segments in the buffer group
            int segmentFlagsToSet = GetSegmentCount(bufferGroup);
            // Build our own array to be sure we have the right size
            int blockFlagArraySize = GetBlockFlagArraySizeNeeded(bufferGroup);
            ulong[] setBlockUsedFlags = new ulong[blockFlagArraySize];
            ulong[] setBlockZeroFlags = new ulong[blockFlagArraySize];
            // Track the number of segments marked in use
            int segmentsAvailable = segmentFlagsToSet;
            int segmentsInUse = 0;
            int blockFlagIndex = 0;
            ulong blockFlagMask = 1;
            Dictionary<int, List<int>> availableSegmentMap = new();
            // Track the running list of available segments
            int? availableSegmentRunStart = null;
            int availableSegmentRunCount = 0;

            // Loop through the requested flags
            for (int segmentIndex = 0; segmentIndex < segmentFlagsToSet; segmentIndex++)
            {
                // Set the block used flag
                if ((blockFlagIndex < blockUsedFlags.Length)  &&
                    ((blockUsedFlags[blockFlagIndex] & blockFlagMask) == blockFlagMask))
                {
                    setBlockUsedFlags[blockFlagIndex] |= blockFlagMask;
                    segmentsInUse++;
                    segmentsAvailable--;
                    // Store and reset the available segment run tracking
                    if (availableSegmentRunCount > 0)
                    {
                        UpdateAvailableSegmentMap(availableSegmentMap, ref availableSegmentRunStart, ref availableSegmentRunCount);
                    }
                }
                else
                {
                    // Update the available segment run tracking
                    availableSegmentRunStart??= segmentIndex;
                    availableSegmentRunCount++;
                }

                // Set the block zero flag
                if ((blockFlagIndex < blockZeroFlags.Length)  &&
                    ((blockZeroFlags[blockFlagIndex] & blockFlagMask) == blockFlagMask))
                {
                    setBlockZeroFlags[blockFlagIndex] |= blockFlagMask;
                }

                // Move to the next block flag
                blockFlagMask <<= 1;
                if (blockFlagMask != 0)
                {
                    continue;
                }

                blockFlagMask = 1;
                blockFlagIndex++;
            }
            // Do a last update of the available segment map
            UpdateAvailableSegmentMap(availableSegmentMap, ref availableSegmentRunStart, ref availableSegmentRunCount);

            BlockUsedFlagsField.SetValue(bufferGroup, setBlockUsedFlags);
            BlockZeroFlagsField.SetValue(bufferGroup, setBlockZeroFlags);
            SetSegmentsInUse(bufferGroup, segmentsInUse);
            return (segmentsAvailable, availableSegmentMap);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the block used flags for the buffer group to the specified boolean array,
        /// converting the array to a ulong array for storage in the buffer group.
        /// </summary>
        /// <param name="bufferGroup">
        /// The buffer group to set the block used flags for.
        /// </param>
        /// <param name="blockUsedFlags">
        /// The boolean array of block used flags to set in the buffer group.
        /// </param>
        /// <param name="blockZeroFlags">
        /// The boolean array of block zero flags to set in the buffer group.
        /// </param>
        private static (int availableSegmentCount, Dictionary<int, List<int>> availableSegmentMap)
            SetBlockUsedFlags (MemorySegmentedBufferGroup bufferGroup, bool[] blockUsedFlags, bool[] blockZeroFlags)
        {
            // Get the number of segments in the buffer group
            int segmentFlagsToSet = GetSegmentCount(bufferGroup);
            // Build our own array to be sure we have the right size
            int blockFlagArraySize = GetBlockFlagArraySizeNeeded(bufferGroup);
            ulong[] setBlockUsedFlags = new ulong[blockFlagArraySize];
            ulong[] setBlockZeroFlags = new ulong[blockFlagArraySize];

            // Set the proper flags in the ulong arrays
            for (int segmentIndex = 0; segmentIndex < segmentFlagsToSet; segmentIndex++)
            {
                int flagIndex = Math.DivRem(segmentIndex, BlockFlagSetSize, out int flagOffset);
                ulong flagMask = 1UL << flagOffset;

                if ((segmentIndex < blockUsedFlags.Length) && blockUsedFlags[segmentIndex])
                {
                    setBlockUsedFlags[flagIndex] |= flagMask;
                }

                if ((segmentIndex < blockZeroFlags.Length) && blockZeroFlags[segmentIndex])
                {
                    setBlockZeroFlags[flagIndex] |= flagMask;
                }
            }
            return SetBlockUsedFlags(bufferGroup, setBlockUsedFlags, setBlockZeroFlags);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper to update the available segment map with the current run of available segments.
        /// </summary>
        /// <param name="availableSegmentMap">
        /// The map of available segment runs to update. This is keyed by the count of segments in 
        /// a consecutive segment run, and the value is a list of the starting indexes of the runs.
        /// </param>
        /// <param name="availableSegmentRunStart">
        /// The start index of the current run of available segments (if any).
        /// </param>
        /// <param name="availableSegmentRunCount">
        /// The current count of available segments in the run.
        /// </param>
        private static void UpdateAvailableSegmentMap (Dictionary<int, List<int>> availableSegmentMap,
            ref int? availableSegmentRunStart, ref int availableSegmentRunCount)
        {
            if (availableSegmentRunCount <= 0)
            {
                availableSegmentRunStart = null;
                availableSegmentRunCount = 0;
                return;
            }

            if (availableSegmentMap.TryGetValue(availableSegmentRunCount, out List<int>? segmentList))
            {
                segmentList.Add(availableSegmentRunStart!.Value);
            }
            else
            {
                availableSegmentMap[availableSegmentRunCount] = [availableSegmentRunStart!.Value];
            }
            availableSegmentRunStart = null;
            availableSegmentRunCount = 0;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the in use flags for the segments in the buffer group to the specified 
        /// repeating pattern based on the number of segments in the group by starting with
        /// setting the in use segments and then the available segments.
        /// </summary>
        /// <param name="skipCount">
        /// The number of segments to skip before starting the pattern.
        /// </param>
        /// <param name="bufferGroup">
        /// The buffer group to set the in use flags for.
        /// </param>
        /// <param name="pattern">
        /// The repeating pattern of in use and available segments to set in the buffer group.
        /// </param>
        /// <returns>
        /// The number of segments available for use in the buffer group.
        /// </returns>>
        private static (int availableSegmentCount, Dictionary<int, List<int>> availableSegmentMap)
            SetSegmentInUsePattern (MemorySegmentedBufferGroup bufferGroup,
                int skipCount, params (int inUseCount, int availableCount, bool availableAreZeroed)[] pattern)
        {
            // Get the number of segments in the buffer group
            int segmentCount = GetSegmentCount(bufferGroup);
            // The segment in use flags and zeroed to set
            bool[] segmentInUseFlags = new bool[segmentCount];
            bool[] segmentZeroFlags = new bool[segmentCount];
            // Track the number of segments marked as available
            int patternIndex = 0;
            // Get the values for the first pattern
            (int inUseCount, int availableCount, bool availableAreZeroed) = pattern[patternIndex];

            // Loop through the segments in the buffer group
            for (int segmentIndex = Math.Max(0, skipCount); segmentIndex < segmentCount; segmentIndex++)
            {
                // Set the in use flags for the segment if we are still in the "in-use" part of the pattern
                if (inUseCount > 0)
                {
                    segmentInUseFlags[segmentIndex] = true;
                    inUseCount--;
                    // Store and reset the available segment run tracking
                    continue;
                }
                if (availableCount > 0)
                {
                    // Set the zero flag for the segment if needed
                    segmentZeroFlags[segmentIndex] = availableAreZeroed;
                    availableCount--;
                    continue;
                }
                // We have exhausted the current pattern, move to the next one
                patternIndex = (patternIndex + 1) % pattern.Length;
                (inUseCount, availableCount, availableAreZeroed) = pattern[patternIndex];
                // But we also have to back up the segment index
                segmentIndex--;
            }

            // Set the flags in the buffer group
            return SetBlockUsedFlags(bufferGroup, segmentInUseFlags, segmentZeroFlags);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the in use flags for the segments in the buffer group to the specified 
        /// repeating pattern based on the number of segments in the group, but starts with
        /// skipping the available segments and then setting the in use segments.
        /// </summary>
        /// <param name="skipCount">
        /// The number of segments to skip before starting the pattern.
        /// </param>
        /// <param name="bufferGroup">
        /// The buffer group to set the in use flags for.
        /// </param>
        /// <param name="pattern">
        /// The repeating pattern of in use and available segments to set in the buffer group.
        /// </param>
        /// <returns>
        /// The number of segments available for use in the buffer group.
        /// </returns>>
        private static (int availableSegmentCount, Dictionary<int, List<int>> availableSegmentMap)
            SetSegmentAvailablePattern (MemorySegmentedBufferGroup bufferGroup,
                int skipCount, params (int availableCount, int inUseCount, bool availableAreZeroed)[] pattern)
        {
            // Get the number of segments in the buffer group
            int segmentCount = GetSegmentCount(bufferGroup);
            // The segment in use flags and zeroed to set
            bool[] segmentInUseFlags = new bool[segmentCount];
            bool[] segmentZeroFlags = new bool[segmentCount];
            // Track the number of segments marked as available
            int segmentsAvailable = segmentCount;
            int patternIndex = 0;
            Dictionary<int, List<int>> availableSegmentMap = new();
            // Track the running list of available segments
            int? availableSegmentRunStart = null;
            int availableSegmentRunCount = 0;
            // If we have a skip count, we need to track the available segments
            if (skipCount > 0)
            {
                availableSegmentRunStart = 0;
                availableSegmentRunCount = skipCount;
            }
            // Get the values for the first pattern
            (int availableCount, int inUseCount, bool availableAreZeroed) = pattern[patternIndex];

            // Loop through the segments in the buffer group
            for (int segmentIndex = Math.Max(0, skipCount); segmentIndex < segmentCount; segmentIndex++)
            {
                // Skip the available segments if we are still in the "available" part of the pattern
                if (availableCount > 0)
                {
                    // Update the available segment run tracking
                    availableSegmentRunStart??= segmentIndex;
                    availableSegmentRunCount++;
                    // Set the zero flag for the segment if needed
                    segmentZeroFlags[segmentIndex] = availableAreZeroed;
                    availableCount--;
                    continue;
                }
                if (inUseCount > 0)
                {
                    segmentInUseFlags[segmentIndex] = true;
                    segmentsAvailable--;
                    inUseCount--;
                    // Store and reset the available segment run tracking
                    if (availableSegmentRunCount > 0)
                    {
                        UpdateAvailableSegmentMap(availableSegmentMap, ref availableSegmentRunStart, ref availableSegmentRunCount);
                    }
                    continue;
                }
                // We have exhausted the current pattern, move to the next one
                patternIndex = (patternIndex + 1) % pattern.Length;
                (availableCount, inUseCount, availableAreZeroed) = pattern[patternIndex];
                // But we also have to back up the segment index
                segmentIndex--;
            }
            // Do a last update of the available segment map
            UpdateAvailableSegmentMap(availableSegmentMap, ref availableSegmentRunStart, ref availableSegmentRunCount);

            // Set the flags in the buffer group
            return SetBlockUsedFlags(bufferGroup, segmentInUseFlags, segmentZeroFlags);
            return (segmentsAvailable, availableSegmentMap);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemorySegmentedBufferGroupWithUsedPatterns"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemorySegmentedBufferGroupWithUsedPatterns (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get a single segment zeroed buffer when the current used pattern is 
        /// every other segment is in use (1-0) starting with the first segment.
        /// This should return a buffer from the second segment in the group that is zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetSingleBuffer_WithAlternating_1_0_UsedSegments_GetsProperBuffer ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool();
            SetSegmentInUsePattern(sut, skipCount: 0, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            // We'll always request a zeroed buffer for these tests
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);

            // We should get the second segment in the group
            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestBufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.IsAllZeroes().Should().BeTrue();
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(1);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get a single segment zeroed buffer when the current used pattern is 
        /// every other segment is in use (1-0) starting with the first segment.
        /// This should return a buffer from the second segment in the group that is zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_LargeGroup_GetSingleBuffer_WithAlternating_1_0_UsedSegments_GetsProperBuffer ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            SetSegmentInUsePattern(sut, skipCount: 0, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            // We'll always request a zeroed buffer for these tests
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);

            // We should get the second segment in the group
            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestBufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.IsAllZeroes().Should().BeTrue();
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(1);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get a single segment zeroed buffer when the current used pattern is 
        /// every other segment is in use (1-0) starting with the second segment.
        /// This should return a buffer from the first segment in the group that is zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetSingleBuffer_WithAlternating_1_0_Skip1_UsedSegments_GetsProperBuffer ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool();
            SetSegmentInUsePattern(sut, skipCount: 1, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            // We'll always request a zeroed buffer for these tests
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);

            // We should get the second segment in the group
            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestBufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.IsAllZeroes().Should().BeTrue();
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get a single segment zeroed buffer when the current used pattern is 
        /// every other segment is in use (1-0) starting with the second segment.
        /// This should return a buffer from the first segment in the group that is zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_LargeGroup_GetSingleBuffer_WithAlternating_1_0_Skip1_UsedSegments_GetsProperBuffer ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            SetSegmentInUsePattern(sut, skipCount: 1, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            // We'll always request a zeroed buffer for these tests
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);

            // We should get the second segment in the group
            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestBufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.IsAllZeroes().Should().BeTrue();
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        #region Get Any Remaining Buffers Tests

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (1-0) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithAlternating_1_0_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool();
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 0, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (1-0) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_LargeGroup_GetRemainingBuffers_WithAlternating_1_0_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 0, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (0-1) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithAlternating_1_0_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool();
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 1, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 0;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (0-1) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_LargeGroup_GetRemainingBuffers_WithAlternating_1_0_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 1, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 0;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (1-0-1-1-0) starting with the first segment.
        /// This should return a series of repeating single segment buffers from the second and fifth segments in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithAlternating_1_0_1_1_0_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: 0,
                (inUseCount: 1, availableCount: 1, availableAreZeroed: false), (inUseCount: 2, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 1;
            // We skip 2 first because the first segment is in use
            int usedSegmentSkip = 2;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(1);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                expectedSegmentId += 1 + usedSegmentSkip;
                usedSegmentSkip = (usedSegmentSkip == 2) ? 1 : 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (1-0-1-1-0) starting with the second segment.
        /// This should return a series of repeating single segment buffers from the second and fifth segments in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithAlternating_1_0_1_1_0_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: 1,
                (inUseCount: 1, availableCount: 1, availableAreZeroed: false), (inUseCount: 2, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 0;
            // We skip 1 first because the first segment is available
            int usedSegmentSkip = 1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(1);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                expectedSegmentId += 1 + usedSegmentSkip;
                usedSegmentSkip = (usedSegmentSkip == 2) ? 1 : 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (0-1-0-0-1) starting with the first segment.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithAlternating_0_1_0_0_1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: 0,
                (availableCount: 1, inUseCount: 1, availableAreZeroed: false), (availableCount: 2, inUseCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
            IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                .OrderByDescending(kvp => kvp.Key)
                .GetEnumerator();
            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

            // The index of the current segment size offset in the available segment map
            int availableSegmentSizeOffsetIndex = 0;

            // Try to get all the available segments
            while (availableSegments > 0)
            {
                // Check if we need to move to the next segment size
                while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                {
                    availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                    availableSegmentSizeOffsetIndex = 0;
                }
                int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(expectedSegmentCount);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                availableSegments -= buffer.SegmentCount;
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (0-1-0-0-1) starting with the second segment.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithAlternating_0_1_0_0_1_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: 1,
                (availableCount: 1, inUseCount: 1, availableAreZeroed: false), (availableCount: 2, inUseCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
            IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                .OrderByDescending(kvp => kvp.Key)
                .GetEnumerator();
            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

            // The index of the current segment size offset in the available segment map
            int availableSegmentSizeOffsetIndex = 0;

            // Try to get all the available segments
            while (availableSegments > 0)
            {
                // Check if we need to move to the next segment size
                while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                {
                    availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                    availableSegmentSizeOffsetIndex = 0;
                }
                int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(expectedSegmentCount);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                availableSegments -= buffer.SegmentCount;
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is randomly generated.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact(Explicit = true)]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithRandomUsedPatterns_GetsProperBuffers ()
        {
            for (int testLoop = 0; testLoop < 200; testLoop++)
            {
                (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 5);
                int useSkipCount = GetTestInteger(0, 7);
                int patternCount = GetTestInteger(1, 5);
                bool useAvailablePattern = RandomSource.GetRandomBoolean();
                TestWriteLine($"Test Loop: {testLoop} - Skip: {useSkipCount}, {(useAvailablePattern ? "Available" : "Used")} Patterns: {patternCount}");

                int availableSegments;
                Dictionary<int, List<int>> availableSegmentMap;
                if (useAvailablePattern)
                {
                    (int availableCount, int inUseCount, bool availableAreZeroed)[] pattern = new (int, int, bool)[patternCount];
                    for (int patternIndex = 0; patternIndex < patternCount; patternIndex++)
                    {
                        pattern[patternIndex] = (GetTestInteger(1, 12), GetTestInteger(1, 12), false);
                        TestWriteLine($"    Available: {pattern[patternIndex].availableCount}, Used: {pattern[patternIndex].inUseCount}");
                    }
                    (availableSegments, availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: useSkipCount, pattern);
                }
                else
                {
                    (int inUseCount, int availableCount, bool availableAreZeroed)[] pattern = new (int, int, bool)[patternCount];
                    for (int patternIndex = 0; patternIndex < patternCount; patternIndex++)
                    {
                        pattern[patternIndex] = (GetTestInteger(1, 12), GetTestInteger(1, 12), false);
                        TestWriteLine($"    Used: {pattern[patternIndex].inUseCount}, Available: {pattern[patternIndex].availableCount}");
                    }
                    (availableSegments, availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: useSkipCount, pattern);
                }

                int expectedUsedSegments = sut.SegmentCount - availableSegments;
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);

                // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
                IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                    .OrderByDescending(kvp => kvp.Key)
                    .GetEnumerator();
                availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

                // The index of the current segment size offset in the available segment map
                int availableSegmentSizeOffsetIndex = 0;

                // Try to get all the available segments
                while (availableSegments > 0)
                {
                    try
                    {
                        // Check if we need to move to the next segment size
                        while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                        {
                            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                            availableSegmentSizeOffsetIndex = 0;
                        }
                        int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                        int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                        int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                        // We'll always request a zeroed buffer for these tests
                        (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                        expectedUsedSegments += buffer.SegmentCount;

                        // We should get the next available segment in the group
                        result.Should().Be(GetBufferResult.Available);
                        // We should only get one segment back
                        buffer.SegmentCount.Should().Be(expectedSegmentCount);
                        buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                        GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                        availableSegments -= buffer.SegmentCount;
                        buffer.IsAllZeroes().Should().BeTrue();
                        buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                        buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);

                    }
                    catch
                    {
                        TestWriteLine($"** Exception thrown with {availableSegments} segments left to get");
                        throw;
                    }
                }

                // An attempt to get another buffer should fail with the group full
                int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
                fullResult.Should().Be(GetBufferResult.GroupFull);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is randomly generated.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact(Explicit = true)]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public void UsingMemorySegmentedBufferGroup_GetRemainingBuffers_WithRandomUsedBlockFlags_GetsProperBuffers ()
        {
            const int BlockFlagSetTestCount = 9;
            for (int testLoop = 0; testLoop < 200; testLoop++)
            {
                (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * BlockFlagSetTestCount);
                ulong[] setUsedBlockFlags = Enumerable.Range(0, BlockFlagSetSize).Select(_ => GetTestUnsignedLongInteger()).ToArray();
                ulong[] setZeroBlockFlags = Enumerable.Repeat(0, BlockFlagSetSize).Select(_ => 0UL).ToArray();

                (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetBlockUsedFlags(sut, setUsedBlockFlags, setZeroBlockFlags);

                TestWriteLine($"Test Loop: {testLoop} - Used Block Flags: {string.Join(", ", setUsedBlockFlags.Select(flag => flag.ToString("X16")))} - Zero Block Flags: {{all cleared}}");

                int expectedUsedSegments = sut.SegmentCount - availableSegments;
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);

                // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
                IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                    .OrderByDescending(kvp => kvp.Key)
                    .GetEnumerator();
                availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

                // The index of the current segment size offset in the available segment map
                int availableSegmentSizeOffsetIndex = 0;

                // Try to get all the available segments
                while (availableSegments > 0)
                {
                    try
                    {
                        // Check if we need to move to the next segment size
                        while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                        {
                            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                            availableSegmentSizeOffsetIndex = 0;
                        }
                        int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                        int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                        int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                        // We'll always request a zeroed buffer for these tests
                        (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, true, bufferPool);
                        expectedUsedSegments += buffer.SegmentCount;

                        // We should get the next available segment in the group
                        result.Should().Be(GetBufferResult.Available);
                        // We should only get one segment back
                        buffer.SegmentCount.Should().Be(expectedSegmentCount);
                        buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                        GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                        availableSegments -= buffer.SegmentCount;
                        buffer.IsAllZeroes().Should().BeTrue();
                        buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                        buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                    }
                    catch
                    {
                        TestWriteLine($"** Exception thrown with {availableSegments} segments left to get");
                        throw;
                    }
                }

                // An attempt to get another buffer should fail with the group full
                int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
                fullResult.Should().Be(GetBufferResult.GroupFull);
            }
        }
        //--------------------------------------------------------------------------------    

        #endregion Get Any Remaining Buffers Tests

        #region Get Preferred Remaining Buffers Tests

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (1-0) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithAlternating_1_0_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool();
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 0, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 1;
            int preferredSegmentId = -1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + 1;
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (1-0) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_LargeGroup_GetPreferredRemainingBuffers_WithAlternating_1_0_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 0, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 1;
            int preferredSegmentId = -1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + 1;
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (0-1) starting with the first segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithAlternating_1_0_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool();
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 1, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 0;
            int preferredSegmentId = -1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + 1;
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// every other segment is in use (0-1) starting with the second segment.
        /// This should return a series of single segment buffers from every other segment in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_LargeGroup_GetPreferredRemainingBuffers_WithAlternating_1_0_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) =
                SetSegmentInUsePattern(sut, skipCount: 1, (inUseCount: 1, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 0;
            int preferredSegmentId = -1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                buffer.SegmentCount.Should().Be(1);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + 1;
                // Skip one segment for the next test
                expectedSegmentId += 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (1-0-1-1-0) starting with the first segment.
        /// This should return a series of repeating single segment buffers from the second and fifth segments in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithAlternating_1_0_1_1_0_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: 0,
                (inUseCount: 1, availableCount: 1, availableAreZeroed: false), (inUseCount: 2, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 1;
            // We skip 2 first because the first segment is in use
            int usedSegmentSkip = 2;
            int preferredSegmentId = -1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(1);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // Skip one segment for the next test
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + 1;
                expectedSegmentId += 1 + usedSegmentSkip;
                usedSegmentSkip = (usedSegmentSkip == 2) ? 1 : 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (1-0-1-1-0) starting with the second segment.
        /// This should return a series of repeating single segment buffers from the second and fifth segments in the group that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithAlternating_1_0_1_1_0_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: 1,
                (inUseCount: 1, availableCount: 1, availableAreZeroed: false), (inUseCount: 2, availableCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Try to get all the available segments, we should get one segment back each time
            int expectedSegmentId = 0;
            // We skip 1 first because the first segment is available
            int usedSegmentSkip = 1;
            int preferredSegmentId = -1;
            while (availableSegments-- > 0)
            {
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(1);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + 1;
                // Skip one segment for the next test
                expectedSegmentId += 1 + usedSegmentSkip;
                usedSegmentSkip = (usedSegmentSkip == 2) ? 1 : 2;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (0-1-0-0-1) starting with the first segment.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithAlternating_0_1_0_0_1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: 0,
                (availableCount: 1, inUseCount: 1, availableAreZeroed: false), (availableCount: 2, inUseCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
            IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                .OrderByDescending(kvp => kvp.Key)
                .GetEnumerator();
            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

            // The index of the current segment size offset in the available segment map
            int availableSegmentSizeOffsetIndex = 0;
            int preferredSegmentId = -1;

            // Try to get all the available segments
            while (availableSegments > 0)
            {
                // Check if we need to move to the next segment size
                while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                {
                    availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                    availableSegmentSizeOffsetIndex = 0;
                }
                int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(expectedSegmentCount);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                availableSegments -= buffer.SegmentCount;
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + expectedSegmentCount;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is 
        /// (0-1-0-0-1) starting with the second segment.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithAlternating_0_1_0_0_1_Skip1_UsedSegments_GetsProperBuffers ()
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 3);
            (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: 1,
                (availableCount: 1, inUseCount: 1, availableAreZeroed: false), (availableCount: 2, inUseCount: 1, availableAreZeroed: false));
            int expectedUsedSegments = sut.SegmentCount - availableSegments;

            // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
            IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                .OrderByDescending(kvp => kvp.Key)
                .GetEnumerator();
            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

            // The index of the current segment size offset in the available segment map
            int availableSegmentSizeOffsetIndex = 0;
            int preferredSegmentId = -1;

            // Try to get all the available segments
            while (availableSegments > 0)
            {
                // Check if we need to move to the next segment size
                while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                {
                    availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                    availableSegmentSizeOffsetIndex = 0;
                }
                int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                // We'll always request a zeroed buffer for these tests
                (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                    sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                expectedUsedSegments += buffer.SegmentCount;

                // We should get the next available segment in the group (NOT @ the preferred segment offset)
                result.Should().Be(GetBufferResult.Available);
                // We should only get one segment back
                buffer.SegmentCount.Should().Be(expectedSegmentCount);
                buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                availableSegments -= buffer.SegmentCount;
                buffer.IsAllZeroes().Should().BeTrue();
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                // With this pattern, we will never get the preferred segment
                bufferSegmentIsPreferred.Should().BeFalse();
                preferredSegmentId = expectedSegmentId + expectedSegmentCount;
            }

            // An attempt to get another buffer should fail with the group full
            int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
            fullResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is randomly generated.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact(Explicit = true)]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithRandomUsedPatterns_GetsProperBuffers ()
        {
            for (int testLoop = 0; testLoop < 200; testLoop++)
            {
                (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * 5);
                int useSkipCount = GetTestInteger(0, 7);
                int patternCount = GetTestInteger(1, 5);
                bool useAvailablePattern = RandomSource.GetRandomBoolean();

                TestWriteLine($"Test Loop: {testLoop} - Skip: {useSkipCount}, {(useAvailablePattern ? "Available" : "Used")} Patterns: {patternCount}");

                int availableSegments;
                Dictionary<int, List<int>> availableSegmentMap;
                if (useAvailablePattern)
                {
                    (int availableCount, int inUseCount, bool availableAreZeroed)[] pattern = new (int, int, bool)[patternCount];
                    for (int patternIndex = 0; patternIndex < patternCount; patternIndex++)
                    {
                        pattern[patternIndex] = (GetTestInteger(1, 12), GetTestInteger(1, 12), false);
                        TestWriteLine($"    Available: {pattern[patternIndex].availableCount}, Used: {pattern[patternIndex].inUseCount}");
                    }
                    (availableSegments, availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: useSkipCount, pattern);
                }
                else
                {
                    (int inUseCount, int availableCount, bool availableAreZeroed)[] pattern = new (int, int, bool)[patternCount];
                    for (int patternIndex = 0; patternIndex < patternCount; patternIndex++)
                    {
                        pattern[patternIndex] = (GetTestInteger(1, 12), GetTestInteger(1, 12), false);
                        TestWriteLine($"    Used: {pattern[patternIndex].inUseCount}, Available: {pattern[patternIndex].availableCount}");
                    }
                    (availableSegments, availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: useSkipCount, pattern);
                }

                int expectedUsedSegments = sut.SegmentCount - availableSegments;
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);

                // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
                IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                    .OrderByDescending(kvp => kvp.Key)
                    .GetEnumerator();
                availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

                // The index of the current segment size offset in the available segment map
                int availableSegmentSizeOffsetIndex = 0;
                int preferredSegmentId = -1;

                // Try to get all the available segments
                while (availableSegments > 0)
                {
                    try
                    {
                        // Check if we need to move to the next segment size
                        while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                        {
                            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                            availableSegmentSizeOffsetIndex = 0;
                        }
                        int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                        int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                        int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                        // We'll always request a zeroed buffer for these tests
                        (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                            sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                        expectedUsedSegments += buffer.SegmentCount;

                        // We should get the next available segment in the group (NOT @ the preferred segment offset)
                        result.Should().Be(GetBufferResult.Available);
                        // We should only get one segment back
                        buffer.SegmentCount.Should().Be(expectedSegmentCount);
                        buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                        GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                        availableSegments -= buffer.SegmentCount;
                        buffer.IsAllZeroes().Should().BeTrue();
                        buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                        buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                        // Since we are always asking for the maximum segments available, we will never get the preferred segment
                        bufferSegmentIsPreferred.Should().BeFalse();
                        preferredSegmentId = expectedSegmentId + expectedSegmentCount;
                    }
                    catch
                    {
                        TestWriteLine($"** Exception thrown with {availableSegments} segments left to get");
                        throw;
                    }
                }

                // An attempt to get another buffer should fail with the group full
                int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
                fullResult.Should().Be(GetBufferResult.GroupFull);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get the available buffers when the current used pattern is randomly generated.
        /// This should return a series of repeating segment buffers based on the pattern that are zeroed.
        /// </summary>
        [Fact(Explicit = true)]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public void UsingMemorySegmentedBufferGroup_GetPreferredRemainingBuffers_WithRandomUsedBlockFlags_GetsProperBuffers ()
        {
            const int BlockFlagSetTestCount = 9;
            for (int testLoop = 0; testLoop < 200; testLoop++)
            {
                (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * BlockFlagSetTestCount);
                ulong[] setUsedBlockFlags = Enumerable.Range(0, BlockFlagSetSize).Select(_ => GetTestUnsignedLongInteger()).ToArray();
                ulong[] setZeroBlockFlags = Enumerable.Repeat(0, BlockFlagSetSize).Select(_ => 0UL).ToArray();

                (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetBlockUsedFlags(sut, setUsedBlockFlags, setZeroBlockFlags);

                TestWriteLine($"Test Loop: {testLoop} - Used Block Flags: {string.Join(", ", setUsedBlockFlags.Select(flag => flag.ToString("X16")))} - Zero Block Flags: {{all cleared}}");

                int expectedUsedSegments = sut.SegmentCount - availableSegments;
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);

                // Walk through the available segments based on the size of those segments - we expect to get the larger segment groups first
                IEnumerator<KeyValuePair<int, List<int>>> availableSegmentSizeEnumerator = availableSegmentMap
                    .OrderByDescending(kvp => kvp.Key)
                    .GetEnumerator();
                availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();

                // The index of the current segment size offset in the available segment map
                int availableSegmentSizeOffsetIndex = 0;
                int preferredSegmentId = -1;

                // Try to get all the available segments
                while (availableSegments > 0)
                {
                    try
                    {
                        // Check if we need to move to the next segment size
                        while (availableSegmentSizeOffsetIndex >= availableSegmentSizeEnumerator.Current.Value.Count)
                        {
                            availableSegmentSizeEnumerator.MoveNext().Should().BeTrue();
                            availableSegmentSizeOffsetIndex = 0;
                        }
                        int expectedSegmentCount = availableSegmentSizeEnumerator.Current.Key;
                        int expectedSegmentId = availableSegmentSizeEnumerator.Current.Value[availableSegmentSizeOffsetIndex++];
                        int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (availableSegments + 1);

                        // We'll always request a zeroed buffer for these tests
                        (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                            sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                        expectedUsedSegments += buffer.SegmentCount;

                        // We should get the next available segment in the group (NOT @ the preferred segment offset)
                        result.Should().Be(GetBufferResult.Available);
                        // We should only get one segment back
                        buffer.SegmentCount.Should().Be(expectedSegmentCount);
                        buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                        GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                        availableSegments -= buffer.SegmentCount;
                        buffer.IsAllZeroes().Should().BeTrue();
                        buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                        buffer.BufferInfo.SegmentId.Should().Be(expectedSegmentId);
                        // Since we are always asking for the maximum segments available, we will never get the preferred segment
                        bufferSegmentIsPreferred.Should().BeFalse();
                        preferredSegmentId = expectedSegmentId + expectedSegmentCount;
                    }
                    catch
                    {
                        TestWriteLine($"** Exception thrown with {availableSegments} segments left to get");
                        throw;
                    }
                }

                // An attempt to get another buffer should fail with the group full
                int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
                fullResult.Should().Be(GetBufferResult.GroupFull);
            }
        }
        //--------------------------------------------------------------------------------    

        #endregion Get Preferred Remaining Buffers Tests

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get random buffers when the current used pattern is randomly generated.
        /// </summary>
        [Fact(Explicit = true)]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public void UsingMemorySegmentedBufferGroup_GetRandomBuffers_WithRandomUsedBlockFlags_GetsProperBuffers ()
        {
            const int blockFlagSetTestCount = 9;
            TimeSpan timeBoxRunTime = DefaultExplicitTestTimeBox;
            Stopwatch runTimer = Stopwatch.StartNew();
            int testLoopCount = 0;

            while (runTimer.Elapsed < timeBoxRunTime)
            {
                testLoopCount++;
                // Get the buffer group being tested, and a simple buffer pool for attaching to the buffers
                (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * blockFlagSetTestCount);
                // Get a set of used block flags for the test
                ulong[] setUsedBlockFlags = Enumerable.Range(0, BlockFlagSetSize).Select(_ => GetTestUnsignedLongInteger()).ToArray();
                // Get a set of zeroed block flags for the test - in this case, none of the blocks are marked as zeroed
                ulong[] setZeroBlockFlags = Enumerable.Repeat(0, BlockFlagSetSize).Select(_ => 0UL).ToArray();

                // Setup the internal current state of used and zeroed block flags in the buffer group, and get the number of available segments
                // as well as a map of available segments by size
                (int availableSegments, Dictionary<int, List<int>> availableSegmentMap) = SetBlockUsedFlags(sut, setUsedBlockFlags, setZeroBlockFlags);

#pragma warning disable HAA0601
                TestWriteLine($"Test Loop: {testLoopCount} - Used Block Flags: {string.Join(", ", setUsedBlockFlags.Select(flag => flag.ToString("X16")))} - Zero Block Flags: {{all cleared}}");
#pragma warning restore HAA0601

                // Track what we expect to be the number of used segments in the group
                int expectedUsedSegments = sut.SegmentCount - availableSegments;
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);

                // Start out NOT having a preferred segment ID
                int preferredSegmentId = -1;

                // Randomly get buffers until we can't get any more segments
                while (availableSegments > 0)
                {
                    try
                    {
                        // Get a random number of segments to request for the buffer
                        int requestSegmentCount = GetTestInteger(1, Math.Min(9, availableSegments) + 1);
                        // Calculate the buffer size based on the number of segments requested
                        int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * requestSegmentCount;

                        // We'll always request a zeroed buffer for these tests
                        (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                            sut.GetBuffer(bufferSize: requestBufferSize, requireZeroed: true,
                                bufferPool: bufferPool, preferredFirstSegmentIndex: preferredSegmentId);
                        // Be sure we got the buffer
                        result.Should().Be(GetBufferResult.Available);

                        // Update the expected used segments count and the number of available segments
                        expectedUsedSegments += buffer.SegmentCount;
                        availableSegments -= buffer.SegmentCount;

                        // Check the number of segments returned
                        buffer.SegmentCount.Should().BeLessThanOrEqualTo(requestSegmentCount);
                        buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                        GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                        // While the buffer blocks are marked as not-zeroed, we are always asking for zeroed buffers
                        buffer.IsAllZeroes().Should().BeTrue();
                        buffer.BufferInfo.BlockId.Should().Be(sut.Id);

                        // Check if we got the preferred segment
                        if (preferredSegmentId >= 0)
                            bufferSegmentIsPreferred.Should().Be(preferredSegmentId == buffer.BufferInfo.SegmentId);
                        else
                            bufferSegmentIsPreferred.Should().BeFalse();

                        // If this is a preferred segment, the entry could come from anywhere
                        // in the available segment map, so we can't rely on it coming from
                        // the first entry in the map for a given segment size.
                        (KeyValuePair<int, List<int>> segmentSizeEntry, int segmentSizeListIndex) =
                            bufferSegmentIsPreferred ?
                                availableSegmentMap
                                    .Select(kvp => (mapEntry: kvp, segmentIndex: kvp.Value.IndexOf(buffer.BufferInfo.SegmentId)))
                                    .FirstOrDefault(kvp => kvp.segmentIndex >= 0) :
                                (availableSegmentMap
                                .FirstOrDefault(kvp => kvp.Value[0] == buffer.BufferInfo.SegmentId), 0);
                        // Make sure we found the segment size entry
                        segmentSizeEntry.Should().NotBe(default(KeyValuePair<int, List<int>>));

                        // Make sure the segment was gotten from a properly sized segment group
                        buffer.SegmentCount.Should().BeLessThanOrEqualTo(segmentSizeEntry.Key);
                        // Reset preferred segment for the next test but we may set it later
                        // THIS MUST STAY HERE - we need to reset the preferred segment ID
                        // AFTER we check the buffer segment is preferred above
                        preferredSegmentId = -1;

                        segmentSizeEntry.Value.RemoveAt(segmentSizeListIndex);
                        // If there are no more entries in the list, remove the entry
                        if (0 == segmentSizeEntry.Value.Count)
                            availableSegmentMap.Remove(segmentSizeEntry.Key);
                        // If we found the exact size for the available map, then we can just continue
                        if (segmentSizeEntry.Key == buffer.SegmentCount)
                        {
                            continue;
                        }
                        // The segment size was smaller than the available space in this group, so 
                        // adjust the available map.
                        int remainingGroupSegments = segmentSizeEntry.Key - buffer.SegmentCount;
                        // Set the preferred segment ID to the next segment after the one we just got
                        // for the next loop
                        preferredSegmentId = buffer.BufferInfo.SegmentId + buffer.SegmentCount;
                        if (availableSegmentMap.ContainsKey(remainingGroupSegments))
                        {
                            availableSegmentMap[remainingGroupSegments].Add(preferredSegmentId);
                            availableSegmentMap[remainingGroupSegments].Sort();
                            continue;
                        }
                        availableSegmentMap[remainingGroupSegments] = [preferredSegmentId];
                    }
                    catch
                    {
                        TestWriteLine($"** Exception thrown with {availableSegments} segments left to get");
                        throw;
                    }
                }

                // An attempt to get another buffer should fail with the group full
                int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
                fullResult.Should().Be(GetBufferResult.GroupFull);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemorySegmentedBufferGroup.GetBuffer(int, bool, MemorySegmentedBufferPool)"/> 
        /// method to get random buffers when the current used pattern is randomly generated.
        /// </summary>
        [Fact(Explicit = true)]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public void UsingMemorySegmentedBufferGroup_GetRandomBuffers_WithRandomUsedPatterns_GetsProperBuffers ()
        {
            const int BlockFlagSetTestCount = 9;
            for (int testLoop = 0; testLoop < 200; testLoop++)
            {
                (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(segmentCount: BlockFlagSetSize * BlockFlagSetTestCount);
                int useSkipCount = GetTestInteger(0, 7);
                int patternCount = GetTestInteger(1, 5);
                bool useAvailablePattern = RandomSource.GetRandomBoolean();

                TestWriteLine($"Test Loop: {testLoop} - Skip: {useSkipCount}, {(useAvailablePattern ? "Available" : "Used")} Patterns: {patternCount}");

                int availableSegments;
                Dictionary<int, List<int>> availableSegmentMap;
                if (useAvailablePattern)
                {
                    (int availableCount, int inUseCount, bool availableAreZeroed)[] pattern = new (int, int, bool)[patternCount];
                    for (int patternIndex = 0; patternIndex < patternCount; patternIndex++)
                    {
                        pattern[patternIndex] = (GetTestInteger(1, 12), GetTestInteger(1, 12), false);
                        TestWriteLine($"    Available: {pattern[patternIndex].availableCount}, Used: {pattern[patternIndex].inUseCount}");
                    }
                    (availableSegments, availableSegmentMap) = SetSegmentAvailablePattern(sut, skipCount: useSkipCount, pattern);
                }
                else
                {
                    (int inUseCount, int availableCount, bool availableAreZeroed)[] pattern = new (int, int, bool)[patternCount];
                    for (int patternIndex = 0; patternIndex < patternCount; patternIndex++)
                    {
                        pattern[patternIndex] = (GetTestInteger(1, 12), GetTestInteger(1, 12), false);
                        TestWriteLine($"    Used: {pattern[patternIndex].inUseCount}, Available: {pattern[patternIndex].availableCount}");
                    }
                    (availableSegments, availableSegmentMap) = SetSegmentInUsePattern(sut, skipCount: useSkipCount, pattern);
                }

                int expectedUsedSegments = sut.SegmentCount - availableSegments;
                GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);

                int preferredSegmentId = -1;

                // Randomly get buffers until we can't get any more segments
                while (availableSegments > 0)
                {
                    try
                    {
                        int requestSegmentCount = GetTestInteger(1, Math.Min(9, availableSegments) + 1);
                        int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * requestSegmentCount;

                        // We'll always request a zeroed buffer for these tests
                        (SegmentBuffer buffer, GetBufferResult result, bool bufferSegmentIsPreferred) =
                            sut.GetBuffer(requestBufferSize, true, bufferPool, preferredSegmentId);
                        // Be sure we got the buffer
                        result.Should().Be(GetBufferResult.Available);

                        expectedUsedSegments += buffer.SegmentCount;
                        availableSegments -= buffer.SegmentCount;
                        // Check the number of segments returned
                        buffer.SegmentCount.Should().BeLessThanOrEqualTo(requestSegmentCount);
                        buffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize * buffer.SegmentCount);
                        GetSegmentsInUse(sut).Should().Be(expectedUsedSegments);
                        buffer.IsAllZeroes().Should().BeTrue();
                        buffer.BufferInfo.BlockId.Should().Be(sut.Id);

                        // Check if we got the preferred segment
                        if (preferredSegmentId >= 0)
                            bufferSegmentIsPreferred.Should().Be(preferredSegmentId == buffer.BufferInfo.SegmentId);
                        else
                            bufferSegmentIsPreferred.Should().BeFalse();

                        // If this is a preferred segment, the entry could come from anywhere
                        // in the available segment map, so we can't rely on it coming from
                        // the first entry in the map for a given segment size.
                        (KeyValuePair<int, List<int>> segmentSizeEntry, int segmentSizeListIndex) =
                            bufferSegmentIsPreferred ?
                                availableSegmentMap
                                    .Select(kvp => (mapEntry: kvp, segmentIndex: kvp.Value.IndexOf(buffer.BufferInfo.SegmentId)))
                                    .FirstOrDefault(kvp => kvp.segmentIndex >= 0) :
                                (availableSegmentMap
                                .FirstOrDefault(kvp => kvp.Value[0] == buffer.BufferInfo.SegmentId), 0);
                        // Make sure we found the segment size entry
                        segmentSizeEntry.Should().NotBe(default(KeyValuePair<int, List<int>>));

                        // Make sure the segment was gotten from a properly sized segment group
                        buffer.SegmentCount.Should().BeLessThanOrEqualTo(segmentSizeEntry.Key);
                        // Reset preferred segment for the next test but we may set it later
                        // THIS MUST STAY HERE - we need to reset the preferred segment ID
                        // after we check the buffer segment is preferred above
                        preferredSegmentId = -1;

                        segmentSizeEntry.Value.RemoveAt(segmentSizeListIndex);
                        // If there are no more entries in the list, remove the entry
                        if (0 == segmentSizeEntry.Value.Count)
                            availableSegmentMap.Remove(segmentSizeEntry.Key);
                        // If we found the exact size for the available map, then we can just continue
                        if (segmentSizeEntry.Key == buffer.SegmentCount)
                        {
                            continue;
                        }
                        // The segment size was smaller than the available space in this group, so 
                        // adjust the available map.
                        int remainingGroupSegments = segmentSizeEntry.Key - buffer.SegmentCount;
                        // Set the preferred segment ID to the next segment after the one we just got
                        // for the next loop
                        preferredSegmentId = buffer.BufferInfo.SegmentId + buffer.SegmentCount;
                        if (availableSegmentMap.ContainsKey(remainingGroupSegments))
                        {
                            availableSegmentMap[remainingGroupSegments].Add(preferredSegmentId);
                            availableSegmentMap[remainingGroupSegments].Sort();
                            continue;
                        }
                        availableSegmentMap[remainingGroupSegments] = [preferredSegmentId];
                    }
                    catch
                    {
                        TestWriteLine($"** Exception thrown with {availableSegments} segments left to get");
                        throw;
                    }
                }

                // An attempt to get another buffer should fail with the group full
                int fullRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer _, GetBufferResult fullResult) = sut.GetBuffer(fullRequestBufferSize, true, bufferPool);
                fullResult.Should().Be(GetBufferResult.GroupFull);
            }
        }
        //--------------------------------------------------------------------------------    

        //================================================================================

        #endregion Test Methods
    }
}
