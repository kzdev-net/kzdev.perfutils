// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.ExceptionServices;

using FluentAssertions;

using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, "Memory")]
    public partial class UsingMemoryStreamSlim : UsingMemoryStreamSlimUnitTestBase
    {
        /// <summary>
        /// The size of the test read/write segments.
        /// </summary>
        private const int TestSegmentSize = 0x100;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSlim"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamSlim (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with default settings and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateDefault_HasExpectedSettingsAndCapacity ()
        {
            using MemoryStreamSlim stream = MemoryStreamSlim.Create();

            stream.Capacity.Should().Be(0);
            stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimOptions.DefaultZeroBufferBehavior);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with a specific capacity and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithCapacity_HasExpectedSettingsAndCapacity ()
        {
            int testCapacity = GetTestInteger(1, 1000);
            using MemoryStreamSlim stream = MemoryStreamSlim.Create(testCapacity);

            stream.Capacity.Should().Be(testCapacity);
            stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimOptions.DefaultZeroBufferBehavior);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with specific options and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithOptions_HasExpectedSettingsAndCapacity ()
        {
            MemoryStreamSlimOptions options = new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None };
            using MemoryStreamSlim stream = MemoryStreamSlim.Create(options);

            stream.Capacity.Should().Be(0);
            stream.Settings.ZeroBufferBehavior.Should().Be(options.ZeroBufferBehavior);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with specific capacity options and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithCapacityAndOptions_HasExpectedSettingsAndCapacity ()
        {
            int testCapacity = GetTestInteger(1, 1000);
            MemoryStreamSlimOptions options = new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None };
            using MemoryStreamSlim stream = MemoryStreamSlim.Create(testCapacity, options);

            stream.Capacity.Should().Be(testCapacity);
            stream.Settings.ZeroBufferBehavior.Should().Be(options.ZeroBufferBehavior);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with specific options and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithOptionsDelegate_HasExpectedSettingsAndCapacity ()
        {
            using MemoryStreamSlim stream = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);

            stream.Capacity.Should().Be(0);
            stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.None);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with specific capacity options and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithCapacityAndOptionsDelegate_HasExpectedSettingsAndCapacity ()
        {
            int testCapacity = GetTestInteger(1, 1000);
            using MemoryStreamSlim stream = MemoryStreamSlim.Create(testCapacity, options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);

            stream.Capacity.Should().Be(testCapacity);
            stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.None);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with specific options and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithOptionsStateDelegate_HasExpectedSettingsAndCapacity ()
        {
            using MemoryStreamSlim stream = MemoryStreamSlim.Create((options, bufferBehavior) =>
                options.ZeroBufferBehavior = bufferBehavior, MemoryStreamSlimZeroBufferOption.None);

            stream.Capacity.Should().Be(0);
            stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.None);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating a MemoryStreamSlim instance with specific capacity options and verifying
        /// the capacity and zero buffer behavior are as expected.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_CreateWithCapacityAndOptionsStateDelegate_HasExpectedSettingsAndCapacity ()
        {
            int testCapacity = GetTestInteger(1, 1000);
            using MemoryStreamSlim stream = MemoryStreamSlim.Create(testCapacity,
                (options, bufferBehavior) => options.ZeroBufferBehavior = bufferBehavior, MemoryStreamSlimZeroBufferOption.None);

            stream.Capacity.Should().Be(testCapacity);
            stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.None);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating an instance MemoryStreamSlim and verifying the initial property values.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_ExpandableStream_HasValidInitialPropertyValues ()
        {
            using Stream stream = MemoryStreamSlim.Create();

            stream.CanRead.Should().BeTrue();
            stream.CanSeek.Should().BeTrue();
            stream.CanWrite.Should().BeTrue();
            stream.CanTimeout.Should().BeFalse();
            stream.Length.Should().Be(0);
            stream.Position.Should().Be(0);

            stream.Invoking(s => s.ReadTimeout).Should().Throw<InvalidOperationException>();
            stream.Invoking(s => s.WriteTimeout).Should().Throw<InvalidOperationException>();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating different instances of MemoryStreamSlim, setting various random
        /// capacities, and verifying the capacity property is correct.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetRandomCapacity_CapacityPropertyIsCorrect ()
        {
            const int maxTestCapacity = MemorySegmentedBufferGroup.StandardBufferSegmentSize * 64;
            for (int instanceLoop = 0; instanceLoop < 50; instanceLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create();
                for (int testLoop = 0; testLoop < 1000; testLoop++)
                {
                    int setCapacity = GetTestInteger(maxTestCapacity + 1);
                    try
                    {
                        testService.Capacity = setCapacity;
                        testService.Capacity.Should().Be(setCapacity);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($@"** Failed on loop {testLoop} with capacity {setCapacity}");
                        throw;
                    }
                }
                // Try zero
                testService.Capacity = 0;
                testService.Capacity.Should().Be(0);
                // Test another new set of capacities
                for (int testLoop = 0; testLoop < 1000; testLoop++)
                {
                    int setCapacity = GetTestInteger(maxTestCapacity + 1);
                    try
                    {
                        testService.Capacity = setCapacity;
                        testService.Capacity.Should().Be(setCapacity);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($@"** Failed on loop {testLoop} with capacity {setCapacity}");
                        throw;
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating different instances of MemoryStreamSlim, setting various random
        /// capacities, and verifying the capacity property is correct.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetCapacity_CapacityPropertyIsCorrect ()
        {
            foreach (int[] testCapacitySizes in GetTestCapacitySizes())
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.OnRelease);
                for (int testLoop = 0; testLoop < testCapacitySizes.Length; testLoop++)
                {
                    int setCapacity = testCapacitySizes[testLoop];
                    try
                    {
                        testService.Capacity = setCapacity;
                        testService.Capacity.Should().Be(setCapacity);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($"** Failed on loop {testLoop} with capacity {setCapacity} from capacity set [{string.Join(",", testCapacitySizes.Select(cap => cap.ToString()))}]");
                        throw;
                    }
                }
                // Try zero
                testService.SetLength(0);
                testService.Capacity = 0;
                testService.Capacity.Should().Be(0);
                // Test another new set of capacities
                for (int testLoop = 0; testLoop < testCapacitySizes.Length; testLoop++)
                {
                    int setCapacity = testCapacitySizes[testLoop];
                    try
                    {
                        testService.Capacity = setCapacity;
                        testService.Capacity.Should().Be(setCapacity);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($"** Failed on loop {testLoop} with capacity {setCapacity} from capacity set [{string.Join(",", testCapacitySizes.Select(cap => cap.ToString()))}]");
                        throw;
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Capacity property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value below the current length, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetCapacityToLength_SetsProperCapacity ()
        {
            const int maxTestCapacity = MemorySegmentedBufferGroup.StandardBufferSegmentSize * 64;
            for (int instanceLoop = 0; instanceLoop < 50; instanceLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create();
                for (int testLoop = 0; testLoop < 1000; testLoop++)
                {
                    int setLength = GetTestInteger(10, maxTestCapacity + 1);
                    try
                    {
                        testService.SetLength(setLength);
                        testService.Capacity = setLength;
                        testService.Capacity.Should().Be(setLength);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($"** Failed on loop {instanceLoop} with length {setLength}");
                        throw;
                    }
                }
                // Try zero
                testService.SetLength(0);
                testService.Capacity = 0;
                testService.Capacity.Should().Be(0);
                // Test another new set of capacities
                for (int testLoop = 0; testLoop < 1000; testLoop++)
                {
                    int setLength = GetTestInteger(10, maxTestCapacity + 1);
                    try
                    {
                        testService.SetLength(setLength);
                        testService.Capacity = setLength;
                        testService.Capacity.Should().Be(setLength);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($"** Failed on loop {instanceLoop} with length {setLength}");
                        throw;
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Capacity property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value below zero, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetCapacityBelowZero_ThrowsException ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();
            testService.Invoking(s => s.Capacity = -1).Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("Capacity must be greater than or equal to zero. (Parameter 'Capacity')");
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Capacity property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value above the maximum allowed capacity, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetCapacityTooLarge_ThrowsException ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();
            testService.Invoking(s => s.CapacityLong = testService.MaximumCapacity + 1).Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage($"Capacity must be between 0 and {testService.MaximumCapacity}. (Parameter 'Capacity')");
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Capacity property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value below the current length, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetCapacityBelowLength_ThrowsException ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();
            int setLength = GetTestInteger(10, (MemorySegmentedBufferGroup.StandardBufferSegmentSize * 64) + 1);
            testService.SetLength(setLength);
            testService.Invoking(s => s.Capacity = setLength - 1).Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("Requested capacity is less than current size. (Parameter 'Capacity')");
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating different instances of MemoryStreamSlim, setting various random
        /// lengths, and verifying the length property is correct.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLength_LengthPropertyIsCorrect ()
        {
            foreach (int[] testLengthValues in GetTestLengthValues())
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create();
                for (int testLoop = 0; testLoop < testLengthValues.Length; testLoop++)
                {
                    int setLength = testLengthValues[testLoop];
                    try
                    {
                        testService.SetLength(setLength);
                        testService.Length.Should().Be(setLength);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($"** Failed on loop {testLoop} with length {setLength} from length set [{string.Join(",", testLengthValues.Select(cap => cap.ToString()))}]");
                        throw;
                    }
                }
                // Try zero
                testService.SetLength(0);
                testService.Length.Should().Be(0);
                // Now, loop through all the test lengths again
                for (int testLoop = 0; testLoop < testLengthValues.Length; testLoop++)
                {
                    int setLength = testLengthValues[testLoop];
                    try
                    {
                        testService.SetLength(setLength);
                        testService.Length.Should().Be(setLength);
                    }
                    catch (Exception)
                    {
                        TestWriteLine($"** Failed on loop {testLoop} with length {setLength} from length set [{string.Join(",", testLengthValues.Select(cap => cap.ToString()))}]");
                        throw;
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value below zero, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthBelowZero_ThrowsException ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();
            testService.Invoking(s => s.SetLength(-1)).Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("value must be greater than or equal to zero. (Parameter 'value')");
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value above the maximum allowed length, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthTooLarge_ThrowsException ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();
            testService.Invoking(s => s.SetLength(testService.MaximumCapacity + 1)).Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage($"value must be between 0 and {testService.MaximumCapacity}. (Parameter 'value')");
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value lower than the current length and verifying the length is set properly
        /// as well as the capacity and position properties.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthSmaller_SetsProperLength ()
        {
            // Fill the stream with random bytes
            for (int testLoop = 0; testLoop < 1000; testLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
                int newLength = GetTestInteger(byteCount - 2);
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and new length of {newLength}");

                MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);

                // Change the length to a random value
                testService.SetLength(newLength);
                testService.Length.Should().Be(newLength);
                testService.Capacity.Should().Be(byteCount);
                testService.Position.Should().Be(byteCount);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value lower than the current length and verifying the ReadByte method
        /// returns -1 (because the position is beyond the length).
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthSmaller_ReadByteReturnsProperValue ()
        {
            for (int testLoop = 0; testLoop < 1000; testLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
                int newLength = GetTestInteger(byteCount - 2);
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and new length of {newLength}");

                MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);

                // Change the length to a random value
                testService.SetLength(newLength);
                testService.ReadByte().Should().Be(-1);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value lower than the current length and verifying the Read method
        /// returns no data (because the position is beyond the length).
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthSmaller_ReadReturnsNoCopiedData ()
        {
            for (int testLoop = 0; testLoop < 1000; testLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
                int newLength = GetTestInteger(byteCount - 2);
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and new length of {newLength}");

                MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);

                // Change the length to a random value
                testService.SetLength(newLength);
                byte[] buffer = new byte[0x10000];
                testService.Read(buffer, 0, buffer.Length).Should().Be(0);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value lower than the current length and verifying the Write method
        /// still writes new data to the stream starting at the stream position.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthSmaller_WritesAppendsToPosition ()
        {
            for (int testLoop = 0; testLoop < 1000; testLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
                int newLength = GetTestInteger(byteCount - 2);
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and new length of {newLength}");

                MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);

                // Change the length to a random value
                testService.SetLength(newLength);
                int newByteCount = RandomSource.GetRandomInteger(10, byteCount);
                byte[] newData = new byte[newByteCount];
                GetRandomBytes(newData, newByteCount);
                testService.Write(newData, 0, newByteCount);

                testService.Length.Should().Be(byteCount + newByteCount);
                testService.Position.Should().Be(byteCount + newByteCount);
                testService.Capacity.Should().Be(byteCount + newByteCount);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value higher than the current length and verifying the length is set properly
        /// as well as the capacity and position properties.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthLarger_SetsProperLength ()
        {
            // Fill the stream with random bytes
            for (int testLoop = 0; testLoop < 1000; testLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
                int newLength = byteCount + GetTestInteger(10, 1000);
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and new length of {newLength}");

                MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);

                // Change the length to a random value
                testService.SetLength(newLength);
                testService.Length.Should().Be(newLength);
                testService.Capacity.Should().Be(newLength);
                testService.Position.Should().Be(byteCount);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value higher than the current length and verifying the filled space is zeroed.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_SetLengthLarger_FillsWithZeroedBytes ()
        {
            // Fill the stream with random bytes
            for (int testLoop = 0; testLoop < 5_000; testLoop++)
            {
                using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
                int addLength = GetTestInteger(10, 1000);
                int newLength = byteCount + addLength;
                TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount} and new length of {newLength}");

                MemoryTestPrep.FillStreamWithRandomBytes(testService, byteCount, 0x10000);

                // Change the length to a random value
                testService.SetLength(newLength);
                byte[] bytes = new byte[addLength];
                testService.Read(bytes, 0, addLength).Should().Be(addLength);
                bytes.Should().AllBeEquivalentTo<byte>(0);
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
        /// to a value higher than the current length and verifying the filled space is zeroed.
        /// The set to lengths are from a set of standard sizes
        /// </summary>
        [Fact]
        [Trait(TestConstants.TestTrait.TimeGenre, TestConstants.TimeGenreName.MedRun)]
        public void UsingMemoryStreamSlim_SetLengthLargerUsingSizeIntervals_FillsWithZeroedBytes ()
        {
            int[] testLengths = [MemorySegmentedBufferGroup.StandardBufferSegmentSize / 4,
                MemorySegmentedBufferGroup.StandardBufferSegmentSize / 2,
                MemorySegmentedBufferGroup.StandardBufferSegmentSize,
                (MemorySegmentedBufferGroup.StandardBufferSegmentSize / 2) * 3,
                MemorySegmentedBufferGroup.StandardBufferSegmentSize * 2,
                MemorySegmentedBufferGroup.StandardBufferSegmentSize * 3,
                MemorySegmentedBufferGroup.StandardBufferSegmentSize * 4];
            byte[] readCompareArray = new byte[testLengths.Max()];

            // Test the full lengths
            for (int sizeIndex = 0; sizeIndex < testLengths.Length; sizeIndex++)
            {
                int fullLength = testLengths[sizeIndex];
                // Test initial write data sizes that are less than the current full length
                for (int initialSize = 0xC00; initialSize < fullLength; initialSize += 0xC00)
                {
                    // Test pushing the position beyond the target length to different values
                    for (int positionIndex = sizeIndex; positionIndex < testLengths.Length; positionIndex++)
                    {
                        int setPositionValue = testLengths[positionIndex];
                        using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None);
                        int addLength = fullLength - initialSize;
                        TestWriteLine($"Running test loop {sizeIndex} with byte count of {initialSize} and new length of {fullLength}");

                        MemoryTestPrep.FillStreamWithRandomBytes(testService, initialSize, 0x1000);

                        // Set the position of the stream
                        testService.Position = setPositionValue;

                        // Change the length to a random value
                        testService.SetLength(fullLength);
                        // Now set the position back in order to read the data which 
                        // should have been filled in with zeros
                        testService.Position = initialSize;
                        Span<byte> readSpan = readCompareArray.AsSpan(0, addLength);
                        testService.Read(readSpan).Should().Be(addLength);
                        for (int byteIndex = 0; byteIndex < addLength; byteIndex++)
                        {
                            readSpan[byteIndex].Should().Be(0);
                        }
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating an instance MemoryStreamSlim as a dynamic sized stream and verifying 
        /// that calling GetBuffer throws an exception.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_GetBuffer_ThrowsException ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();

            // Fill the stream with random bytes
            int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
            MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);
            testService.Invoking(s => s.GetBuffer())
                .Should()
                .Throw<NotSupportedException>()
                .WithMessage($"The operation is not supported with {nameof(MemoryStreamSlimMode.Dynamic)} mode MemoryStreamSlim instances.");
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests creating an instance MemoryStreamSlim as a dynamic sized stream and verifying 
        /// that calling TryGetBuffer returns false.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamSlim_TryGetBuffer_ReturnsFalse ()
        {
            using MemoryStreamSlim testService = MemoryStreamSlim.Create();

            // Fill the stream with random bytes
            int byteCount = RandomSource.GetRandomInteger(10, 0x2_0000);
            MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, 0x10000);
            testService.TryGetBuffer(out _).Should().BeFalse();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting instances of the <see cref="MemorySegmentedBufferGroup"/> class with 
        /// various segment counts and verifies that the segment count is correct.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamSlim_AllocateAndDisposeManySmallStreams_GetsCorrectResults ()
        {
            int maximumSmallBufferSize = SegmentMemoryStreamSlim.SmallBufferSizes[^1];
            ExceptionDispatchInfo? taskException = null;

            Task[] runningTasks = Enumerable.Range(0, Math.Max(8, Environment.ProcessorCount * 2)).Select(_ => Task.Run(() =>
            {
                try
                {
                    List<MemoryStreamSlim> streams = new();
                    for (int testLoop = 0; testLoop < 500; testLoop++)
                    {
                        if (0 == (GetTestInteger() % 3))
                        {
                            // Every so often, dispose a stream
                            if (streams.Count == 0)
                            {
                                continue;
                            }
                            int disposeIndex = GetTestInteger(0, streams.Count);
                            MemoryStreamSlim disposeStream = streams[disposeIndex];
                            streams.RemoveAt(disposeIndex);
                            disposeStream.Dispose();
                            continue;
                        }
                        int byteCount = RandomSource.GetRandomInteger(20, maximumSmallBufferSize);
                        MemoryStreamSlim testService = MemoryStreamSlim
                            .Create(options =>
                                options.ZeroBufferBehavior = (0 == (testLoop & 1)) ? MemoryStreamSlimZeroBufferOption.OnRelease : MemoryStreamSlimZeroBufferOption.None);
                        TestWriteLine($"Running test loop {testLoop} with byte count of {byteCount}");

                        byte[] dataCopy =
                            MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, TestSegmentSize);
                        VerifyContentsFromStartToEndInBlocks(testService, dataCopy, 0x73);
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

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
