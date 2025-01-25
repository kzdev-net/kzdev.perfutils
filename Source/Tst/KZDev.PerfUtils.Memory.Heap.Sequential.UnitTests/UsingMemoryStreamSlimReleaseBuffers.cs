// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime;

using FluentAssertions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class that never run in 
    /// parallel with other tests.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, "Memory")]
    public class UsingMemoryStreamSlimReleaseBuffers : UsingMemoryStreamSlimUnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSlim"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamSlimReleaseBuffers (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests writing data to the stream and verifying that the contents of the other stream
        /// is identical to the data written.
        /// </summary>
        /// <remarks>
        /// Clearly this test is very specific to using GC Heap memory and would not apply
        /// to using unmanaged memory, so this test is contained in the heap specific test
        /// project.
        /// </remarks>
        [Fact]
        public void UsingMemoryStreamSlimReleaseBuffers_CopyFullToStream_SettingReleaseMemoryProperlyReleasesMemory ()
        {
            int[] testDataSizes = GenerateTestDataSizes(1000, 0xF_FFFF).ToArray();

            // Get the GC as clean as possible
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration);
            using TestMetricsMonitor testMetricsMonitor = new TestMetricsMonitor();
            // Fill the streams with random bytes
            foreach (int testSegmentSize in TestSegmentSizes)
                for (int testLoop = 0; testLoop < 1000; testLoop++)
                {
                    // We actually just want to load a number of streams and then release them - to build up the memory usage
                    using MemoryStreamSlim testService = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.OnRelease);
                    int byteCount = testDataSizes[RandomSource.GetRandomInteger(testDataSizes.Length)];
                    MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, byteCount, testSegmentSize);
                }

            // Clean the GC and capture the current LOH memory usage
            GC.Collect(GC.MaxGeneration);
            double runCompleteLohMemory = testMetricsMonitor.LohSize;
            // Tell the MemoryStreamSlim to release its memory buffers
            MemoryStreamSlim.ReleaseMemoryBuffers();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration);
            // We should drop LOH memory by at least 1/2 (it should actually drop to zero, but we can't guarantee that)
            double newLohMemory = testMetricsMonitor.LohSize;
            newLohMemory.Should().BeLessThan(runCompleteLohMemory / 2);
        }
        //--------------------------------------------------------------------------------    

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
