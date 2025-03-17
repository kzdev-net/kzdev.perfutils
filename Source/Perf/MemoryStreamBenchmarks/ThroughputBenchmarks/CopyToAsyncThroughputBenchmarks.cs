// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks;

/// <summary>
/// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream is
/// copied to another asynchronous stream (using CopyToAsync)
/// </summary>
[MemoryDiagnoser]
public class CopyToAsyncThroughputBenchmarks
{
    /// <summary>
    /// Helper method to compute the loop count based on the data size
    /// </summary>
    /// <param name="dataSize"></param>
    /// <returns></returns>
    private static int ComputeLoopCount (int dataSize)
    {
        return (int)Math.Max(5, Math.Min(100, 10_000 / Math.Pow(1.3, Math.Log(dataSize, 2))));
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
    /// The mock file stream instance used for the benchmarks
    /// </summary>
    private FileStreamMock? _fileStream;

    /// <summary>
    /// The data to fill the streams with
    /// </summary>
    protected byte[]? fillData;

    /// <summary>
    /// The options to use for the MemoryStreamSlim instances
    /// </summary>
    protected MemoryStreamSlimOptions MemoryStreamSlimOptions { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new();

    /// <summary>
    /// The common helper utility for processing stream benchmarks
    /// </summary>
    protected readonly StreamUtility streamUtility = new();

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
    public int DataSize { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = 0x100_0000;

    /// <summary>
    /// The different ways to create the stream instances, by specifying capacity or not
    /// </summary>
    [ParamsAllValues]
    public bool CapacityOnCreate { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

    /// <summary>
    /// Controls if the stream is initially filled with the data in a single operation or in segments
    /// </summary>
    [ParamsAllValues]
    public bool BulkInitialFill { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

    /// <summary>
    /// The different ways to create the stream instances, by specifying capacity or not
    /// </summary>
    // We are leaving this as a parameter to allow for testing with linear and exponential buffer growth in the
    // future if needed, but currently the tests are showing no notable difference in performance
    // with either approach, so we are leaving it linear by default.
    //[ParamsAllValues]
    public bool ExponentialBufferGrowth { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

    /// <summary>
    /// Indicates if the memory stream slim should use native memory
    /// </summary>
    // We are leaving this as a parameter to allow for testing with and without native memory in the
    // future if needed, but currently the tests are showing no notable difference in performance
    // with or without native memory, so we are leaving it off by default.
    //[ParamsAllValues]
    public bool UseNativeMemory { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;
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
    private Task ProcessStreamAsync (Stream stream, int dataLength)
    {
        return BulkInitialFill ? 
            streamUtility.BulkFillCopyToAsync(stream, _fileStream!, fillData!, dataLength) :
            streamUtility.SegmentFillCopyToAsync(stream, _fileStream!, fillData!, dataLength, SegmentSize);
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
        fillData = new byte[allocateDataSize];
        // We set the initial capacity of the file stream to minimize the impact of
        // the internal memory management of the file stream on the benchmarks as we
        // write to that stream.
        _fileStream = new FileStreamMock(allocateDataSize);
        TestData.GetRandomBytes(TestData.SecureRandomSource, fillData, allocateDataSize);
        // Avoid any benchmark impacts from clearing memory buffers
        MemoryStreamSlimOptions = new MemoryStreamSlimOptions() { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None };
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
    [Benchmark(Baseline = true, Description = "MemoryStream CopyToAsync")]
    public void UseMemoryStream ()
    {
        int processDataLength = DataSize;
        ManualResetEventSlim completeEvent = new(false);
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            completeEvent.Reset();
            using MemoryStream stream = CapacityOnCreate ? new MemoryStream(processDataLength) : new MemoryStream();
            Task asyncTask = ProcessStreamAsync(stream, processDataLength)
                .ContinueWith(task =>
                {
                    completeEvent.Set();
                    // Ensure the task is completed - and throw any exceptions
                    task.Wait();
                });
            // Wait for the signal event to be set
            completeEvent.Wait();
            // Wait on the task for any possible exceptions
            asyncTask.Wait();
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using RecyclableMemoryStream
    /// </summary>
    [Benchmark(Description = "RecyclableMemoryStream CopyToAsync")]
    public void UseRecyclableMemoryStream ()
    {
        int processDataLength = DataSize;
        ManualResetEventSlim completeEvent = new(false);
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            completeEvent.Reset();
            // Avoid any benchmark impacts from clearing memory buffers
            using RecyclableMemoryStream stream = CapacityOnCreate ?
                BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark", processDataLength) :
                BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark");
            Task asyncTask = ProcessStreamAsync(stream, processDataLength)
                .ContinueWith(task =>
                {
                    completeEvent.Set();
                    // Ensure the task is completed - and throw any exceptions
                    task.Wait();
                });
            // Wait for the signal event to be set
            completeEvent.Wait();
            // Wait on the task for any possible exceptions
            asyncTask.Wait();
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStreamSlim
    /// </summary>
    [Benchmark(Description = "MemoryStreamSlim CopyToAsync")]
    public void UseMemoryStreamSlim ()
    {
        int processDataLength = DataSize;
        ManualResetEventSlim completeEvent = new(false);
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            completeEvent.Reset();
            using MemoryStreamSlim stream = CapacityOnCreate ?
                MemoryStreamSlim.Create(processDataLength, MemoryStreamSlimOptions) :
                MemoryStreamSlim.Create(MemoryStreamSlimOptions);
            Task asyncTask = ProcessStreamAsync(stream, processDataLength)
                .ContinueWith(task =>
                {
                    completeEvent.Set();
                    // Ensure the task is completed - and throw any exceptions
                    task.Wait();
                });
            // Wait for the signal event to be set
            completeEvent.Wait();
            // Wait on the task for any possible exceptions
            asyncTask.Wait();
        }
    }
    //--------------------------------------------------------------------------------
}
