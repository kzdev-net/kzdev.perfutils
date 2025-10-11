// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Tests;

namespace KZDev.PerfUtils.Common.UnitTests;

//################################################################################
/// <summary>
/// Unit tests for the <see cref="UtilityHelpers"/> class.
/// </summary>
public class UsingUtilityHelpers : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingUtilityHelpers"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingUtilityHelpers(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Test Methods

    //================================================================================

    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndex_OnSmallArrays_GetsCorrectIndex()
    {
        int useArraySize = UtilityHelpers.LargeArraySize - 1;

        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            LoopStart:
            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Ensure that no two values in adjacent indexes are adjacent values
            for (int checkIndex = 1; checkIndex < bufferSizeArray.Length; checkIndex++)
            {
                if (bufferSizeArray[checkIndex] - bufferSizeArray[checkIndex - 1] == 1)
                {
                    goto LoopStart;
                }
            }

            // Pick a test index in the array.
            int testIndex = GetTestInteger(0, useArraySize);

            try
            {
                // Get the buffer size at the test index.
                int bufferSize = bufferSizeArray[testIndex];

                // Get the index of the buffer size using the sequential walk method.
                int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one less than the current buffer size.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one more than the previous buffer size.
                int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                // Get the index of the buffer size using the sequential walk method.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);
            }
            catch
            {
                TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                TestWriteLine($"Test index: {testIndex}");
                throw;
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndexForEachIndex_OnSmallArrays_GetsCorrectIndex()
    {
        int useArraySize = UtilityHelpers.LargeArraySize - 1;

        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            LoopStart:
            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Ensure that no two values in adjacent indexes are adjacent values
            for (int checkIndex = 1; checkIndex < bufferSizeArray.Length; checkIndex++)
            {
                if (bufferSizeArray[checkIndex] - bufferSizeArray[checkIndex - 1] == 1)
                {
                    goto LoopStart;
                }
            }

            for (int testIndex = 0; testIndex < bufferSizeArray.Length; testIndex++)
            {
                try
                {
                    // Get the buffer size at the test index.
                    int bufferSize = bufferSizeArray[testIndex];

                    // Get the index of the buffer size using the sequential walk method.
                    int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);

                    // Now, also test if the buffer size is one less than the current buffer size.
                    bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);

                    // Now, also test if the buffer size is one more than the previous buffer size.
                    int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                    // Get the index of the buffer size using the sequential walk method.
                    bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);
                }
                catch
                {
                    TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                    TestWriteLine($"Test index: {testIndex}");
                    throw;
                }
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of adjacent buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndex_OnSmallArraysOfAdjacentSizes_GetsCorrectIndex()
    {
        int useArraySize = UtilityHelpers.LargeArraySize - 1;

        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerSequenceMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Pick a test index in the array.
            int testIndex = GetTestInteger(1, useArraySize);

            try
            {
                // Get the buffer size at the test index.
                int bufferSize = bufferSizeArray[testIndex];

                // Get the index of the buffer size using the sequential walk method.
                int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one less than the current buffer size.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex - 1);

                // Now, also test if the buffer size is one more than the previous buffer size.
                int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                // Get the index of the buffer size using the sequential walk method.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);
            }
            catch
            {
                TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                TestWriteLine($"Test index: {testIndex}");
                throw;
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of adjacent buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndexForEachIndex_OnSmallArraysOfAdjacentSizes_GetsCorrectIndex()
    {
        int useArraySize = UtilityHelpers.LargeArraySize - 1;

        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerSequenceMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            for (int testIndex = 1; testIndex < bufferSizeArray.Length; testIndex++)
            {
                try
                {
                    // Get the buffer size at the test index.
                    int bufferSize = bufferSizeArray[testIndex];

                    // Get the index of the buffer size using the sequential walk method.
                    int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);

                    // Now, also test if the buffer size is one less than the current buffer size.
                    bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex - 1);

                    // Now, also test if the buffer size is one more than the previous buffer size.
                    int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                    // Get the index of the buffer size using the sequential walk method.
                    bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);
                }
                catch
                {
                    TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                    TestWriteLine($"Test index: {testIndex}");
                    throw;
                }
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndex_OnLargeArrays_GetsCorrectIndex()
    {
        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            LoopStart:
            int useArraySize = GetTestInteger(UtilityHelpers.LargeArraySize, UtilityHelpers.LargeArraySize * 3);

            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Ensure that no two values in adjacent indexes are adjacent values
            for (int checkIndex = 1; checkIndex < bufferSizeArray.Length; checkIndex++)
            {
                if (bufferSizeArray[checkIndex] - bufferSizeArray[checkIndex - 1] == 1)
                {
                    goto LoopStart;
                }
            }

            // Pick a test index in the array.
            int testIndex = GetTestInteger(0, useArraySize);

            try
            {
                // Get the buffer size at the test index.
                int bufferSize = bufferSizeArray[testIndex];

                // Get the index of the buffer size using the sequential walk method.
                int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one less than the current buffer size.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one more than the previous buffer size.
                int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                // Get the index of the buffer size using the sequential walk method.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);
            }
            catch
            {
                TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                TestWriteLine($"testIndex index: {testIndex}");
                throw;
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndexForEachIndex_OnLargeArrays_GetsCorrectIndex()
    {
        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            LoopStart:
            int useArraySize = GetTestInteger(UtilityHelpers.LargeArraySize, UtilityHelpers.LargeArraySize * 3);

            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Ensure that no two values in adjacent indexes are adjacent values
            for (int checkIndex = 1; checkIndex < bufferSizeArray.Length; checkIndex++)
            {
                if (bufferSizeArray[checkIndex] - bufferSizeArray[checkIndex - 1] == 1)
                {
                    goto LoopStart;
                }
            }

            for (int testIndex = 0; testIndex < bufferSizeArray.Length; testIndex++)
            {
                try
                {
                    // Get the buffer size at the test index.
                    int bufferSize = bufferSizeArray[testIndex];

                    // Get the index of the buffer size using the sequential walk method.
                    int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);

                    // Now, also test if the buffer size is one less than the current buffer size.
                    bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);

                    // Now, also test if the buffer size is one more than the previous buffer size.
                    int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                    // Get the index of the buffer size using the sequential walk method.
                    bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                    // The index should be the same as the selected test index.
                    bufferSizeIndex.Should().Be(testIndex);
                }
                catch
                {
                    TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                    TestWriteLine($"Test index: {testIndex}");
                    throw;
                }
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of adjacent buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndex_OnLargeArraysOfAdjacentSizes_GetsCorrectIndex()
    {
        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            int useArraySize = GetTestInteger(UtilityHelpers.LargeArraySize, UtilityHelpers.LargeArraySize * 3);

            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerSequenceMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Pick a test index in the array.
            int testIndex = GetTestInteger(1, useArraySize);

            try
            {
                // Get the buffer size at the test index.
                int bufferSize = bufferSizeArray[testIndex];

                // Get the index of the buffer size using the sequential walk method.
                int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one less than the current buffer size.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex - 1);

                // Now, also test if the buffer size is one more than the previous buffer size.
                int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                // Get the index of the buffer size using the sequential walk method.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);
            }
            catch
            {
                TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                TestWriteLine($"Test index: {testIndex}");
                throw;
            }
        }
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests a series of adjacent buffer size arrays and buffer sizes to be sure that the correct
    /// index is found when using the sequential walk method.
    /// </summary>
    [Fact]
    public void UsingUtilityHelpers_GetBufferSizeIndexForEachIndex_OnLargeArraysOfAdjacentSizes_GetsCorrectIndex()
    {
        // Run the test many times
        for (int loopIndex = 0; loopIndex < 100_000; loopIndex++)
        {
            int useArraySize = GetTestInteger(UtilityHelpers.LargeArraySize, UtilityHelpers.LargeArraySize * 3);

            // Get a random buffer size array with a minimum size of 5.
            int[] bufferSizeArray = GetTestIntegerSequenceMutableSet(useArraySize, 5, int.MaxValue);

            // Sort the array in ascending order.
            Array.Sort(bufferSizeArray);

            // Pick a test index in the array.
            int testIndex = GetTestInteger(1, useArraySize);

            try
            {
                // Get the buffer size at the test index.
                int bufferSize = bufferSizeArray[testIndex];

                // Get the index of the buffer size using the sequential walk method.
                int bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);

                // Now, also test if the buffer size is one less than the current buffer size.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, bufferSize - 1);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex - 1);

                // Now, also test if the buffer size is one more than the previous buffer size.
                int testBufferSize = (testIndex == 0) ? 1 : bufferSizeArray[testIndex - 1] + 1;

                // Get the index of the buffer size using the sequential walk method.
                bufferSizeIndex = UtilityHelpers.GetBufferSizeIndex(bufferSizeArray, testBufferSize);

                // The index should be the same as the selected test index.
                bufferSizeIndex.Should().Be(testIndex);
            }
            catch
            {
                TestWriteLine($"Buffer size array: {string.Join(", ", bufferSizeArray)}");
                TestWriteLine($"Test index: {testIndex}");
                throw;
            }
        }
    }
    //--------------------------------------------------------------------------------    

    //================================================================================

    #endregion Test Methods
}
//################################################################################
