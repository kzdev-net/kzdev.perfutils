// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream 
    /// is filled and read in segments with a set loop count.
    /// </summary>
    [MemoryDiagnoser]
    public class SetLoopCountFillAndReadThroughputBenchmarks
    {
        /// <summary>
        /// The size of the segments to fill and read in.
        /// </summary>
        private const int SegmentSize = 0x1000;  // 4KB

        /// <summary>
        /// The specifically set loop iteration count for the benchmarks
        /// </summary>
        private int? _setLoopCount;

        /// <summary>
        /// The common helper utility for processing stream benchmarks
        /// </summary>
        protected readonly StreamUtility streamUtility = new();

        /// <summary>
        /// The data to fill the streams with
        /// </summary>
        protected byte[]? fillData;

        /// <summary>
        /// The buffer to read the data back to
        /// </summary>
        protected byte[]? readBuffer;

        /// <summary>
        /// The options to use for the MemoryStreamSlim instances
        /// </summary>
        protected MemoryStreamSlimOptions MemoryStreamSlimOptions { get; set; } = new();

        /// <summary>
        /// The number of loop iterations to perform for each benchmark
        /// </summary>
        public int LoopCount 
        {
            get => _setLoopCount ?? 50;
            set => _setLoopCount = (value < 1) ? null : value;
        }

        /// <summary>
        /// The different bulk data sizes that will be used for the benchmarks
        /// </summary>
        [Params(0x2_0000, 0xF_0000, 0x100_0000, 0x5FF_0000, 0xC80_0000)]
        public int DataSize { get; set; } = 0x100_0000;

        /// <summary>
        /// Indicates if the stream should be configured to zero out buffers when released
        /// </summary>
        [ParamsAllValues]
        public bool ZeroBuffers { get; set; } = true;

        /// <summary>
        /// The different ways to create the stream instances, by specifying capacity or not
        /// </summary>
        // We are leaving this as a parameter to allow for testing with linear and exponential buffer growth in the
        // future if needed, but currently the tests are showing no notable difference in performance
        // with either approach, so we are leaving it linear by default.
        //[ParamsAllValues]
        public bool ExponentialBufferGrowth { get; set; } = false;

        /// <summary>
        /// Indicates if the memory stream slim should use native memory
        /// </summary>
        // We are leaving this as a parameter to allow for testing with and without native memory in the
        // future if needed, but currently the tests are showing no notable difference in performance
        // with or without native memory, so we are leaving it off by default.
        //[ParamsAllValues]
        public bool UseNativeMemory { get; set; } = false;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Processes the bulk fill and read operation on the stream
        /// </summary>
        /// <param name="stream">
        /// The stream instance being used.
        /// </param>
        /// <param name="dataLength">
        /// The length of the data to fill and read back
        /// </param>
        private void ProcessStream (Stream stream, int dataLength)
        {
            stream.Position = 0;
            streamUtility.SegmentFillAndRead(stream, fillData!, readBuffer!, dataLength, SegmentSize);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global setup for the benchmark methods
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup ()
        {
            MemoryStreamSlim.UseNativeLargeMemoryBuffers = UseNativeMemory;
            // Only need to allocate the buffers once, and we want the same data for all benchmarks
            if (fillData is not null)
                return;

            fillData = new byte[SegmentSize];
            readBuffer = new byte[SegmentSize];
            TestData.GetRandomBytes(TestData.SecureRandomSource, fillData, SegmentSize);
            MemoryStreamSlimOptions = new MemoryStreamSlimOptions() { ZeroBufferBehavior = ZeroBuffers ? MemoryStreamSlimZeroBufferOption.OnRelease : MemoryStreamSlimZeroBufferOption.None };
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup ()
        {
            fillData = null;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStream
        /// </summary>
        [Benchmark(Baseline = true, Description = "MemoryStream set loop count fill and read")]
        public void UseMemoryStream ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStream stream = new MemoryStream();
                ProcessStream(stream, processDataLength);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using RecyclableMemoryStream
        /// </summary>
        [Benchmark(Description = "RecyclableMemoryStream set loop count fill and read")]
        public void UseRecyclableMemoryStream ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using RecyclableMemoryStream stream = 
                    BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark");
                ProcessStream(stream, processDataLength);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStreamSlim
        /// </summary>
        [Benchmark(Description = "MemoryStreamSlim set loop count fill and read")]
        public void UseMemoryStreamSlim ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStreamSlim stream = MemoryStreamSlim.Create(MemoryStreamSlimOptions);
                ProcessStream(stream, processDataLength);
            }
        }
        //--------------------------------------------------------------------------------
    }
}