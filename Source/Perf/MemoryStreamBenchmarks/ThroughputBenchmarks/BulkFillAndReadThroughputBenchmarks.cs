// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream is filled and read in bulk.
    /// </summary>
    [MemoryDiagnoser]
    public class BulkFillAndReadThroughputBenchmarks : FillAndReadThroughputBenchmarks
    {
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
            streamUtility.BulkFillAndRead(stream, fillData!, readBuffer!, dataLength);
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

            int allocateDataSize = DataSize;
            if (GrowEachLoop)
                allocateDataSize += (LoopCount * LoopGrowAmount);
            fillData = new byte[allocateDataSize];
            readBuffer = new byte[allocateDataSize];
            TestData.GetRandomBytes(fillData, allocateDataSize);
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
        [Benchmark(Baseline = true, Description = "MemoryStream bulk fill and read")]
        public void UseMemoryStream ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStream stream = CapacityOnCreate ? new MemoryStream(processDataLength) : new MemoryStream();
                ProcessStream(stream, processDataLength);
                if (GrowEachLoop)
                    processDataLength += LoopGrowAmount;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using RecyclableMemoryStream
        /// </summary>
        [Benchmark(Description = "RecyclableMemoryStream bulk fill and read")]
        public void UseRecyclableMemoryStream ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using RecyclableMemoryStream stream = CapacityOnCreate ?
                    BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark", processDataLength) :
                    BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream();
                ProcessStream(stream, processDataLength);
                if (GrowEachLoop)
                    processDataLength += LoopGrowAmount;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStreamSlim
        /// </summary>
        [Benchmark(Description = "MemoryStreamSlim bulk fill and read")]
        public void UseMemoryStreamSlim ()
        {
            int processDataLength = DataSize;
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStreamSlim stream = CapacityOnCreate ? MemoryStreamSlim.Create(processDataLength, MemoryStreamSlimOptions) : MemoryStreamSlim.Create(MemoryStreamSlimOptions);
                ProcessStream(stream, processDataLength);
                if (GrowEachLoop)
                    processDataLength += LoopGrowAmount;
            }
        }
        //--------------------------------------------------------------------------------
    }
}