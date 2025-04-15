// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks;

/// <summary>
/// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream 
/// is used as the destination of a Deflate compression operation.
/// </summary>
[MemoryDiagnoser]
public class DeflateCompressionThroughputBenchmarks : CompressionThroughputBenchmarks
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
    private void ProcessStream (Stream stream, long dataLength)
    {
        stream.Position = 0;
        using DeflateStream compressionStream = new DeflateStream(stream, CompressionMode.Compress, true);
        compressionStream.Write(byteData!, 0, (int)dataLength);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Common global setup for the benchmark methods
    /// </summary>
    [GlobalSetup]
    public void GlobalSetup ()
    {
        DoCommonGlobalSetup();
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
        BenchMarkHelpers.ReleaseStreamManagers();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStream
    /// </summary>
    [Benchmark(Baseline = true, Description = "MemoryStream Deflate Compression")]
    public void UseMemoryStream ()
    {
        // Capture the parameters once locally
        int dataSize = DataSize;
        int loopCount = LoopCount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStream stream = CapacityOnCreate ?
                new MemoryStream(dataSize) :
                new MemoryStream();
            ProcessStream(stream, dataSize);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using RecyclableMemoryStream
    /// </summary>
    [Benchmark(Description = "RecyclableMemoryStream Deflate Compression")]
    public void UseRecyclableMemoryStream ()
    {
        // Capture the parameters once locally
        int dataSize = DataSize;
        int loopCount = LoopCount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using RecyclableMemoryStream stream = CapacityOnCreate ?
                BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark", dataSize) :
                BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark");
            ProcessStream(stream, dataSize);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStreamSlim
    /// </summary>
    [Benchmark(Description = "MemoryStreamSlim Deflate Compression")]
    public void UseMemoryStreamSlim ()
    {
        // Capture the parameters once locally
        int dataSize = DataSize;
        int loopCount = LoopCount;
        MemoryStreamSlimOptions memoryStreamSlimOptions = MemoryStreamSlimOptions;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStreamSlim stream = CapacityOnCreate ?
                MemoryStreamSlim.Create(dataSize, memoryStreamSlimOptions) :
                MemoryStreamSlim.Create(memoryStreamSlimOptions);
            ProcessStream(stream, dataSize);
        }
    }
    //--------------------------------------------------------------------------------
}
