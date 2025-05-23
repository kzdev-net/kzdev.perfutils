﻿// Copyright (c) Kevin Zehrer
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
public class LargeSegmentedFillAndReadThroughputBenchmarks : LargeFillAndReadThroughputBenchmarks
{
    /// <summary>
    /// The size of the segments to fill and read in.
    /// </summary>
    private const int SegmentSize = 0x8000;  // 32KB

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
    /// Benchmark using RecyclableMemoryStream
    /// </summary>
    [Benchmark(Baseline = true, Description = "Large RecyclableMemoryStream fill and read")]
    public void UseLargeRecyclableMemoryStream ()
    {
        // Capture the parameters once locally
        long processDataLength = DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using RecyclableMemoryStream stream = CapacityOnCreate ?
                BenchMarkHelpers.GetLargeMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark", processDataLength) :
                BenchMarkHelpers.GetLargeMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark");
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStreamSlim
    /// </summary>
    [Benchmark(Description = "Large MemoryStreamSlim fill and read")]
    public void UseLargeMemoryStreamSlim ()
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
