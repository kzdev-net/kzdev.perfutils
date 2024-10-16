// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.ExceptionServices;

using FluentAssertions;

using KZDev.PerfUtils.Internals;

using static KZDev.PerfUtils.Tests.FileStreamMock;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    public partial class UsingMemoryStreamSlim
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Static constructor for the <see cref="UsingMemoryStreamSlim"/> class.
        /// </summary>
        static UsingMemoryStreamSlim ()
        {
#if NATIVEMEMORY
            MemoryStreamSlim.UseNativeLargeMemoryBuffers = true;
#else
            MemoryStreamSlim.UseNativeLargeMemoryBuffers = false;
#endif
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Test helper to instantiate a new <see cref="MemoryStreamSlim"/> object as the test subject service.
        /// </summary>
        private Func<int, MemoryStreamSlim> CreateTestService { get; } =
            // By default, we don't zero the buffer on release because we want to capture any possible buffer corruption
            // between uses
            _ => MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and reading it back using the ReadByte method,
        /// and verifying that each byte read is the same as the byte written.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public void UsingMemoryStreamSlim_ReadByByte_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy = MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    VerifyContentsFromStartToEndByByte(testService, dataCopy);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream using the WriteByte method and reading it back 
        /// and verifying that each byte read is the same as the byte written.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public void UsingMemoryStreamSlim_WriteByByte_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            // Fill the stream with random bytes
            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // ReSharper disable once MethodHasAsyncOverload
                    byte[] fillData = MemoryTestPrep.GetRandomByteArray(byteCount);
                    for (int i = 0; i < fillData.Length; i++)
                        testService.WriteByte(fillData[i]);
                    VerifyContentsFromStartToEndOneRead(testService, fillData);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and reading it back using the Read method in segments
        /// and verifying that each byte read is the same as the byte written.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_ReadBySegments_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy = MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    // Pick an unusual block size
                    VerifyContentsFromStartToEndInBlocks(testService, dataCopy, 0x47);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and reading it back using the Read method in segments
        /// and verifying that each byte read is the same as the byte written.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_ReadBySegments_OnManyThreads_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            ExceptionDispatchInfo? taskException = null;

            Task[] runningTasks = Enumerable.Range(0, Math.Max(8, Environment.ProcessorCount * 2)).Select(_ => Task.Run(() =>
            {
                try
                {
                    foreach (int testSegmentSize in TestSegmentSizes)
                        for (int testLoop = 0; testLoop < testLoops; testLoop++)
                        {
                            int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                            using MemoryStreamSlim testService = CreateTestService(byteCount);
                            TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                            // Fill the stream with random bytes
                            // ReSharper disable once MethodHasAsyncOverload
                            byte[] dataCopy = MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                            // Pick an unusual block size
                            VerifyContentsFromStartToEndInBlocks(testService, dataCopy, 0x47);
                        }
                }
                catch (Exception error)
                {
                    taskException ??= ExceptionDispatchInfo.Capture(error);
                }
            })).ToArray();
            await Task.WhenAll(runningTasks);
            taskException?.Throw();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and reading it back using the Read method in segments
        /// and verifying that each byte read is the same as the byte written.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_ReadBySegmentsAsync_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    // Pick an unusual block size
                    await VerifyContentsFromStartToEndInBlocksAsync(testService, dataCopy, 0x47);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests repeatedly writing data to the stream, repositioning the stream, and writing 
        /// more data to the stream, and then verifying that the data read back is the same as the
        /// original data written.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public void UsingMemoryStreamSlim_RepeatedWriteAndPosition_KeepsDataConsistent ()
        {
            int[] testDataSizes = GenerateTestDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 20).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumRandomPositionTestLoops, MaximumRandomPositionTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy = MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    for (int loopIndex = 0; loopIndex < 100; loopIndex++)
                    {
                        testService.Position = loopIndex switch
                        {
                            0 => 0,
                            1 => byteCount / 2,
                            2 => Math.Min(byteCount, MemorySegmentedBufferGroup.StandardBufferSegmentSize),
                            3 => Math.Max(0, byteCount - MemorySegmentedBufferGroup.StandardBufferSegmentSize),
                            4 => Math.Min(byteCount, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2),
                            5 => Math.Min(byteCount, MemorySegmentedBufferGroup.StandardBufferSegmentSize / 2),
                            _ => (byteCount < 2) ? byteCount : RandomSource.GetRandomInteger(byteCount)
                        };
                        // Write the data again from this point
                        testService.Write(dataCopy, (int)testService.Position, dataCopy.Length - (int)testService.Position);
                    }
                    // Verify the contents
                    VerifyContentsFromStartToEndOneRead(testService, dataCopy);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests repeatedly writing data to the stream in chunks that are multiples (or 1/2) of the
        /// standard segment size, and then verifying that the data read back is the same as the
        /// original data written.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_WriteInStandardSegmentSizeMultiples_WritesCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 20).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            for (int testLoop = 0; testLoop < testLoops; testLoop++)
            {
                int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                using MemoryStreamSlim testService = CreateTestService(byteCount);
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount}");

                // Get the random test bytes
                byte[] dataCopy = MemoryTestPrep.GetRandomByteArray(byteCount);
                int bytesLeft = byteCount;
                int writeOffset = 0;
                int loopCount = 0;
                while (bytesLeft > 0)
                {
                    int writeCount = Math.Min(bytesLeft, (loopCount++ % 3) switch
                    {
                        0 => MemorySegmentedBufferGroup.StandardBufferSegmentSize,
                        1 => MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2,
                        _ => MemorySegmentedBufferGroup.StandardBufferSegmentSize / 2
                    });
                    testService.Write(dataCopy, writeOffset, writeCount);
                    bytesLeft -= writeCount;
                    writeOffset += writeCount;
                }
                // Verify the contents
                VerifyContentsFromStartToEndOneRead(testService, dataCopy);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream in chunks and then jumping the position in steps
        /// that are multiples (or 1/2) of the standard segment size, and then verifying that the
        /// resulting data read back is the same as the original data written including with
        /// gaps of zeroes.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_WriteWithStandardSegmentSizeMultipleJumps_WritesCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 20).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumRandomPositionTestLoops, MaximumRandomPositionTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Get the random test bytes
                    byte[] dataCopy = MemoryTestPrep.GetRandomByteArray(byteCount);
                    // For fixed mode streams, we need the original data that will be used for the gaps instead of zeroes
                    byte[]? originalData = (testService.Mode == MemoryStreamSlimMode.Fixed) ? testService.ToArray() : null;
                    int bytesLeft = byteCount;
                    int writeOffset = 0;
                    while (bytesLeft > 0)
                    {
                        int writeCount = Math.Min(bytesLeft, testSegmentSize);
                        testService.Write(dataCopy, writeOffset, writeCount);
                        bytesLeft -= writeCount;
                        if (0 == bytesLeft)
                            break;
                        writeOffset += writeCount;
                        // Now, jump a multiple of the standard segment size
                        int jumpToMultiple = (GetTestInteger() % 4) switch
                        {
                            0 => MemorySegmentedBufferGroup.StandardBufferSegmentSize,
                            1 => MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2,
                            2 => MemorySegmentedBufferGroup.StandardBufferSegmentSize * 3,
                            _ => MemorySegmentedBufferGroup.StandardBufferSegmentSize / 2
                        };
                        // Get the target jump position
                        int targetJumpTo = writeOffset + jumpToMultiple;
                        // Adjust so that we land right on the multiple
                        int targetMultipleOf = Math.DivRem(targetJumpTo, jumpToMultiple, out int remainder);
                        if (remainder != 0)
                            targetJumpTo = targetMultipleOf * jumpToMultiple;
                        // Where will we really jump to?
                        int newWriteOffset = Math.Min(targetJumpTo, byteCount);
                        // Adjust the bytes left
                        bytesLeft -= (newWriteOffset - writeOffset);
                        // Jump to the new position
                        testService.Position = newWriteOffset;
                        // If we have a fixed array and original data contents, then copy those bytes into our comparison array
                        if (originalData is not null)
                            Array.Copy(originalData, writeOffset, dataCopy, writeOffset, newWriteOffset - writeOffset);
                        // If we are about to break out of the loop, then set the new stream length
                        if (bytesLeft <= 0)
                            testService.SetLength(newWriteOffset);
                        if (originalData is null)
                            // Fill in the source data array gap with zeroes
                            Array.Clear(dataCopy, writeOffset, newWriteOffset - writeOffset);
                        // Set the new write offset
                        writeOffset = newWriteOffset;
                    }
                    // Verify the contents
                    VerifyContentsFromStartToEndOneRead(testService, dataCopy);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream in chunks and then jumping the position in steps
        /// that random, and then verifying that the resulting data read back is the same 
        /// as the original data written including with gaps of zeroes.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_WriteWithRandomJumps_WritesCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 20).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumRandomPositionTestLoops, MaximumRandomPositionTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Get the random test bytes
                    byte[] dataCopy = MemoryTestPrep.GetRandomByteArray(byteCount);
                    // For fixed mode streams, we need the original data that will be used for the gaps instead of zeroes
                    byte[]? originalData = (testService.Mode == MemoryStreamSlimMode.Fixed) ? testService.ToArray() : null;
                    int bytesLeft = byteCount;
                    int writeOffset = 0;
                    while (bytesLeft > 0)
                    {
                        int writeCount = Math.Min(bytesLeft, testSegmentSize);
                        testService.Write(dataCopy, writeOffset, writeCount);
                        bytesLeft -= writeCount;
                        if (0 == bytesLeft)
                            break;
                        writeOffset += writeCount;
                        // Now, jump a multiple of the standard segment size
                        int jumpToMultiple = RandomSource.GetRandomInteger(1, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2);
                        // Get the target jump position
                        int targetJumpTo = writeOffset + jumpToMultiple;
                        // Adjust so that we land right on the multiple
                        int targetMultipleOf = Math.DivRem(targetJumpTo, jumpToMultiple, out int remainder);
                        if (remainder != 0)
                            targetJumpTo = targetMultipleOf * jumpToMultiple;
                        // Where will we really jump to?
                        int newWriteOffset = Math.Min(targetJumpTo, byteCount);
                        // Adjust the bytes left
                        bytesLeft -= (newWriteOffset - writeOffset);
                        // Jump to the new position
                        testService.Position = newWriteOffset;
                        // If we have a fixed array and original data contents, then copy those bytes into our comparison array
                        if (originalData is not null)
                            Array.Copy(originalData, writeOffset, dataCopy, writeOffset, newWriteOffset - writeOffset);
                        // If we are about to break out of the loop, then set the new stream length
                        if (bytesLeft <= 0)
                            testService.SetLength(newWriteOffset);
                        if (originalData is null)
                            // Fill in the source data array gap with zeroes
                            Array.Clear(dataCopy, writeOffset, newWriteOffset - writeOffset);
                        // Set the new write offset
                        writeOffset = newWriteOffset;
                    }
                    // Verify the contents
                    VerifyContentsFromStartToEndOneRead(testService, dataCopy);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and getting it back as an array using the ToArray 
        /// method and verifying that the array is the same as the data written.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.LongRun)]
        public async Task UsingMemoryStreamSlim_GetArray_ReturnsCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    // Get the array and verify it
                    byte[] returnedArray = testService.ToArray();
                    dataCopy.Should().BeEquivalentTo(returnedArray);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and verifying that the contents of the other stream
        /// is identical to the data written.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CopyFullToStream_CopiesAllData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    // Copy to the memory stream
                    testService.Seek(0, SeekOrigin.Begin);
                    testService.CopyTo(memoryStream);
                    testService.Position.Should().Be(testService.Length);
                    testService.Length.Should().Be(byteCount);
                    memoryStream.Position.Should().Be(memoryStream.Length);
                    memoryStream.Length.Should().Be(byteCount);
                    // Verify the contents
                    VerifyContentsFromStartToEndInBlocks(memoryStream, dataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and verifying that the contents of the other stream
        /// is identical to the data written.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyFullToStream_CopiesAllDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    // Copy to the memory stream
                    testService.Seek(0, SeekOrigin.Begin);
                    await testService.CopyToAsync(memoryStream);
                    testService.Position.Should().Be(testService.Length);
                    testService.Length.Should().Be(byteCount);
                    memoryStream.Position.Should().Be(memoryStream.Length);
                    memoryStream.Length.Should().Be(byteCount);
                    // Verify the contents
                    await VerifyContentsFromStartToEndInBlocksAsync(memoryStream, dataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the asynchronous stream and verifying that the contents of the other stream
        /// is identical to the data written.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyFullToAsyncStream_CopiesAllDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    // Emulate asynchronous delays for writes to this stream
                    await using FileStreamMock fileStream = new FileStreamMock();
                    // Copy to the memory stream
                    testService.Seek(0, SeekOrigin.Begin);
                    await testService.CopyToAsync(fileStream);
                    testService.Position.Should().Be(testService.Length);
                    testService.Length.Should().Be(byteCount);
                    fileStream.Position.Should().Be(fileStream.Length);
                    fileStream.Length.Should().Be(byteCount);
                    // Verify the contents - suspend any async delays
                    using SuspendFileStreamAsyncDelay verifySuspendScope = fileStream.SuspendAsyncDelay();
                    await VerifyContentsFromStartToEndInBlocksAsync(fileStream, dataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the asynchronous stream and verifying that the contents of the other stream
        /// is identical to the data written.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyFullToAsyncStream_OnManyThreads_CopiesAllDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            ExceptionDispatchInfo? taskException = null;


            Task[] runningTasks = Enumerable.Range(0, Math.Max(8, Environment.ProcessorCount * 2)).Select(_ => Task.Run(async () =>
            {
                try
                {
                    foreach (int testSegmentSize in TestSegmentSizes)
                        for (int testLoop = 0; testLoop < testLoops; testLoop++)
                        {
                            int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                            await using MemoryStreamSlim testService = CreateTestService(byteCount);
                            TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                            // Fill the stream with random bytes
                            byte[] dataCopy =
                                await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                            // Emulate asynchronous delays for writes to this stream
                            await using FileStreamMock fileStream = new FileStreamMock();
                            // Copy to the memory stream
                            testService.Seek(0, SeekOrigin.Begin);
                            await testService.CopyToAsync(fileStream);
                            testService.Position.Should().Be(testService.Length);
                            testService.Length.Should().Be(byteCount);
                            fileStream.Position.Should().Be(fileStream.Length);
                            fileStream.Length.Should().Be(byteCount);
                            // Verify the contents - suspend any async delays
                            using SuspendFileStreamAsyncDelay verifySuspendScope = fileStream.SuspendAsyncDelay();
                            await VerifyContentsFromStartToEndInBlocksAsync(fileStream, dataCopy, 0x61);
                        }
                }
                catch (Exception error)
                {
                    taskException ??= ExceptionDispatchInfo.Capture(error);
                }
            })).ToArray();
            await Task.WhenAll(runningTasks);
            taskException?.Throw();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a larger stream and verifying that the contents of the other 
        /// stream is correct. is identical to the data written.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CopyToLargerStream_CopiesCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    byte[] otherDataCopy =
                        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(memoryStream, GetTestInteger(byteCount + 1, byteCount + 0x10_0000), testSegmentSize);

                    // How much data do we want to write?
                    int copySourceFrom = GetTestInteger(Math.Max(0, (byteCount > 10) ? byteCount - GetTestInteger(byteCount - 10) : byteCount));
                    int copyCount = dataCopy.Length - copySourceFrom;
                    int copyDestFrom = GetTestInteger(otherDataCopy.Length - copyCount);
                    TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                    // Copy the data to the memory stream
                    testService.Seek(copySourceFrom, SeekOrigin.Begin);
                    memoryStream.Seek(copyDestFrom, SeekOrigin.Begin);

                    // Copy to the memory stream
                    testService.CopyTo(memoryStream);
                    testService.Position.Should().Be(testService.Length);
                    memoryStream.Position.Should().Be(copyDestFrom + copyCount);
                    memoryStream.Length.Should().Be(otherDataCopy.Length);
                    // Update the expected data copy
                    Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                    // Verify the contents
                    VerifyContentsFromStartToEndInBlocks(memoryStream, otherDataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a larger stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyToLargerStream_CopiesCorrectDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    byte[] otherDataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(memoryStream, GetTestInteger(byteCount + 1, byteCount + 0x10_0000), testSegmentSize);

                    // How much data do we want to write?
                    int copySourceFrom = GetTestInteger(Math.Max(0, (byteCount > 10) ? byteCount - GetTestInteger(byteCount - 10) : byteCount));
                    int copyCount = dataCopy.Length - copySourceFrom;
                    int copyDestFrom = GetTestInteger(otherDataCopy.Length - copyCount);
                    TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                    // Copy the data to the memory stream
                    testService.Seek(copySourceFrom, SeekOrigin.Begin);
                    memoryStream.Seek(copyDestFrom, SeekOrigin.Begin);

                    // Copy to the memory stream
                    await testService.CopyToAsync(memoryStream);
                    testService.Position.Should().Be(testService.Length);
                    memoryStream.Position.Should().Be(copyDestFrom + copyCount);
                    memoryStream.Length.Should().Be(otherDataCopy.Length);
                    // Update the expected data copy
                    Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                    // Verify the contents
                    await VerifyContentsFromStartToEndInBlocksAsync(memoryStream, otherDataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a larger asynchronous stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyToLargerAsyncStream_CopiesCorrectDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    byte[] otherDataCopy;
                    // Emulate asynchronous delays for writes to this stream
                    await using FileStreamMock fileStream = new FileStreamMock();
                    {
                        // Suspend the async delays while the stream is initially filled
                        using SuspendFileStreamAsyncDelay fillSuspendScope = fileStream.SuspendAsyncDelay();
                        otherDataCopy =
                            await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(fileStream, GetTestInteger(byteCount + 1, byteCount + 0x10_0000), testSegmentSize);
                    }

                    // How much data do we want to write?
                    int copySourceFrom = GetTestInteger(Math.Max(0, (byteCount > 10) ? byteCount - GetTestInteger(byteCount - 10) : byteCount));
                    int copyCount = dataCopy.Length - copySourceFrom;
                    int copyDestFrom = GetTestInteger(otherDataCopy.Length - copyCount);
                    TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                    // Copy the data to the memory stream
                    testService.Seek(copySourceFrom, SeekOrigin.Begin);
                    fileStream.Seek(copyDestFrom, SeekOrigin.Begin);

                    // Copy to the memory stream
                    await testService.CopyToAsync(fileStream);
                    testService.Position.Should().Be(testService.Length);
                    fileStream.Position.Should().Be(copyDestFrom + copyCount);
                    fileStream.Length.Should().Be(otherDataCopy.Length);
                    // Update the expected data copy
                    Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                    // Verify the contents - suspend any async delays
                    using SuspendFileStreamAsyncDelay verifySuspendScope = fileStream.SuspendAsyncDelay();
                    await VerifyContentsFromStartToEndInBlocksAsync(fileStream, otherDataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a larger asynchronous stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public async Task UsingMemoryStreamSlim_CopyToLargerAsyncStream_OnManyThreads_CopiesCorrectDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            ExceptionDispatchInfo? taskException = null;

            Task[] runningTasks = Enumerable.Range(0, Math.Max(8, Environment.ProcessorCount * 2)).Select(_ => Task.Run(async () =>
            {
                try
                {
                    foreach (int testSegmentSize in TestSegmentSizes)
                        for (int testLoop = 0; testLoop < testLoops; testLoop++)
                        {
                            int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                            await using MemoryStreamSlim testService = CreateTestService(byteCount);
                            TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                            // Fill the stream with random bytes
                            byte[] dataCopy =
                                await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                            byte[] otherDataCopy;
                            // Emulate asynchronous delays for writes to this stream
                            await using FileStreamMock fileStream = new FileStreamMock();
                            {
                                // Suspend the async delays while the stream is initially filled
                                using SuspendFileStreamAsyncDelay fillSuspendScope = fileStream.SuspendAsyncDelay();
                                otherDataCopy =
                                    await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(fileStream, GetTestInteger(byteCount + 1, byteCount + 0x10_0000), testSegmentSize);
                            }

                            // How much data do we want to write?
                            int copySourceFrom = GetTestInteger(Math.Max(0, (byteCount > 10) ? byteCount - GetTestInteger(byteCount - 10) : byteCount));
                            int copyCount = dataCopy.Length - copySourceFrom;
                            int copyDestFrom = GetTestInteger(otherDataCopy.Length - copyCount);
                            TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                            // Copy the data to the memory stream
                            testService.Seek(copySourceFrom, SeekOrigin.Begin);
                            fileStream.Seek(copyDestFrom, SeekOrigin.Begin);

                            // Copy to the memory stream
                            await testService.CopyToAsync(fileStream);
                            testService.Position.Should().Be(testService.Length);
                            fileStream.Position.Should().Be(copyDestFrom + copyCount);
                            fileStream.Length.Should().Be(otherDataCopy.Length);
                            // Update the expected data copy
                            Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                            // Verify the contents - suspend any async delays
                            using SuspendFileStreamAsyncDelay verifySuspendScope = fileStream.SuspendAsyncDelay();
                            await VerifyContentsFromStartToEndInBlocksAsync(fileStream, otherDataCopy, 0x61);
                        }
                }
                catch (Exception error)
                {
                    taskException ??= ExceptionDispatchInfo.Capture(error);
                }
            })).ToArray();
            await Task.WhenAll(runningTasks);
            taskException?.Throw();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a smaller stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CopyToSmallerStream_CopiesCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    byte[] otherDataCopy =
                        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(memoryStream, GetTestInteger(byteCount / 2, byteCount), testSegmentSize);

                    // How much data do we want to write?
                    int copyDestFrom = GetTestInteger(Math.Max(0, otherDataCopy.Length - GetTestInteger(Math.Min(otherDataCopy.Length, 10), Math.Max(11, otherDataCopy.Length - 10))));
                    int copyCount = otherDataCopy.Length - copyDestFrom;
                    int copySourceFrom = byteCount - copyCount;
                    TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                    // Copy the data to the memory stream
                    testService.Seek(copySourceFrom, SeekOrigin.Begin);
                    memoryStream.Seek(copyDestFrom, SeekOrigin.Begin);

                    // Copy to the memory stream
                    testService.CopyTo(memoryStream);
                    testService.Position.Should().Be(copySourceFrom + copyCount);
                    memoryStream.Position.Should().Be(memoryStream.Length);
                    memoryStream.Length.Should().Be(otherDataCopy.Length);
                    // Update the expected data copy
                    Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                    // Verify the contents
                    VerifyContentsFromStartToEndInBlocks(memoryStream, otherDataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a smaller stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyToSmallerStream_CopiesCorrectDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    byte[] otherDataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(memoryStream, GetTestInteger(byteCount / 2, byteCount), testSegmentSize);

                    // How much data do we want to write?
                    int copyDestFrom = GetTestInteger(Math.Max(0, otherDataCopy.Length - GetTestInteger(Math.Min(otherDataCopy.Length, 10), Math.Max(11, otherDataCopy.Length - 10))));
                    int copyCount = otherDataCopy.Length - copyDestFrom;
                    int copySourceFrom = byteCount - copyCount;
                    TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                    // Copy the data to the memory stream
                    testService.Seek(copySourceFrom, SeekOrigin.Begin);
                    memoryStream.Seek(copyDestFrom, SeekOrigin.Begin);

                    // Copy to the memory stream
                    await testService.CopyToAsync(memoryStream);
                    testService.Position.Should().Be(copySourceFrom + copyCount);
                    memoryStream.Position.Should().Be(memoryStream.Length);
                    memoryStream.Length.Should().Be(otherDataCopy.Length);
                    // Update the expected data copy
                    Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                    // Verify the contents
                    await VerifyContentsFromStartToEndInBlocksAsync(memoryStream, otherDataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a smaller asynchronous stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_CopyToSmallerAsyncStream_CopiesCorrectDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    await using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                    byte[] otherDataCopy;
                    // Emulate asynchronous delays for writes to this stream
                    await using FileStreamMock fileStream = new FileStreamMock();
                    {
                        // Suspend the async delays while the stream is initially filled
                        using SuspendFileStreamAsyncDelay fillSuspendScope = fileStream.SuspendAsyncDelay();
                        otherDataCopy =
                            await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(fileStream, GetTestInteger(byteCount / 2, byteCount), testSegmentSize);
                    }

                    // How much data do we want to write?
                    int copyDestFrom = GetTestInteger(Math.Max(0, otherDataCopy.Length - GetTestInteger(Math.Min(otherDataCopy.Length, 10), Math.Max(11, otherDataCopy.Length - 10))));
                    int copyCount = otherDataCopy.Length - copyDestFrom;
                    int copySourceFrom = byteCount - copyCount;
                    TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                    // Copy the data to the memory stream
                    testService.Seek(copySourceFrom, SeekOrigin.Begin);
                    fileStream.Seek(copyDestFrom, SeekOrigin.Begin);

                    // Copy to the memory stream
                    await testService.CopyToAsync(fileStream);
                    testService.Position.Should().Be(copySourceFrom + copyCount);
                    fileStream.Position.Should().Be(fileStream.Length);
                    fileStream.Length.Should().Be(otherDataCopy.Length);
                    // Update the expected data copy
                    Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                    // Verify the contents - suspend any async delays
                    using SuspendFileStreamAsyncDelay verifySuspendScope = fileStream.SuspendAsyncDelay();
                    await VerifyContentsFromStartToEndInBlocksAsync(fileStream, otherDataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to a smaller asynchronous stream and verifying that the contents of the other 
        /// stream is correct.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public async Task UsingMemoryStreamSlim_CopyToSmallerAsyncStream_OnManyThreads_CopiesCorrectDataAsync ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);


            ExceptionDispatchInfo? taskException = null;

            Task[] runningTasks = Enumerable.Range(0, Math.Max(8, Environment.ProcessorCount * 2)).Select(_ => Task.Run(async () =>
            {
                try
                {
                    foreach (int testSegmentSize in TestSegmentSizes)
                        for (int testLoop = 0; testLoop < testLoops; testLoop++)
                        {
                            int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                            await using MemoryStreamSlim testService = CreateTestService(byteCount);
                            TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                            // Fill the stream with random bytes
                            byte[] dataCopy =
                                await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(testService, byteCount, testSegmentSize);
                            byte[] otherDataCopy;
                            // Emulate asynchronous delays for writes to this stream
                            await using FileStreamMock fileStream = new FileStreamMock();
                            {
                                // Suspend the async delays while the stream is initially filled
                                using SuspendFileStreamAsyncDelay fillSuspendScope = fileStream.SuspendAsyncDelay();
                                otherDataCopy =
                                    await MemoryTestPrep.FillStreamAndArrayWithRandomBytesAsync(fileStream, GetTestInteger(byteCount / 2, byteCount), testSegmentSize);
                            }

                            // How much data do we want to write?
                            int copyDestFrom = GetTestInteger(Math.Max(0, otherDataCopy.Length - GetTestInteger(Math.Min(otherDataCopy.Length, 10), Math.Max(11, otherDataCopy.Length - 10))));
                            int copyCount = otherDataCopy.Length - copyDestFrom;
                            int copySourceFrom = byteCount - copyCount;
                            TestWriteLine($"  Destination stream size is {otherDataCopy.Length}, Copy Count = {copyCount}, Source Offset = {copySourceFrom}, Dest Offset = {copyDestFrom}");

                            // Copy the data to the memory stream
                            testService.Seek(copySourceFrom, SeekOrigin.Begin);
                            fileStream.Seek(copyDestFrom, SeekOrigin.Begin);

                            // Copy to the memory stream
                            await testService.CopyToAsync(fileStream);
                            testService.Position.Should().Be(copySourceFrom + copyCount);
                            fileStream.Position.Should().Be(fileStream.Length);
                            fileStream.Length.Should().Be(otherDataCopy.Length);
                            // Update the expected data copy
                            Array.Copy(dataCopy, copySourceFrom, otherDataCopy, copyDestFrom, copyCount);
                            // Verify the contents - suspend any async delays
                            using SuspendFileStreamAsyncDelay verifySuspendScope = fileStream.SuspendAsyncDelay();
                            await VerifyContentsFromStartToEndInBlocksAsync(fileStream, otherDataCopy, 0x61);
                        }
                }
                catch (Exception error)
                {
                    taskException ??= ExceptionDispatchInfo.Capture(error);
                }
            })).ToArray();
            await Task.WhenAll(runningTasks);
            taskException?.Throw();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and verifying that the contents of the other stream
        /// is identical to the data written.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_WriteToStream_CopiesAllData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumTestLoops, MaximumTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Fill the stream with random bytes
                    byte[] dataCopy =
                        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                    using MemoryStream memoryStream = new MemoryStream();
                    // Copy to the memory stream - first pick a random location as the current
                    // position - this should not change during the write operation.
                    int setPosition = GetTestInteger(byteCount);
                    testService.Seek(setPosition, SeekOrigin.Begin);
                    testService.WriteTo(memoryStream);
                    testService.Position.Should().Be(setPosition);
                    memoryStream.Position.Should().Be(memoryStream.Length);
                    memoryStream.Length.Should().Be(byteCount);
                    // Verify the contents
                    VerifyContentsFromStartToEndInBlocks(memoryStream, dataCopy, 0x61);
                }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests jumping around the stream space and randomly writing data to the stream and 
        /// verifying that the contents of the stream are consistent.
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public void UsingMemoryStreamSlim_WriteWithChaos_WritesCorrectData ()
        {
            int[] testDataSizes = GenerateTestDataSizes(MemorySegmentedBufferGroup.StandardBufferSegmentSize, MemorySegmentedBufferGroup.StandardBufferSegmentSize * 20).ToArray();
            int testLoops = RandomSource.GetRandomInteger(MinimumRandomPositionTestLoops, MaximumRandomPositionTestLoops + 1);

            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < testLoops; testLoop++)
                {
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    using MemoryStreamSlim testService = CreateTestService(byteCount);
                    TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and segment size {testSegmentSize}");

                    // Get an array to compare the results to
                    byte[] dataCopy = new byte[byteCount];
                    // For fixed mode streams, we should start with the expected data array to be the same data as the initial stream contents
                    if (testService.Mode == MemoryStreamSlimMode.Fixed)
                        testService.Read(dataCopy, 0, byteCount);
                    int dataCopyIndexPosition = 0;

                    for (int chaosIndex = 0; chaosIndex < 1000; chaosIndex++)
                    {
                        switch (GetTestInteger() % 4)
                        {
                            case 0:
                                WriteArrayDataToStream(testService, dataCopy, ref dataCopyIndexPosition);
                                break;

                            case 1:
                                WriteSpanDataToStream(testService, dataCopy, ref dataCopyIndexPosition);
                                break;

                            case 2:
                                WriteByteDataToStream(testService, dataCopy, ref dataCopyIndexPosition);
                                break;

                            case 3:
                                PositionStream(testService, byteCount, out dataCopyIndexPosition);
                                break;

                            case 4:
                                SeekStream(testService, byteCount, ref dataCopyIndexPosition);
                                break;

                            case 5:
                                SetStreamLength(testService, byteCount, dataCopy);
                                break;
                        }
                    }
                    // Verify the contents
                    testService.SetLength(Math.Max(testService.Length, dataCopy.Length));
                    VerifyContentsFromStartToEndOneRead(testService, dataCopy);
                }

            void WriteArrayDataToStream (MemoryStreamSlim stream, byte[] dataCopyArray, ref int dataCopyArrayPosition)
            {
                int writePosition = (int)stream.Position;
                int spaceLeft = dataCopyArray.Length - writePosition;
                int byteCount = (spaceLeft < 10) ? spaceLeft : RandomSource.GetRandomInteger(1, spaceLeft / 2);
                byte[] writeData = MemoryTestPrep.GetRandomByteArray(byteCount);

                stream.Write(writeData);
                Array.Copy(writeData, 0, dataCopyArray, dataCopyArrayPosition, byteCount);
                dataCopyArrayPosition += byteCount;
            }

            void WriteSpanDataToStream (MemoryStreamSlim stream, byte[] dataCopyArray, ref int dataCopyArrayPosition)
            {
                int writePosition = (int)stream.Position;
                int spaceLeft = dataCopyArray.Length - writePosition;
                int byteCount = (spaceLeft < 10) ? spaceLeft : RandomSource.GetRandomInteger(1, spaceLeft / 2);
                byte[] writeData = MemoryTestPrep.GetRandomByteArray(byteCount);

                stream.Write(writeData.AsSpan());
                Array.Copy(writeData, 0, dataCopyArray, dataCopyArrayPosition, byteCount);
                dataCopyArrayPosition += byteCount;
            }

            void WriteByteDataToStream (MemoryStreamSlim stream, byte[] dataCopyArray, ref int dataCopyArrayPosition)
            {
                int writePosition = (int)stream.Position;
                int spaceLeft = dataCopyArray.Length - writePosition;
                if (spaceLeft < 1)
                    return;
                byte writeData = GetRandomByte();
                stream.WriteByte(writeData);
                dataCopyArray[dataCopyArrayPosition++] = writeData;
            }

            void PositionStream (MemoryStreamSlim stream, int maxPosition, out int dataCopyArrayPosition)
            {
                int newPosition = (0 == maxPosition) ? 0 : RandomSource.GetRandomInteger(maxPosition);
                stream.Position = dataCopyArrayPosition = newPosition;
            }

            void SeekStream (MemoryStreamSlim stream, int maxPosition, ref int dataCopyArrayPosition)
            {
                int currentPosition = (int)stream.Position;

                switch (RandomSource.GetRandomInteger(4))
                {
                    case 0:
                        {
                            int newPosition = dataCopyArrayPosition = (0 == maxPosition) ? 0 : RandomSource.GetRandomInteger(maxPosition);
                            stream.Seek(newPosition, SeekOrigin.Begin);
                        }
                        break;

                    case 1:
                        {
                            int newOffset = (currentPosition == maxPosition) ? 0 : RandomSource.GetRandomInteger(maxPosition - currentPosition);
                            dataCopyArrayPosition += newOffset;
                            stream.Seek(newOffset, SeekOrigin.Current);
                        }
                        break;

                    case 2:
                        {
                            int newOffset = (0 == currentPosition) ? 0 : -RandomSource.GetRandomInteger(currentPosition);
                            dataCopyArrayPosition += newOffset;
                            stream.Seek(newOffset, SeekOrigin.Current);
                        }
                        break;

                    case 3:
                        {
                            long newOffset = (0 == stream.Length) ? 0 : -RandomSource.GetRandomLongInteger(stream.Length);
                            dataCopyArrayPosition = (int)(stream.Length + newOffset);
                            stream.Seek(newOffset, SeekOrigin.End);
                        }
                        break;
                }
            }

            void SetStreamLength (MemoryStreamSlim stream, int maxPosition, byte[] dataCopyArray)
            {
                int currentLength = (int)stream.Length;
                int newLength = (maxPosition < 2) ? maxPosition : RandomSource.GetRandomInteger(2, maxPosition);

                // Do we need to clear data in the copy?
                if ((stream.Mode == MemoryStreamSlimMode.Dynamic) && (newLength < currentLength))
                {
                    Array.Clear(dataCopyArray, newLength, currentLength - newLength);
                }

                stream.SetLength(newLength);
            }
        }
        //--------------------------------------------------------------------------------    

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
