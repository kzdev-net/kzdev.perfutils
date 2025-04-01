// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks;

/// <summary>
/// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream 
/// is filled and read in segments.
/// </summary>
[MemoryDiagnoser]
public class SegmentedFillAndReadThroughputBenchmarks : FillAndReadThroughputBenchmarks
{
    /// <summary>
    /// The size of the segments to fill and read in.
    /// </summary>
    private const int SegmentSize = 0x1000;  // 4KB

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
        streamUtility.SegmentFillAndRead(stream, fillData!, readBuffer!, dataLength, SegmentSize);
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
    [Benchmark(Baseline = true, Description = "MemoryStream segmented fill and read")]
    public void UseMemoryStream ()
    {
        // Capture the parameters once locally
        long processDataLength = DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStream stream = CapacityOnCreate ? 
                (processDataLength > int.MaxValue ? throw new InvalidOperationException($"({nameof(processDataLength)}) must be less than or equal to int.MaxValue") : new MemoryStream((int)processDataLength)) : 
                new MemoryStream();
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using RecyclableMemoryStream
    /// </summary>
    [Benchmark(Description = "RecyclableMemoryStream segmented fill and read")]
    public void UseRecyclableMemoryStream ()
    {
        // Capture the parameters once locally
        long processDataLength = DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using RecyclableMemoryStream stream = CapacityOnCreate ?
                BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark", processDataLength) :
                BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark");
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStreamSlim
    /// </summary>
    [Benchmark(Description = "MemoryStreamSlim segmented fill and read")]
    public void UseMemoryStreamSlim ()
    {
        // Capture the parameters once locally
        long processDataLength = DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        MemoryStreamSlimOptions memoryStreamSlimOptions = MemoryStreamSlimOptions;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStreamSlim stream = CapacityOnCreate ? 
                MemoryStreamSlim.Create(processDataLength, memoryStreamSlimOptions) : 
                MemoryStreamSlim.Create(memoryStreamSlimOptions);
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
}
