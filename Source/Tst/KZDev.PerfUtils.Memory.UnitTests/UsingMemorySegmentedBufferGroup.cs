// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemorySegmentedBufferGroup"/> class.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, "Memory")]
    public class UsingMemorySegmentedBufferGroup : MemorySegmentedBufferGroupUnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemorySegmentedBufferGroup"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemorySegmentedBufferGroup (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting instances of the <see cref="MemorySegmentedBufferGroup"/> class with 
        /// various segment counts and verifies that the segment count is correct.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GettingInstanceWithVariousSegmentCounts_SegmentCountIsCorrect (bool useNativeMemory)
        {
            for (int testLoop = 0; testLoop < 100; testLoop++)
            {
                int segmentCount = GetTestInteger(1, MemorySegmentedGroupGenerationArray.MaxAllowedGroupSegmentCount + 1);
                MemorySegmentedBufferGroup sut = GetTestBufferGroup(useNativeMemory: useNativeMemory, segmentCount: segmentCount);
                sut.SegmentCount.Should().Be(segmentCount);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// requesting all the segments and verifies that the buffer is the correct size.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetFullGroupBuffer_ReturnsProperBuffer (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            // We want to get the full buffer
            int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * sut.SegmentCount;

            // We don't need the buffer zeroed, but we expect to get the full buffer that starts at segment 0
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestBufferSize);
            buffer.SegmentCount.Should().Be(sut.SegmentCount);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// requesting more segments than are available and verifies that we get a buffer
        /// with as many segments as are available.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_TryTooLargeGetBuffer_ReturnsSegmentWithPartOfRequest (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            // We expect to get the full buffer
            int expectedBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * sut.SegmentCount;
            // We want to get a buffer that is larger than the group
            int requestBufferSize = expectedBufferSize + MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            // We don't need the buffer zeroed, but we expect to get the full buffer that starts at segment 0
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(expectedBufferSize);
            buffer.SegmentCount.Should().Be(sut.SegmentCount);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting three buffers from the <see cref="MemorySegmentedBufferGroup"/> class
        /// requesting various segments and verifying that we get the full number of segments
        /// in the different buffers, and they are all the correct size.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetThreeBuffers_BuffersReturnedAreCorrectSizeAndValues (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int availableSegments = sut.SegmentCount;
            int[] getSegments = new int[3];

            // We don't want single segment buffers, and no more than 1/3 of the available segments in each request
            getSegments[0] = GetTestInteger(2, (availableSegments / 3) + 1);
            getSegments[1] = GetTestInteger(2, (availableSegments / 3) + 1);
            getSegments[2] = availableSegments - getSegments[0] - getSegments[1];

            SegmentBuffer[] segmentBuffers = new SegmentBuffer[3];
            for (int requestIndex = 0; requestIndex < 3; requestIndex++)
            {
                // How large is this buffer?
                int requestBufferSize = getSegments[requestIndex] * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                // We don't need the buffer zeroed, and we expect to get the full buffer size requested
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, false, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(requestBufferSize);
                buffer.SegmentCount.Should().Be(getSegments[requestIndex]);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                // A new buffer group should return consecutive segments (ID is the segment number offset in the group)
                buffer.BufferInfo.SegmentId.Should().Be((requestIndex == 0) ? 0 : (segmentBuffers[requestIndex - 1].BufferInfo.SegmentId + segmentBuffers[requestIndex - 1].BufferInfo.SegmentCount));
                segmentBuffers[requestIndex] = buffer;
            }
        }
        //--------------------------------------------------------------------------------    

        #region Single Segment Allocation

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the start of the buffer group. We verify that we get the correct buffer
        /// at the start of the group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleNonZeroedSegmentBuffer_ShouldAllocateFromTheStart (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            // We are asking for a non-zeroed buffer, and we expect to get a segment sized buffer that starts at segment 0
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the start of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleNonZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            // Single segment buffer size
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = sut.SegmentCount / 2;

            // Set the internal segments to used up to the mark (1/2 of the segments)
            sut.SetSegmentsUsed(0, markSegmentCount).Should().Be(markSegmentCount);

            // We are asking for a non-zeroed buffer, and we expect to get a segment sized buffer that starts at the mark
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(markSegmentCount);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleNonZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentIndex = sut.SegmentCount - 1;

            // Move from the last to the first internal segment and try to get that segment as a buffer
            while (markSegmentIndex >= 0)
            {
                // Initially reset to all segments being used
                sut.SetAllSegmentsUsed();
                // Set the single segment we are expecting to get as free
                sut.SetSegmentsFree(markSegmentIndex, 1, false);

                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, false, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(bufferSize);
                buffer.SegmentCount.Should().Be(1);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(markSegmentIndex);
                markSegmentIndex--;
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the start of the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleZeroedSegmentBuffer_ShouldAllocateFromTheStart (bool useNativeMemory)
        {
            // Start with a group that is not zeroed
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory, nonZeroFilled: true);
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            // We are asking for a zeroed buffer, and we expect to get a segment sized buffer that starts at segment 0
            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
            buffer.IsAllZeroes().Should().BeTrue();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the start of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory, nonZeroFilled: true);
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = sut.SegmentCount / 2;

            sut.SetSegmentsUsed(0, markSegmentCount).Should().Be(markSegmentCount);

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(1);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(markSegmentCount);
            buffer.IsAllZeroes().Should().BeTrue();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory, nonZeroFilled: true);
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentIndex = sut.SegmentCount - 1;

            while (markSegmentIndex >= 0)
            {
                sut.SetAllSegmentsUsed();
                sut.SetSegmentsFree(markSegmentIndex, 1, false);

                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(bufferSize);
                buffer.SegmentCount.Should().Be(1);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(markSegmentIndex);
                buffer.IsAllZeroes().Should().BeTrue();
                markSegmentIndex--;
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// one more time than there are segments and verifies that we get the proper result.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSingleSegmentAfterFull_ShouldReturnFullResult (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory, nonZeroFilled: true);
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            for (int segmentNumber = 0; segmentNumber < sut.SegmentCount; segmentNumber++)
            {
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(bufferSize);
                buffer.SegmentCount.Should().Be(1);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(segmentNumber);
                buffer.IsAllZeroes().Should().BeTrue();
            }

            (SegmentBuffer _, GetBufferResult checkResult) = sut.GetBuffer(bufferSize, true, bufferPool);

            checkResult.Should().Be(GetBufferResult.GroupFull);
        }
        //--------------------------------------------------------------------------------    

        #endregion Single Segment Allocation


        #region Single Preferred Segment Allocation

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single preferred segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the preferred segment index.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSinglePreferredSegmentBuffer_ShouldAllocateStartingAtPreferredIndex (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int requestFirstBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * GetTestInteger(1, sut.SegmentCount);

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestFirstBufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestFirstBufferSize);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);

            (SegmentBuffer nextBuffer, GetBufferResult nextResult, bool isPreferredSegment) =
                sut.GetBuffer(MemorySegmentedBufferGroup.StandardBufferSegmentSize, false, bufferPool, buffer.SegmentCount);
            nextResult.Should().Be(GetBufferResult.Available);
            nextBuffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
            isPreferredSegment.Should().BeTrue();
            nextBuffer.BufferInfo.BlockId.Should().Be(sut.Id);
            nextBuffer.BufferInfo.SegmentId.Should().Be(buffer.SegmentCount);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single preferred segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the preferred segment index.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetSinglePreferredSegmentBuffer_FromSingleAvailableSegment_ShouldAllocateStartingAtPreferredIndex (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int requestFirstBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (sut.SegmentCount - 1);

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestFirstBufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestFirstBufferSize);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);

            (SegmentBuffer nextBuffer, GetBufferResult nextResult, bool isPreferredSegment) =
                sut.GetBuffer(MemorySegmentedBufferGroup.StandardBufferSegmentSize, false, bufferPool, buffer.SegmentCount);
            nextResult.Should().Be(GetBufferResult.Available);
            nextBuffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
            isPreferredSegment.Should().BeTrue();
            nextBuffer.BufferInfo.BlockId.Should().Be(sut.Id);
            nextBuffer.BufferInfo.SegmentId.Should().Be(buffer.SegmentCount);
        }
        //--------------------------------------------------------------------------------    

        #endregion Single Preferred Segment Allocation

        #region Multiple Segment Allocation

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleNonZeroedSegmentBuffer_ShouldAllocateFromTheStart (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int segmentCount = GetTestInteger(2, sut.SegmentCount);
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(segmentCount);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleNonZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int segmentCount = GetTestInteger(2, sut.SegmentCount - 4);
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = GetTestInteger(1, sut.SegmentCount - segmentCount + 1);

            sut.SetSegmentsUsed(0, markSegmentCount).Should().Be(markSegmentCount);

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(segmentCount);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(markSegmentCount);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleNonZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);

            for (int fillSegmentCount = 1; fillSegmentCount < sut.SegmentCount - 2; fillSegmentCount++)
            {
                sut.SetAllSegmentsFree(false);
                sut.SetSegmentsUsed(0, fillSegmentCount).Should().Be(fillSegmentCount);

                int segmentCount = GetTestInteger(2, sut.SegmentCount - fillSegmentCount + 1);
                int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;

                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, false, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(bufferSize);
                buffer.SegmentCount.Should().Be(segmentCount);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(fillSegmentCount);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleZeroedSegmentBuffer_ShouldAllocateFromTheStart (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int segmentCount = GetTestInteger(2, sut.SegmentCount);
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(segmentCount);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);
            buffer.IsAllZeroes().Should().BeTrue();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int segmentCount = GetTestInteger(2, sut.SegmentCount - 4);
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = GetTestInteger(1, sut.SegmentCount - segmentCount + 1);

            sut.SetSegmentsUsed(0, markSegmentCount).Should().Be(markSegmentCount);

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(bufferSize);
            buffer.SegmentCount.Should().Be(segmentCount);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(markSegmentCount);
            buffer.IsAllZeroes().Should().BeTrue();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the free segments in the buffer group.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);

            for (int fillSegmentCount = 1; fillSegmentCount < sut.SegmentCount - 2; fillSegmentCount++)
            {
                sut.SetAllSegmentsFree(false);
                sut.SetSegmentsUsed(0, fillSegmentCount).Should().Be(fillSegmentCount);

                int segmentCount = GetTestInteger(2, sut.SegmentCount - fillSegmentCount + 1);
                int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;

                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(bufferSize);
                buffer.SegmentCount.Should().Be(segmentCount);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be(fillSegmentCount);
                buffer.IsAllZeroes().Should().BeTrue();
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// from a group that has a series of free holes that are progressively larger but none are large enough
        /// to fulfill the request, and this should return the largest available.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleSegmentBuffer_LargerThanAvailable_WithProgressivelyLargerHoles_ShouldReturnLargestAvailable (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);

            sut.SetAllSegmentsUsed();
            int currentOpenSegmentCount = 1;
            int markOpenSegmentIndex = 0;

            // Make a set of holes that are progressively larger 
            while ((markOpenSegmentIndex + currentOpenSegmentCount) < sut.SegmentCount - 1)
            {
                sut.SetSegmentsFree(markOpenSegmentIndex, currentOpenSegmentCount, false);
                currentOpenSegmentCount++;
                // Move to the next hole
                markOpenSegmentIndex += currentOpenSegmentCount;
            }

            int segmentCount = currentOpenSegmentCount;
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int expectedSegmentCount = segmentCount - 1;
            int expectedBufferSize = expectedSegmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(expectedBufferSize);
            buffer.SegmentCount.Should().Be(expectedSegmentCount);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// from a group that has a series of free holes that are progressively smaller but none are large enough
        /// to fulfill the request, and this should return the largest available.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultipleSegmentBuffer_LargerThanAvailable_WithProgressivelySmallerHoles_ShouldReturnLargestAvailable (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);

            sut.SetAllSegmentsUsed();
            int currentOpenSegmentCount = 1;
            int markOpenSegmentIndex = sut.SegmentCount - 1;

            // Make a set of holes that are progressively smaller (by making larger holes from the end to the beginning) 
            while ((markOpenSegmentIndex - currentOpenSegmentCount) >= 0)
            {
                sut.SetSegmentsFree(markOpenSegmentIndex - currentOpenSegmentCount, currentOpenSegmentCount, false);
                currentOpenSegmentCount++;
                // Move to the next hole
                markOpenSegmentIndex -= currentOpenSegmentCount;
            }

            int segmentCount = currentOpenSegmentCount;
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int expectedSegmentCount = segmentCount - 1;
            int expectedBufferSize = expectedSegmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(bufferSize, true, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(expectedBufferSize);
            buffer.SegmentCount.Should().Be(expectedSegmentCount);
        }
        //--------------------------------------------------------------------------------    

        #endregion Multiple Segment Allocation

        #region Multiple Preferred Segment Allocation

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple preferred segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the preferred segment index.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultiplePreferredSegmentBuffer_ShouldAllocateStartingAtPreferredIndex (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int firstRequestSegmentCount = GetTestInteger(1, sut.SegmentCount - 1);
            int requestFirstBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * firstRequestSegmentCount;

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestFirstBufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestFirstBufferSize);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);

            int nextRequestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (sut.SegmentCount - firstRequestSegmentCount);
            (SegmentBuffer nextBuffer, GetBufferResult nextResult, bool isPreferredSegment) =
                sut.GetBuffer(nextRequestBufferSize, false, bufferPool, buffer.SegmentCount);
            nextResult.Should().Be(GetBufferResult.Available);
            nextBuffer.Length.Should().Be(nextRequestBufferSize);
            isPreferredSegment.Should().BeTrue();
            nextBuffer.BufferInfo.BlockId.Should().Be(sut.Id);
            nextBuffer.BufferInfo.SegmentId.Should().Be(buffer.SegmentCount);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple preferred segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the preferred segment index.
        /// </summary>
        [Theory]
        [ClassData(typeof(BoolValuesData))]
        public void UsingMemorySegmentedBufferGroup_GetMultiplePreferredSegmentBuffer_FromSingleAvailableSegment_ShouldAllocateStartingAtPreferredIndex (bool useNativeMemory)
        {
            (MemorySegmentedBufferGroup sut, MemorySegmentedBufferPool bufferPool) = GetTestGroupAndPool(useNativeMemory: useNativeMemory);
            int requestFirstBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * (sut.SegmentCount - 1);

            (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestFirstBufferSize, false, bufferPool);

            result.Should().Be(GetBufferResult.Available);
            buffer.Length.Should().Be(requestFirstBufferSize);
            buffer.BufferInfo.BlockId.Should().Be(sut.Id);
            buffer.BufferInfo.SegmentId.Should().Be(0);

            // We are going to request a buffer that is one segment larger than the available segment, but we should
            // get just one segment back
            (SegmentBuffer nextBuffer, GetBufferResult nextResult, bool isPreferredSegment) =
                sut.GetBuffer(MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2, false, bufferPool, buffer.SegmentCount);
            nextResult.Should().Be(GetBufferResult.Available);
            nextBuffer.Length.Should().Be(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
            isPreferredSegment.Should().BeTrue();
            nextBuffer.BufferInfo.BlockId.Should().Be(sut.Id);
            nextBuffer.BufferInfo.SegmentId.Should().Be(buffer.SegmentCount);
        }
        //--------------------------------------------------------------------------------    

        #endregion Multiple Preferred Segment Allocation

        //================================================================================

        #endregion Test Methods
    }
}
