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
/// is filled and read in segments.
/// </summary>
[MemoryDiagnoser]
public class ContinuousGrowFillAndReadThroughputBenchmarks
{
    /// <summary>
    /// The first data size to start with for the first loop iteration
    /// </summary>
    private const int StartDataSize = 0x8_0000;

    /// <summary>
    /// The last data size to end with for the last loop iteration
    /// </summary>
    private const int EndDataSize = 0x600_0000;

    /// <summary>
    /// The size of the segments to fill and read in.
    /// </summary>
    private const int SegmentSize = 0x1000;  // 4KB

    /// <summary>
    /// The amount that each loop iteration will grow the processing data set by
    /// </summary>
    private const int LoopGrowAmount = 0x40_0000;

    /// <summary>
    /// The number of loop iterations to perform for each benchmark
    /// </summary>
    private const int LoopCount = 1 + (EndDataSize - StartDataSize) / LoopGrowAmount;

    /// <summary>
    /// The write iteration step to capture the array from the stream
    /// </summary>
    private const int GetArrayWriteSegmentIteration = 0x4_0000 / SegmentSize;

    /// <summary>
    /// The data to fill the streams with
    /// </summary>
    private byte[]? _fillData;

    /// <summary>
    /// The buffer to read the data back to
    /// </summary>
    private byte[]? _readBuffer;

    /// <summary>
    /// The options to use for the MemoryStreamSlim instances
    /// </summary>
    private MemoryStreamSlimOptions MemoryStreamSlimOptions { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new();

    /// <summary>
    /// Indicates if the stream should be configured to zero out buffers when released
    /// </summary>
    [ParamsAllValues]
    public bool ZeroBuffers { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

    /// <summary>
    /// The different ways to create the stream instances, by specifying capacity or not
    /// </summary>
    [ParamsAllValues]
    public bool ExponentialBufferGrowth { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

    /// <summary>
    /// Indicates if the memory stream slim should use native memory
    /// </summary>
    // We are leaving this as a parameter to allow for testing with and without native memory in the
    // future if needed, but currently the tests are showing no measurable difference in performance
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
    /// <param name="getArrayCallback">
    /// The callback to get the array from the stream
    /// </param>
    private void ProcessStream<T> (T stream, int dataLength,
        Func<T, byte[]> getArrayCallback)
        where T : Stream
    {
        stream.Position = 0;
        byte[]? captureArray = null;

        int writeSizeLeft = dataLength;
        int segmentLoop = 0;
        while (writeSizeLeft > 0)
        {
            int writeSize = Math.Min(writeSizeLeft, SegmentSize);
            stream.Write(_fillData!, 0, writeSize);
            writeSizeLeft -= writeSize;
            if (segmentLoop++ != GetArrayWriteSegmentIteration)
                continue;
            captureArray = getArrayCallback(stream);
        }

        // Reset the position to the start of the stream for reading
        stream.Position = 0;
        int readSizeLeft = dataLength;
        while (readSizeLeft > 0)
        {
            int readSize = Math.Min(readSizeLeft, SegmentSize);
            int readInSize = stream.Read(_readBuffer!, 0, readSize);
            if (readInSize < readSize)
                throw new Exception("Failed to read all data back");
            readSizeLeft -= readInSize;
        }
        GC.KeepAlive(captureArray);
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
        TestData.GetRandomBytes(TestData.SecureRandomSource, _fillData, SegmentSize);
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
    [Benchmark(Baseline = true, Description = "MemoryStream growth fill and read")]
    public void UseMemoryStream ()
    {
        int processDataLength = StartDataSize;
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            using MemoryStream stream = new MemoryStream();
            ProcessStream(stream, processDataLength, workingStream => workingStream.ToArray());
            processDataLength += LoopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using RecyclableMemoryStream
    /// </summary>
    [Benchmark(Description = "RecyclableMemoryStream growth fill and read")]
    public void UseRecyclableMemoryStream ()
    {
        int processDataLength = StartDataSize;
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            using RecyclableMemoryStream stream =
                BenchMarkHelpers.GetMemoryStreamManager(ZeroBuffers, ExponentialBufferGrowth).GetStream("benchmark");
            ProcessStream(stream, processDataLength, workingStream => workingStream.GetBuffer());
            processDataLength += LoopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using MemoryStreamSlim
    /// </summary>
    [Benchmark(Description = "MemoryStreamSlim growth fill and read")]
    public void UseMemoryStreamSlim ()
    {
        int processDataLength = StartDataSize;
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            using MemoryStreamSlim stream =
                MemoryStreamSlim.Create(MemoryStreamSlimOptions);
            ProcessStream(stream, processDataLength, workingStream => workingStream.ToArray());
            processDataLength += LoopGrowAmount;
        }
    }
    //--------------------------------------------------------------------------------
}
