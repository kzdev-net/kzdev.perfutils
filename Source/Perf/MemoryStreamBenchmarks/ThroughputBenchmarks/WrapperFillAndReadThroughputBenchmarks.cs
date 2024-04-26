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
    /// is filled and read in segments.
    /// </summary>
    [MemoryDiagnoser]
    public class WrapperFillAndReadThroughputBenchmarks
    {
        /// <summary>
        /// Helper method to compute the loop count based on the data size
        /// </summary>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        private static int ComputeLoopCount (int dataSize)
        {
            return (int)Math.Max(10, Math.Min(200_000, 0x8000_0000 / Math.Pow(1.96, Math.Log(dataSize, 2))));
        }

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
        private readonly StreamUtility _streamUtility = new();

        /// <summary>
        /// The data to fill the streams with
        /// </summary>
        private byte[]? _fillData;

        /// <summary>
        /// The buffer to read the data back to
        /// </summary>
        private byte[]? _readBuffer;

        /// <summary>
        /// The buffer that will be provided to the stream instances as the buffer to wrap
        /// </summary>
        private byte[] _wrappedBuffer = [];

        /// <summary>
        /// The options to use for the MemoryStreamSlim instances
        /// </summary>
        private MemoryStreamSlimOptions MemoryStreamSlimOptions { get; set; } = new();

        /// <summary>
        /// The number of loop iterations to perform for each benchmark
        /// </summary>
        public int LoopCount
        {
            get => _setLoopCount ?? ComputeLoopCount(DataSize);
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
        public bool UseNativeMemory { get; set; } = true;

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
            _streamUtility.SegmentFillAndRead(stream, _fillData!, _readBuffer!, dataLength, SegmentSize);
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
            if (_fillData is not null)
                return;

            _fillData = new byte[SegmentSize];
            _readBuffer = new byte[SegmentSize];
            _wrappedBuffer = new byte[DataSize];
            TestData.GetRandomBytes(_fillData, SegmentSize);
            TestData.GetRandomBytes(_wrappedBuffer, DataSize);
            MemoryStreamSlimOptions = new MemoryStreamSlimOptions() { ZeroBufferBehavior = ZeroBuffers ? MemoryStreamSlimZeroBufferOption.OnRelease : MemoryStreamSlimZeroBufferOption.None };
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup ()
        {
            _fillData = null;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStream
        /// </summary>
        [Benchmark(Baseline = true, Description = "MemoryStream array wrapper fill and read")]
        public void UseMemoryStream ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStream stream = new MemoryStream(_wrappedBuffer);
                ProcessStream(stream, processDataLength);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using RecyclableMemoryStream
        /// </summary>
        [Benchmark(Description = "RecyclableMemoryStream array wrapper fill and read")]
        public void UseRecyclableMemoryStream ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using RecyclableMemoryStream stream =
                    BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark", _wrappedBuffer, 0, processDataLength);
                ProcessStream(stream, processDataLength);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStreamSlim
        /// </summary>
        [Benchmark(Description = "MemoryStreamSlim array wrapper fill and read")]
        public void UseMemoryStreamSlim ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStreamSlim stream =
                    MemoryStreamSlim.Create(_wrappedBuffer, 0, processDataLength);
                ProcessStream(stream, processDataLength);
            }
        }
        //--------------------------------------------------------------------------------
    }
}