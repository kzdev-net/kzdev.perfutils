// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

using Microsoft.IO;

namespace MemoryStreamBenchmarks;

/// <summary>
/// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream 
/// wraps a buffer that is passed to the stream instance.
/// </summary>
[MemoryDiagnoser]
public class WrapperFillAndReadThroughputBenchmarks
{
    /// <summary>
    /// The specifically set loop iteration count for the benchmarks
    /// </summary>
    private int? _setLoopCount;

    /// <summary>
    /// The size of the segments to fill and read in.
    /// </summary>
    private const int SegmentSize = 0x1000;  // 4KB

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
    /// The number of loop iterations to perform for each benchmark
    /// </summary>
    public int LoopCount
    {
        get => _setLoopCount ?? GetDataSizeLoopCount(DataSize);
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
    public bool UseNativeMemory { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

    /// <summary>
    /// Helper method to get the loop count based on the data size
    /// </summary>
    /// <param name="dataSize">
    /// The data size of the benchmark running.
    /// </param>
    /// <returns>
    /// The number of loops that should be used for a given benchmark operation run.
    /// </returns>
    /// <remarks>
    /// This is largely for providing the ability in the future if needed and/or to 
    /// allow each derived benchmark class the ability to override the default.
    /// </remarks>
    protected virtual int GetDataSizeLoopCount(long dataSize) => 5;

    /// <summary>
    /// Common global setup for computed values.
    /// </summary>
    protected void DoCommonGlobalSetup()
    {
        _setLoopCount ??= GetDataSizeLoopCount(DataSize);
    }
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
        DoCommonGlobalSetup();
        MemoryStreamSlim.UseNativeLargeMemoryBuffers = UseNativeMemory;
        // Only need to allocate the buffers once, and we want the same data for all benchmarks
        if (_fillData is not null)
            return;

        _fillData = new byte[SegmentSize];
        _readBuffer = new byte[SegmentSize];
        _wrappedBuffer = new byte[DataSize];
        TestData.GetRandomBytes(TestData.SecureRandomSource, _fillData, SegmentSize);
        TestData.GetRandomBytes(TestData.SecureRandomSource, _wrappedBuffer, DataSize);
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
        // Capture the parameters once locally
        int processDataLength = DataSize;
        int loopCount = LoopCount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
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
        // Capture the parameters once locally
        int processDataLength = DataSize;
        int loopCount = LoopCount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using RecyclableMemoryStream stream =
                BenchMarkHelpers.GetMemoryStreamManager(false, ExponentialBufferGrowth).GetStream("benchmark", _wrappedBuffer, 0, processDataLength);
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
        // Capture the parameters once locally
        int processDataLength = DataSize;
        int loopCount = LoopCount;
        for (int loopIndex = 0; loopIndex < loopCount; loopIndex++)
        {
            using MemoryStreamSlim stream =
                MemoryStreamSlim.Create(_wrappedBuffer, 0, processDataLength);
            ProcessStream(stream, processDataLength);
        }
    }
    //--------------------------------------------------------------------------------
}
