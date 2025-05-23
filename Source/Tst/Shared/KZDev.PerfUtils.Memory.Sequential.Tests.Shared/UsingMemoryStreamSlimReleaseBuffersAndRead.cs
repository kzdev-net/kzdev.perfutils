﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Tests;

    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class that never run in 
    /// parallel with other tests.
    /// </summary>
    public class UsingMemoryStreamSlimReleaseBuffersAndRead : UsingMemoryStreamSlimUnitTestBase
    {
        /// <summary>
        /// The minimum number of test loops to run for the tests.
        /// </summary>
        private const int MinimumTestLoops = 100;
        /// <summary>
        /// The maximum number of test loops to run for the tests.
        /// </summary>
        private const int MaximumTestLoops = 500;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSlimReleaseBuffersAndRead"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamSlimReleaseBuffersAndRead (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and reading it back using the Read method in segments
        /// and verifying that each byte read is the same as the byte written. This is then done
        ///  a second time after the memory has been released.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_ReadBySegments_AfterMemoryRelease_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            // Fill the stream with random bytes
            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    await using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.OnRelease));
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // ReSharper disable once MethodHasAsyncOverload
                    byte[] dataCopy = MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    // Pick an unusual block size
                    await VerifyContentsFromStartToEndInBlocksAsync(testService, dataCopy, 0x47);
                }

            // Release the memory buffers
            MemoryStreamSlim.ReleaseMemoryBuffers();

            // Run the test loops again.
            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    await using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.OnRelease));
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // ReSharper disable once MethodHasAsyncOverload
                    byte[] dataCopy = MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    // Pick an unusual block size
                    await VerifyContentsFromStartToEndInBlocksAsync(testService, dataCopy, 0x47);
                }
        }
        //--------------------------------------------------------------------------------    

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
