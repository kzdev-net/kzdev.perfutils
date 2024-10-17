// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils.Internals;

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemorySegmentedBufferGroup"/> class.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, "Memory")]
    public class UsingMemoryStreamSegmentedBufferGroup : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSegmentedBufferGroup"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamSegmentedBufferGroup (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns an instance of the system under test using the specified segment count.
        /// </summary>
        /// <param name="segmentCount">
        /// The number of segments to use in the buffer group.
        /// </param>
        /// <param name="useNativeMemory">
        /// Indicates if native memory should be used for the large memory buffer segments.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="MemorySegmentedBufferGroup"/> system under test.
        /// </returns>
        private MemorySegmentedBufferGroup GetSut (bool useNativeMemory = false, int segmentCount = 16) => new(segmentCount, useNativeMemory);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns an instance of the memory segmented buffer pool.
        /// </summary>
        /// <param name="useNativeMemory">
        /// Indicates if native memory should be used for the large memory buffer segments.
        /// </param>
        /// <returns>
        /// A test instance of the <see cref="MemorySegmentedBufferPool"/>.
        /// </returns>
        private MemorySegmentedBufferPool GetTestBufferPool (bool useNativeMemory = false) => new(useNativeMemory);
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting instances of the <see cref="MemorySegmentedBufferGroup"/> class with 
        /// various segment counts and verifies that the segment count is correct.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GettingInstanceWithVariousSegmentCounts_SegmentCountIsCorrect ()
        {
            for (int testLoop = 0; testLoop < 100; testLoop++)
            {
                int segmentCount = GetTestInteger(1, MemorySegmentedGroupGenerationArray.MaxAllowedGroupSegmentCount + 1);
                MemorySegmentedBufferGroup sut = GetSut(segmentCount: segmentCount);
                sut.SegmentCount.Should().Be(segmentCount);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// requesting all the segments and verifies that the buffer is the correct size.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetFullGroupBuffer_ReturnsProperBuffer ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int requestBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * sut.SegmentCount;

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_TryTooLargeGetBuffer_ReturnsSegmentWithPartOfRequest ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int expectedBufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize * sut.SegmentCount;
            int requestBufferSize = expectedBufferSize + MemorySegmentedBufferGroup.StandardBufferSegmentSize;

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetThreeBuffers_BuffersReturnedAreCorrectSizeAndValues ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int availableSegments = sut.SegmentCount;
            int[] getSegments = new int[3];
            // We don't want single segment buffers
            getSegments[0] = GetTestInteger(2, (availableSegments / 3) + 1);
            getSegments[1] = GetTestInteger(2, (availableSegments / 3) + 1);
            getSegments[2] = availableSegments - getSegments[0] - getSegments[1];

            SegmentBuffer[] segmentBuffers = new SegmentBuffer[3];
            for (int i = 0; i < 3; i++)
            {
                int requestBufferSize = getSegments[i] * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
                (SegmentBuffer buffer, GetBufferResult result) = sut.GetBuffer(requestBufferSize, false, bufferPool);

                result.Should().Be(GetBufferResult.Available);
                buffer.Length.Should().Be(requestBufferSize);
                buffer.SegmentCount.Should().Be(getSegments[i]);
                buffer.BufferInfo.BlockId.Should().Be(sut.Id);
                buffer.BufferInfo.SegmentId.Should().Be((i == 0) ? 0 : (segmentBuffers[i - 1].BufferInfo.SegmentId + segmentBuffers[i - 1].BufferInfo.SegmentCount));
                segmentBuffers[i] = buffer;
            }
        }
        //--------------------------------------------------------------------------------    

        #region Single Segment Allocation

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a single segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the start of the buffer group.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleNonZeroedSegmentBuffer_ShouldAllocateFromTheStart ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleNonZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = sut.SegmentCount / 2;

            sut.SetSegmentsUsed(0, markSegmentCount);

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleNonZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentIndex = sut.SegmentCount - 1;

            while (markSegmentIndex >= 0)
            {
                sut.SetAllSegmentsUsed();
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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleZeroedSegmentBuffer_ShouldAllocateFromTheStart ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int bufferSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = sut.SegmentCount / 2;

            sut.SetSegmentsUsed(0, markSegmentCount);

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetSingleSegmentAfterFull_ShouldReturnFullResult ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
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

        #region Multiple Segment Allocation

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a multiple segment buffer from the <see cref="MemorySegmentedBufferGroup"/> class
        /// which should be allocated at the end of the buffer group.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleNonZeroedSegmentBuffer_ShouldAllocateFromTheStart ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleNonZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int segmentCount = GetTestInteger(2, sut.SegmentCount - 4);
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = GetTestInteger(1, sut.SegmentCount - segmentCount + 1);

            sut.SetSegmentsUsed(0, markSegmentCount);

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleNonZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();

            for (int fillSegmentCount = 1; fillSegmentCount < sut.SegmentCount - 2; fillSegmentCount++)
            {
                sut.SetAllSegmentsFree(false);
                sut.SetSegmentsUsed(0, fillSegmentCount);

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleZeroedSegmentBuffer_ShouldAllocateFromTheStart ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleZeroedSegmentBuffer_ShouldAllocateFromTheStartOfAvailable ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();
            int segmentCount = GetTestInteger(2, sut.SegmentCount - 4);
            int bufferSize = segmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            int markSegmentCount = GetTestInteger(1, sut.SegmentCount - segmentCount + 1);

            sut.SetSegmentsUsed(0, markSegmentCount);

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleZeroedSegmentBuffer_FromSingleAvailableSegments_ShouldAllocateFromAvailableSegment ()
        {
            MemorySegmentedBufferGroup sut = GetSut();
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();

            for (int fillSegmentCount = 1; fillSegmentCount < sut.SegmentCount - 2; fillSegmentCount++)
            {
                sut.SetAllSegmentsFree(false);
                sut.SetSegmentsUsed(0, fillSegmentCount);

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleSegmentBuffer_LargerThanAvailable_WithProgressivelyLargerHoles_ShouldReturnLargestAvailable ()
        {
            MemorySegmentedBufferGroup sut = GetSut(segmentCount: 128);
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();

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
        [Fact]
        public void UsingMemoryStreamSegmentedBufferGroup_GetMultipleSegmentBuffer_LargerThanAvailable_WithProgressivelySmallerHoles_ShouldReturnLargestAvailable ()
        {
            MemorySegmentedBufferGroup sut = GetSut(segmentCount: 128);
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool();

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

        //================================================================================

        #endregion Test Methods
    }
}
