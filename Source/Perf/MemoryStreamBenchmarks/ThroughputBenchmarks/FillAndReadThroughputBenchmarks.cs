// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;

namespace MemoryStreamBenchmarks
{
    //################################################################################
    /// <summary>
    /// Base class for the memory stream benchmarks that fill and read the stream.
    /// </summary>
    public abstract class FillAndReadThroughputBenchmarks
    {
        /// <summary>
        /// Helper method to compute the loop count based on the data size
        /// </summary>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        private static int ComputeLoopCount (int dataSize)
        {
            return (int)Math.Max(5, Math.Min(1000, 500_000 / Math.Pow(1.5, Math.Log(dataSize, 2))));
        }

        /// <summary>
        /// The specifically set loop iteration count for the benchmarks
        /// </summary>
        private int? _setLoopCount;

        /// <summary>
        /// The amount that each loop iteration will grow the processing data set by
        /// </summary>
        protected const int LoopGrowAmount = 0x100;

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
            get => _setLoopCount ?? ComputeLoopCount(DataSize);
            set => _setLoopCount = (value < 1) ? null : value;
        }

        /// <summary>
        /// The different bulk data sizes that will be used for the benchmarks
        /// </summary>
        [Params(0x2_0000, 0xF_0000, 0x100_0000, 0x5FF_0000, 0xC80_0000)]
        public int DataSize { get; set; } = 0xC80_0000;

        /// <summary>
        /// The different ways to create the stream instances, by specifying capacity or not
        /// </summary>
        [ParamsAllValues]
        public bool CapacityOnCreate { get; set; } = false;

        /// <summary>
        /// Indicates if the stream should be configured to zero out buffers when released
        /// </summary>
        [ParamsAllValues]
        public bool ZeroBuffers { get; set; } = true;

        /// <summary>
        /// Indicates if each loop iteration should grow the stream capacity
        /// </summary>
        [ParamsAllValues]
        public bool GrowEachLoop { get; set; } = true;

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
    }
    //################################################################################
}
