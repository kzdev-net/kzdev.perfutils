// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using KZDev.PerfUtils.Internals;

#pragma warning disable HAA0601

/*
 * We minimize use of Fluent Assertions in this file because a number of these methods 
 * are hot paths in the tests, and Fluent Assertions often times allocate a number of strings.
 * 
 * Also, we use if statements with Assert.Fail() instead of the Assert.True() and Assert.False()
 * methods because we pass interpolated strings to those methods that we don't want to be evaluated
 * unless the test fails.
 */

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Base class for tests for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    public abstract class UsingMemoryStreamSlimUnitTestBase : UnitTestBase
    {
        /*
         * NOTE: For a number of tests in derived classes, we purposely run a loop over a set of test values
         * instead of using Theory type tests because the test explorer shows each Theory
         * value set as a unique test and given the large number of different test values we 
         * may have, the unit test explorer can get difficult to navigate.
         */

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Static constructor for the <see cref="UsingMemoryStreamSlimUnitTestBase"/> class.
        /// </summary>
        static UsingMemoryStreamSlimUnitTestBase ()
        {
#if NATIVEMEMORY
            MemoryStreamSlim.UseNativeLargeMemoryBuffers = true;
#else
            MemoryStreamSlim.UseNativeLargeMemoryBuffers = false;
#endif
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A set of sizes used for 'segmented' fill and read operations
        /// </summary>
        protected static readonly int[] TestSegmentSizes = [0x10, 0x47, 0x100, 0x1000, 0x7919, 0x1_0000, 0x2_0000];
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Helper for permuting the values in an array of a range of values
        /// </summary>
        /// <param name="permutation">The permutation array to fill with values</param>
        /// <param name="indexUsed">Tracking which permutation index slots are used</param>
        /// <param name="maxValue">The maximum value (or depth) that we will go for the current value</param>
        /// <param name="currentValue">The current value being placed in the permutation</param>
        /// <param name="depth">The depth into the permutation that the current value represents</param>
        private static IEnumerable<int[]> PermuteRangeValue (int[] permutation, bool[] indexUsed,
                int maxValue, int currentValue, int depth)
        {
            if (depth > maxValue)
            {
                int[] returnArray = new int[permutation.Length];
                Array.Copy(permutation, returnArray, permutation.Length);
                yield return returnArray;
            }
            else
            {
                // Depending on the depth, we may have to "loop" the current value
                if (currentValue > maxValue)
                    currentValue = 0;
                for (int useIndex = 0; useIndex < permutation.Length; useIndex++)
                {
                    if (indexUsed[useIndex])
                        continue;
                    permutation[useIndex] = currentValue;
                    indexUsed[useIndex] = true;
                    foreach (int[] returnSet in PermuteRangeValue(permutation, indexUsed, maxValue, currentValue + 1, depth + 1))
                    {
                        yield return returnSet;
                    }
                    indexUsed[useIndex] = false;
                }
            }
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Enumerates over all the possible permutations of values from 0 to <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value to use for the permutations. We will fill the return array with values from 0 to <paramref name="maxValue"/>
        /// </param>
        /// <returns>
        /// An enumerable collection of all the possible permutations of values from 0 to <paramref name="maxValue"/>
        /// </returns>
        public static IEnumerable<int[]> GetAllRangePermutations (int maxValue)
        {
            int returnValueCount = maxValue + 1;
            int[] permutation = new int[returnValueCount];
            bool[] usedValues = new bool[returnValueCount];

            foreach (int[] returnSet in PermuteRangeValue(permutation, usedValues, maxValue, 0, 0))
            {
                yield return returnSet;
            }
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Returns all the possible permutations of the values in the <paramref name="testSizes"/> array.
        /// </summary>
        /// <param name="testSizes">
        /// The full set of test sizes to use for the permutations.
        /// </param>
        /// <returns>
        /// An enumerable collection of all the possible permutations of the values in the <paramref name="testSizes"/> array.
        /// </returns>
        private static IEnumerable<int[]> GetAllPermutations (int[] testSizes)
        {
            foreach (int[] indexPermutation in GetAllRangePermutations(testSizes.Length - 1))
            {
                int[] returnSizes = new int[indexPermutation.Length];
                for (int i = 0; i < indexPermutation.Length; i++)
                    returnSizes[i] = testSizes[indexPermutation[i]];
                yield return returnSizes;
            }
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets a list of test values for size, length and position in a 
        /// <see cref="MemoryStreamSlim"/> instance used for testing by returning all permutations
        /// of values from different buffer sizes and multiples of the standard buffer sizes.
        /// </summary>
        /// <returns>
        /// An enumerable collection of all permutations of test values.
        /// </returns>
        public IEnumerable<int[]> GetAllBufferSizePermutationValues ()
        {
            // We want to return every possible size from within the small buffer sizes, and 
            // 2 standard buffer sizes. For each set buffer size, we will a random size in that size group.
            int testSizeCount = 2 /* For 2 multiples of standard buffer sizes */
                                + SegmentMemoryStreamSlim.SmallBufferSizes.Length; /* For each small buffer size */
            int[] testSizes = new int[testSizeCount];
            int index = 0;
            for (int bufferSizeIndex = 0; bufferSizeIndex < SegmentMemoryStreamSlim.SmallBufferSizes.Length; bufferSizeIndex++)
            {
                int smallBufferSize = SegmentMemoryStreamSlim.SmallBufferSizes[bufferSizeIndex];
                int rangeMinimum = (bufferSizeIndex == 0) ? 0 : SegmentMemoryStreamSlim.SmallBufferSizes[bufferSizeIndex - 1] + 1;
                testSizes[index++] = GetTestInteger(rangeMinimum, smallBufferSize + 1);
            }
            testSizes[index++] = GetTestInteger(0, MemorySegmentedBufferGroup.StandardBufferSegmentSize + 1);
            testSizes[index] = GetTestInteger(MemorySegmentedBufferGroup.StandardBufferSegmentSize + 1, (MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2) + 1);
            return GetAllPermutations(testSizes);
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets a list of test values for size, length and position in a 
        /// <see cref="MemoryStreamSlim"/> instance used for testing by returning a set of 
        /// values that is built from 2 values within a buffer size range, and a single
        /// value from the next neighboring range.
        /// </summary>
        /// <returns>
        /// An enumerable collection of all permutations of test values.
        /// </returns>
        public IEnumerable<int[]> GetAllNeighborBufferSizePermutationValues ()
        {
            // We want to return every possible size from within the small buffer sizes, and 
            // 2 standard buffer sizes. For each set buffer size, we will a random size in that size group.
            HashSet<int> useSizes = [];
            // Add all the small buffer sizes
            foreach (int smallBufferSize in SegmentMemoryStreamSlim.SmallBufferSizes)
            {
                useSizes.Add(smallBufferSize);
            }
            // Add the max buffer size, the max buffer size * 2, and the max buffer size * 3
            useSizes.Add(MemorySegmentedBufferGroup.StandardBufferSegmentSize);
            useSizes.Add(MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2);
            useSizes.Add(MemorySegmentedBufferGroup.StandardBufferSegmentSize * 3);
            // Now, get all the values in the set
            int[] testSizes = useSizes.ToArray();
            int previousBufferLowValue = 0;
            int[] returnSizes = new int[3];
            for (int bufferSizeIndex = 1; bufferSizeIndex < testSizes.Length; bufferSizeIndex++)
            {
                int previousBufferSize = testSizes[bufferSizeIndex - 1];
                int currentBufferSize = testSizes[bufferSizeIndex];
                int previousBufferMidSize = (previousBufferSize + previousBufferLowValue) / 2;
                // We will get three values, two from the previous buffer size range and one from the current buffer size
                returnSizes[0] = GetTestInteger(previousBufferLowValue, previousBufferMidSize + 1);
                returnSizes[1] = GetTestInteger(previousBufferMidSize + 1, previousBufferSize + 1);
                returnSizes[2] = GetTestInteger(previousBufferSize + 1, currentBufferSize + 1);
                // Now, get all permutations of these values
                foreach (int[] permutation in GetAllPermutations(returnSizes))
                {
                    yield return permutation;
                }

                // Set the low size for the next loop
                previousBufferLowValue = previousBufferSize + 1;
            }
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets a list of test values for size, length and position in a 
        /// <see cref="MemoryStreamSlim"/> instance used for testing.
        /// </summary>
        /// <returns>
        /// An enumerable collection of all permutations of test values.
        /// </returns>
        public IEnumerable<int[]> GetTestStreamSizeValues ()
        {
            yield return [0];
            // Get all the permutations of the buffer sizes.
            foreach (int[] bufferSizePermutationValue in GetAllBufferSizePermutationValues())
            {
                yield return bufferSizePermutationValue;
            }
            // Also, get all the permutations of values from neighboring buffer sizes.
            foreach (int[] bufferSizePermutationValue in GetAllNeighborBufferSizePermutationValues())
            {
                yield return bufferSizePermutationValue;
            }
            yield return [0];
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets the test capacity sizes to use for testing the <see cref="MemoryStreamSlim.Capacity"/> property.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int[]> GetTestCapacitySizes () => GetTestStreamSizeValues();
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets the test length values to use for testing the <see cref="MemoryStreamSlim.Length"/> property.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int[]> GetTestLengthValues () => GetTestStreamSizeValues();
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets the test data sizes to use for theory tests, including some random sizes 
        /// </summary>
        /// <param name="randomCount">
        /// The number of random value to include in the test data sizes.
        /// </param>
        /// <param name="maxRandomSize">
        /// The maximum random size to include in the test data sizes.
        /// </param>
        /// <returns>
        /// An enumerable collection of test data sizes.
        /// </returns>
        public IEnumerable<int> GenerateTestDataSizes (int randomCount, int maxRandomSize)
        {
            // Test the lower values
            yield return 0;
            yield return 1;
            yield return 2;
            // Test numbers around increments of typical segment sizes
            foreach (int incrementValue in TestSegmentSizes)
            {
                int currentSize = incrementValue;
                while (currentSize < MemorySegmentedBufferGroup.StandardBufferSegmentSize)
                {
                    foreach (int neighborSize in GetNeighborDataSizes(currentSize))
                    {
                        yield return neighborSize;
                    }

                    int incrementMultiplier = 8;
                    do
                    {
                        int jumpSize = incrementValue * incrementMultiplier;
                        if ((currentSize + jumpSize) < MemorySegmentedBufferGroup.StandardBufferSegmentSize)
                        {
                            currentSize += jumpSize;
                            goto NextCurrentSizeLoop;
                        }

                        incrementMultiplier--;
                    } while (incrementMultiplier > 1);
                    currentSize += incrementValue;
NextCurrentSizeLoop:;
                }
            }

            foreach (int neighborSize in GetNeighborDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize))
            {
                yield return neighborSize;
            }
            foreach (int neighborSize in GetNeighborDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2))
            {
                yield return neighborSize;
            }

            // Note, we only pass maxRandomSize instead of maxRandomSize + 1 because the random generator uses the maximum
            // value as non-inclusive, but we make sure to add the maximum value to the list of test sizes below.
            int[] testSizes = GetTestIntegerMutableSet(randomCount, 0, maxRandomSize);
            // Make sure we always test the max.
            if (testSizes.Length > 0)
                testSizes[0] = maxRandomSize;
            foreach (int testSize in testSizes)
            {
                yield return testSize;
            }

            // Returns a list of values to use for testing that are in the neighborhood of the
            // <paramref name="baseSize"/> value and the <paramref name="baseSize"/> value itself.
            static IEnumerable<int> GetNeighborDataSizes (int baseSize)
            {
                yield return baseSize;
                yield return baseSize - 1;
                yield return baseSize + 1;
                yield return baseSize - 2;
                yield return baseSize + 2;
            }
        }
        //--------------------------------------------------------------------------------   
        /// <summary>
        /// Gets the test data sizes to use for theory tests, including some random sizes 
        /// </summary>
        /// <param name="randomCount">
        /// The number of random value to include in the test data sizes.
        /// </param>
        /// <param name="maxRandomSize">
        /// The maximum random size to include in the test data sizes.
        /// </param>
        /// <returns>
        /// An enumerable collection of test data sizes returned in a form usable as input to a theory test.
        /// </returns>
        public IEnumerable<object[]> GetFillBufferTestSizes (int randomCount, int maxRandomSize)
        {
            foreach (int dataSize in GenerateTestDataSizes(randomCount, maxRandomSize))
            {
                // Return the data size in a form usable as input to a theory test
                yield return [dataSize];
            }
        }
        //--------------------------------------------------------------------------------   

        //--------------------------------------------------------------------------------

        #region Internal Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream byte by byte
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static void VerifyContentsFromStartToEndByByte (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");
            int byteCount = expectedBytes.Length;
            int bytesLeft = byteCount;
            int readIndex = 0;

            while (bytesLeft > 0)
            {
                int readByte = stream.ReadByte();
                Assert.True(readByte == expectedBytes[readIndex++], "readByte should be equal to the expected byte");
                bytesLeft--;
            }
            Assert.True(stream.Position == byteCount, "Position should be at the end of the stream");
            // Verify that reading past the end of the stream returns -1
            Assert.True(stream.ReadByte() == -1, "Reading past the end of the stream should return -1");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="blockSize">
        /// The size of the block to use for reading the data.
        /// </param>
        protected static void VerifyContentsFromStartToEndInBlocksWithArrayBuffer (Stream stream, byte[] expectedBytes, int blockSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");
            int byteCount = expectedBytes.Length;
            byte[] loadBuffer = new byte[Math.Min(blockSize, byteCount)];
            int bytesLeft = byteCount;

            while (bytesLeft > 0)
            {
                int readCount = stream.Read(loadBuffer, 0, loadBuffer.Length);
                if (readCount <= 0)
                    Assert.Fail($"readCount ({readCount}) should be greater than zero");
                if (readCount > bytesLeft)
                    Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                if (readCount > loadBuffer.Length)
                    Assert.Fail($"readCount ({readCount}) should be less than or equal to buffer length ({loadBuffer.Length})");
                for (int i = 0; i < readCount; i++)
                {
                    if (loadBuffer[i] != expectedBytes[byteCount - bytesLeft + i])
                        Assert.Fail($"the byte at index {i} should be the same as the expected byte");
                }
                bytesLeft -= readCount;
            }
            Assert.True(stream.Position == byteCount, "Position should be at the end of the stream");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="blockSize">
        /// The size of the block to use for reading the data.
        /// </param>
        protected static void VerifyContentsFromStartToEndInBlocks (Stream stream, byte[] expectedBytes, int blockSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");
            int byteCount = expectedBytes.Length;
            byte[] loadBuffer = new byte[Math.Min(blockSize, byteCount)];
            int bytesLeft = byteCount;
            bool useSpanInstance = false;

            while (bytesLeft > 0)
            {
                int readCount;
                // Alternate between using the span instance and the array buffer
                if (useSpanInstance)
                {
                    Span<byte> memory = new(loadBuffer);
                    readCount = stream.Read(loadBuffer, 0, loadBuffer.Length);
                    if (readCount <= 0)
                        Assert.Fail($"readCount ({readCount}) should be greater than zero");
                    if (readCount > bytesLeft)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                    if (readCount > loadBuffer.Length)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to buffer length ({loadBuffer.Length})");
                    for (int i = 0; i < readCount; i++)
                    {
                        if (loadBuffer[i] != expectedBytes[byteCount - bytesLeft + i])
                            Assert.Fail($"the byte at index {i} should be the same as the expected byte");
                    }
                }
                else
                {
                    readCount = stream.Read(loadBuffer, 0, loadBuffer.Length);
                    if (readCount <= 0)
                        Assert.Fail($"readCount ({readCount}) should be greater than zero");
                    if (readCount > bytesLeft)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                    if (readCount > loadBuffer.Length)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to buffer length ({loadBuffer.Length})");
                    for (int i = 0; i < readCount; i++)
                    {
                        if (loadBuffer[i] != expectedBytes[byteCount - bytesLeft + i])
                            Assert.Fail($"the byte at index {i} should be the same as the expected byte");
                    }
                }

                bytesLeft -= readCount;
                useSpanInstance = !useSpanInstance;
            }
            Assert.True(stream.Position == byteCount, "Position should be at the end of the stream");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="blockSize">
        /// The size of the block to use for reading the data.
        /// </param>
        protected static async Task VerifyContentsFromStartToEndInBlocksAsync (Stream stream, byte[] expectedBytes, int blockSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");
            int byteCount = expectedBytes.Length;
            byte[] loadBuffer = new byte[Math.Min(blockSize, byteCount)];
            int bytesLeft = byteCount;
            bool useMemoryInstance = false;

            while (bytesLeft > 0)
            {
                int readCount;
                // Alternate between using the memory instance and the array buffer
                if (useMemoryInstance)
                {
                    Memory<byte> memory = new(loadBuffer);
                    readCount = await stream.ReadAsync(memory);
                    if (readCount <= 0)
                        Assert.Fail($"readCount ({readCount}) should be greater than zero");
                    if (readCount > bytesLeft)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                    if (readCount > loadBuffer.Length)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to buffer length ({loadBuffer.Length})");
                    for (int i = 0; i < readCount; i++)
                    {
                        if (loadBuffer[i] != expectedBytes[byteCount - bytesLeft + i])
                            Assert.Fail($"the byte at index {i} should be the same as the expected byte");
                    }
                }
                else
                {
                    readCount = await stream.ReadAsync(loadBuffer, 0, loadBuffer.Length);
                    if (readCount <= 0)
                        Assert.Fail($"readCount ({readCount}) should be greater than zero");
                    if (readCount > bytesLeft)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                    if (readCount > loadBuffer.Length)
                        Assert.Fail($"readCount ({readCount}) should be less than or equal to buffer length ({loadBuffer.Length})");
                    for (int i = 0; i < readCount; i++)
                    {
                        if (loadBuffer[i] != expectedBytes[byteCount - bytesLeft + i])
                            Assert.Fail($"the byte at index {i} should be the same as the expected byte");
                    }
                }
                bytesLeft -= readCount;
                useMemoryInstance = !useMemoryInstance;
            }
            Assert.True(stream.Position == byteCount, "Position should be at the end of the stream");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static void VerifyContentsFromStartToEndInBlocks (Stream stream, byte[] expectedBytes) =>
            VerifyContentsFromStartToEndInBlocks(stream, expectedBytes, 0x10);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static Task VerifyContentsFromStartToEndInBlocksAsync (Stream stream, byte[] expectedBytes) =>
            VerifyContentsFromStartToEndInBlocksAsync(stream, expectedBytes, 0x10);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static void VerifyContentsFromStartToEndOneRead (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");
            int byteCount = expectedBytes.Length;
            // Make the buffer bigger than the byte count to ensure trying to read more bytes than are available
            // will not cause an exception and return the correct number of bytes read.
            byte[] loadBuffer = new byte[byteCount + 0x10];

            int readCount = stream.Read(loadBuffer, 0, loadBuffer.Length);
            Assert.True(readCount == byteCount, "readCount should be equal to byte count, because we should be able to read the entire set of bytes written to the stream");
            if (readCount > byteCount)
                Assert.Fail($"readCount ({readCount}) should be less than or equal to expected bytes length ({byteCount})");
            Assert.True(stream.Position == byteCount, "Position should be at the end of the stream");
            if (loadBuffer[..byteCount].SequenceEqual(expectedBytes))
                return;
            // Not equal, find the first index that is not equal for the assertion message
            for (int checkIndex = 0; checkIndex < byteCount; checkIndex++)
            {
                if (loadBuffer[checkIndex] != expectedBytes[checkIndex])
                    Assert.Fail($"we should get the exact same byte values back as were written to the stream, the byte at index {checkIndex} is the first different byte");
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance by reading 
        /// and comparing the bytes from the start to the end of the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static async Task VerifyContentsFromStartToEndOneReadAsync (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");
            int byteCount = expectedBytes.Length;
            // Make the buffer bigger than the byte count to ensure trying to read more bytes than are available
            // will not cause an exception and return the correct number of bytes read.
            byte[] loadBuffer = new byte[byteCount + 0x10];

            int readCount = await stream.ReadAsync(loadBuffer, 0, loadBuffer.Length);
            Assert.True(readCount == byteCount, "readCount should be equal to byte count, because we should be able to read the entire set of bytes written to the stream");
            if (readCount > byteCount)
                Assert.Fail($"readCount ({readCount}) should be less than or equal to expected bytes length ({byteCount})");
            Assert.True(stream.Position == byteCount, "Position should be at the end of the stream");
            if (loadBuffer[..byteCount].SequenceEqual(expectedBytes))
                return;
            Assert.Fail("we should get the exact same byte values back as were written to the stream");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static void VerifyContents (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            // Verify various different ways of reading the stream
            VerifyContentsFromStartToEndOneRead(stream, expectedBytes);
            VerifyContentsFromStartToEndInBlocks(stream, expectedBytes);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static async Task VerifyContentsAsync (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            // Verify various different ways of reading the stream
            await VerifyContentsFromStartToEndOneReadAsync(stream, expectedBytes);
            await VerifyContentsFromStartToEndInBlocksAsync(stream, expectedBytes);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="bySegmentSize">
        /// The size of each segment used to read the data.
        /// </param>
        protected static void VerifyContents (Stream stream, byte[] expectedBytes, int bySegmentSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            // Verify various different ways of reading the stream
            VerifyContentsFromStartToEndOneRead(stream, expectedBytes);
            VerifyContentsFromStartToEndInBlocks(stream, expectedBytes, bySegmentSize);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the contents of a <see cref="MemoryStreamSlim"/> instance.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="bySegmentSize">
        /// The size of each segment used to read the data.
        /// </param>
        protected static async Task VerifyContentsAsync (Stream stream, byte[] expectedBytes, int bySegmentSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            // Verify various different ways of reading the stream
            await VerifyContentsFromStartToEndOneReadAsync(stream, expectedBytes);
            await VerifyContentsFromStartToEndInBlocksAsync(stream, expectedBytes, bySegmentSize);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the operations of a <see cref="MemoryStreamSlim"/> instance by running
        /// operations with other streams and verifying the contents are correct.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static void VerifyStreamOperations (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            MemoryStream copyToStream = new MemoryStream();
            stream.CopyTo(copyToStream);
            copyToStream.Position = 0;
            byte[] copyToBytes = new byte[expectedBytes.Length + 0x10];
            Assert.Equal(copyToStream.Read(copyToBytes, 0, copyToBytes.Length), expectedBytes.Length);
            Assert.True(stream.Position == expectedBytes.Length, "Position should be at the end of the stream");
            if (copyToBytes[..expectedBytes.Length].SequenceEqual(expectedBytes))
                return;
            Assert.Fail("the bytes should be the same as the expected bytes");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the operations of a <see cref="MemoryStreamSlim"/> instance by running
        /// operations with other streams and verifying the contents are correct.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        protected static async Task VerifyStreamOperationsAsync (Stream stream, byte[] expectedBytes)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            MemoryStream copyToStream = new MemoryStream();
            await stream.CopyToAsync(copyToStream);
            copyToStream.Position = 0;
            byte[] copyToBytes = new byte[expectedBytes.Length + 0x10];
            Assert.Equal(await copyToStream.ReadAsync(copyToBytes, 0, copyToBytes.Length), expectedBytes.Length);
            Assert.True(stream.Position == expectedBytes.Length, "Position should be at the end of the stream");
            if (copyToBytes[..expectedBytes.Length].SequenceEqual(expectedBytes))
                return;
            Assert.Fail("the bytes should be the same as the expected bytes");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the operations of a <see cref="MemoryStreamSlim"/> instance by running
        /// operations with other streams and verifying the contents are correct.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write and read the data.
        /// </param>
        protected static void VerifyStreamOperations (Stream stream, byte[] expectedBytes, int bySegmentSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            MemoryStream copyToStream = new MemoryStream();
            stream.CopyTo(copyToStream, 0x1000);

            int byteIndex = 0;
            copyToStream.Position = 0;
            Assert.Equal(copyToStream.Length, expectedBytes.Length);
            byte[] copyToBytes = new byte[expectedBytes.Length];
            int bytesLeft = expectedBytes.Length;
            while (bytesLeft > 0)
            {
                int bytesToRead = Math.Min(bySegmentSize, bytesLeft);
                int readCount = copyToStream.Read(copyToBytes, byteIndex, bytesToRead);
                if (readCount <= 0)
                    Assert.Fail($"readCount ({readCount}) should be greater than zero");
                if (readCount > bytesLeft)
                    Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                bytesLeft -= readCount;
                byteIndex += readCount;
            }
            Assert.True(stream.Position == expectedBytes.Length, "Position should be at the end of the stream");
            if (copyToBytes.SequenceEqual(expectedBytes))
                return;
            Assert.Fail("the bytes should be the same as the expected bytes");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Verifies the operations of a <see cref="MemoryStreamSlim"/> instance by running
        /// operations with other streams and verifying the contents are correct.
        /// </summary>
        /// <param name="stream">
        /// The stream instance to verify.
        /// </param>
        /// <param name="expectedBytes">
        /// A copy of the bytes that are expected to be in the stream.
        /// </param>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write and read the data.
        /// </param>
        protected static async Task VerifyStreamOperationsAsync (Stream stream, byte[] expectedBytes, int bySegmentSize)
        {
            stream.Position = 0;
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            MemoryStream copyToStream = new MemoryStream();
            await stream.CopyToAsync(copyToStream, 0x1000);

            int byteIndex = 0;
            copyToStream.Position = 0;
            Assert.Equal(copyToStream.Length, expectedBytes.Length);
            byte[] copyToBytes = new byte[expectedBytes.Length];
            int bytesLeft = expectedBytes.Length;
            while (bytesLeft > 0)
            {
                int bytesToRead = Math.Min(bySegmentSize, bytesLeft);
                int readCount = await copyToStream.ReadAsync(copyToBytes, byteIndex, bytesToRead);
                if (readCount <= 0)
                    Assert.Fail($"readCount ({readCount}) should be greater than zero");
                if (readCount > bytesLeft)
                    Assert.Fail($"readCount ({readCount}) should be less than or equal to bytesLeft ({bytesLeft})");
                bytesLeft -= readCount;
                byteIndex += readCount;
            }
            Assert.True(stream.Position == expectedBytes.Length, "Position should be at the end of the stream");
            if (copyToBytes.SequenceEqual(expectedBytes))
                return;
            Assert.Fail("the bytes should be the same as the expected bytes");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests writing a byte array to a <see cref="MemoryStreamSlim"/> and verifying the
        /// bytes when reading them back.
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream instance being tested.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream and verify.
        /// </param>
        protected static async Task TestWriteAndVerifyBytesAsync (IRandomSource randomSource,
            Stream stream, int byteCount)
        {
            // Reset the stream
            stream.Position = 0;
            stream.SetLength(0);
            // Verify the length and the position
            Assert.True(stream.Length == 0, "Length should be 0 to start");
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            // Fill it with random bytes
            byte[] dataCopy =
                await MemoryTestPrep.FillStreamAndArrayWithRandomBytesWithYieldAsync(randomSource, stream, byteCount);

            // Verify the length and the position
            Assert.True(stream.Length == byteCount, "Length should be equal to byte count");
            Assert.True(stream.Position == byteCount, "Position should equal to byte count");

            // Verify the contents
            await VerifyContentsAsync(stream, dataCopy);
            // Verify copying to another stream
            await VerifyStreamOperationsAsync(stream, dataCopy);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests writing a byte array to a <see cref="MemoryStreamSlim"/> and verifying the
        /// bytes when reading them back.
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream instance being tested.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream and verify.
        /// </param>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write and read the data.
        /// </param>
        protected static async Task TestWriteAndVerifyBytesAsync (IRandomSource randomSource,
            Stream stream, int byteCount, int bySegmentSize)
        {
            // Reset the stream
            stream.Position = 0;
            stream.SetLength(0);
            // Verify the length and the position
            Assert.True(stream.Length == 0, "Length should be 0 to start");
            Assert.True(stream.Position == 0, "Position should be 0 to start");

            // Fill it with random bytes
            byte[] dataCopy = await MemoryTestPrep.FillStreamAndArrayWithRandomBytesWithYieldAsync(randomSource, stream, byteCount, bySegmentSize);

            // Verify the length and the position
            Assert.True(stream.Length == byteCount, "Length should be equal to byte count");
            Assert.True(stream.Position == byteCount, "Position should equal to byte count");

            // Verify the contents
            await VerifyContentsAsync(stream, dataCopy, bySegmentSize);
            // Verify copying to another stream
            await VerifyStreamOperationsAsync(stream, dataCopy, bySegmentSize);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Helper method that tests writing a byte array to a <see cref="MemoryStreamSlim"/>
        /// and verifying the bytes when reading them back.
        /// </summary>
        /// <typeparam name="TState">
        /// State data type to pass to the MemoryStreamSlim options setup method.
        /// </typeparam>
        /// <param name="dataSize">
        /// The size of the data stream to write and verify.
        /// </param>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="optionsSetup">
        /// Setup delegate to configure the options for the <see cref="MemoryStreamSlim"/> instance.
        /// </param>
        /// <param name="optionsSetupState">
        /// State data to pass to the options setup method.
        /// </param>
        protected static async Task TestStreamWriteAndVerifyContentAsync<TState> (IRandomSource randomSource,
            int dataSize, Action<MemoryStreamSlimOptions, TState> optionsSetup,
            TState optionsSetupState)
        {
            await using MemoryStreamSlim testService = MemoryStreamSlim.Create(optionsSetup, optionsSetupState);
            await TestWriteAndVerifyBytesAsync(randomSource, testService, dataSize);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Helper method that tests writing a byte array to a <see cref="MemoryStreamSlim"/>
        /// and verifying the bytes when reading them back.
        /// </summary>
        /// <typeparam name="TState">
        /// State data type to pass to the MemoryStreamSlim options setup method.
        /// </typeparam>
        /// <param name="dataSize">
        /// The size of the data stream to write and verify.
        /// </param>
        /// <param name="bySegmentSize">
        /// The size of the segment that will be used for the tests
        /// </param>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="optionsSetup">
        /// Setup delegate to configure the options for the <see cref="MemoryStreamSlim"/> instance.
        /// </param>
        /// <param name="optionsSetupState">
        /// State data to pass to the options setup method.
        /// </param>
        protected static async Task TestStreamWriteAndVerifyContentAsync<TState> (IRandomSource randomSource,
            int dataSize, int bySegmentSize, Action<MemoryStreamSlimOptions, TState> optionsSetup,
            TState optionsSetupState)
        {
            await using MemoryStreamSlim testService = MemoryStreamSlim.Create(optionsSetup, optionsSetupState);
            await TestWriteAndVerifyBytesAsync(randomSource, testService, dataSize, bySegmentSize);
        }
        //--------------------------------------------------------------------------------

        //================================================================================

        #endregion Internal Test Methods

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSlimUnitTestBase"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        protected UsingMemoryStreamSlimUnitTestBase (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
