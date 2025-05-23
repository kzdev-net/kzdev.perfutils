﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks;

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
        DoCommonGlobalSetup();
        MemoryStreamSlim.UseNativeLargeMemoryBuffers = UseNativeMemory;
        // Only need to allocate the buffers once, and we want the same data for all benchmarks
        if (fillData is not null)
            return;

        if (DataSize > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(DataSize), "DataSize must be less than or equal to int.MaxValue");
        int allocateDataSize = (int)DataSize;
        if (GrowEachLoop)
            allocateDataSize += (LoopCount * LoopGrowAmount);
        fillData = new byte[allocateDataSize];
        readBuffer = new byte[allocateDataSize];
        TestData.GetRandomBytes(TestData.SecureRandomSource, fillData, allocateDataSize);
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
        BenchMarkHelpers.ReleaseStreamManagers();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStream
    /// </summary>
    [Benchmark(Baseline = true, Description = "MemoryStream bulk fill and read")]
    public void UseMemoryStream ()
    {
        // Capture the parameters once locally
        int processDataLength = (int)DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStream stream = CapacityOnCreate ? new MemoryStream(processDataLength) : new MemoryStream();
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using RecyclableMemoryStream
    /// </summary>
    [Benchmark(Description = "RecyclableMemoryStream bulk fill and read")]
    public void UseRecyclableMemoryStream ()
    {
        // Capture the parameters once locally
        int processDataLength = (int)DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using RecyclableMemoryStream stream = CapacityOnCreate ?
                BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark", processDataLength) :
                BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream();
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStreamSlim
    /// </summary>
    [Benchmark(Description = "MemoryStreamSlim bulk fill and read")]
    public void UseMemoryStreamSlim ()
    {
        // Capture the parameters once locally
        int processDataLength = (int)DataSize;
        int loopCount = LoopCount;
        int loopGrowAmount = LoopGrowAmount;
        MemoryStreamSlimOptions memoryStreamSlimOptions = MemoryStreamSlimOptions;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStreamSlim stream = CapacityOnCreate ? MemoryStreamSlim.Create(processDataLength, memoryStreamSlimOptions) : MemoryStreamSlim.Create(memoryStreamSlimOptions);
            ProcessStream(stream, processDataLength);
            if (GrowEachLoop)
                processDataLength += loopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
}
