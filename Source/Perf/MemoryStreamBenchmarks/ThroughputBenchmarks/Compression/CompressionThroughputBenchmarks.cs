// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;

namespace MemoryStreamBenchmarks;

//################################################################################
/// <summary>
/// Base class for the memory stream benchmarks that compress into the stream.
/// </summary>
public abstract class CompressionThroughputBenchmarks
{
    /// <summary>
    /// Helper method to compute the loop count based on the data size
    /// </summary>
    /// <param name="dataSize"></param>
    /// <returns></returns>
    private static int ComputeLoopCount (long dataSize) =>
        (dataSize >= 0x100_0000) ? 1 : (int)Math.Max(2, Math.Min(50, 50_000 / Math.Pow(1.2, Math.Log(dataSize, 2))));

    /// <summary>
    /// The specifically set loop iteration count for the benchmarks
    /// </summary>
    private int? _setLoopCount;

    /// <summary>
    /// The source data to be compressed
    /// </summary>
    protected byte[]? byteData;

    /// <summary>
    /// The options to use for the MemoryStreamSlim instances
    /// </summary>
    protected MemoryStreamSlimOptions MemoryStreamSlimOptions { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new();

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
}
//################################################################################
