// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream 
    /// is used as the destination of a GZip compression operation.
    /// </summary>
    [MemoryDiagnoser]
    public class BrotliCompressionThroughputBenchmarks : CompressionThroughputBenchmarks
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
            using BrotliStream compressionStream = new BrotliStream(stream, CompressionMode.Compress, true);
            compressionStream.Write(byteData!, 0, dataLength);
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
            if (byteData is not null)
                return;

            byteData = new byte[DataSize];
            TestData.GetRandomBytes(TestData.SecureRandomSource, byteData, DataSize);
            MemoryStreamSlimOptions = new MemoryStreamSlimOptions() { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None };
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup ()
        {
            byteData = null;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStream
        /// </summary>
        [Benchmark(Baseline = true, Description = "MemoryStream BrotliStream Compression")]
        public void UseMemoryStream ()
        {
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStream stream = CapacityOnCreate ?
                    new MemoryStream(DataSize) :
                    new MemoryStream();
                ProcessStream(stream, DataSize);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using RecyclableMemoryStream
        /// </summary>
        [Benchmark(Description = "RecyclableMemoryStream BrotliStream Compression")]
        public void UseRecyclableMemoryStream ()
        {
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using RecyclableMemoryStream stream = CapacityOnCreate ?
                    BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark", DataSize) :
                    BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark");
                ProcessStream(stream, DataSize);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using MemoryStreamSlim
        /// </summary>
        [Benchmark(Description = "MemoryStreamSlim BrotliStream Compression")]
        public void UseMemoryStreamSlim ()
        {
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                using MemoryStreamSlim stream = CapacityOnCreate ?
                    MemoryStreamSlim.Create(DataSize, MemoryStreamSlimOptions) :
                    MemoryStreamSlim.Create(MemoryStreamSlimOptions);
                ProcessStream(stream, DataSize);
            }
        }
        //--------------------------------------------------------------------------------
    }
}
