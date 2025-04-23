// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;

namespace MemoryStreamBenchmarks;

//################################################################################
/// <summary>
/// Base class for the memory stream benchmarks that fill and read the stream.
/// </summary>
public abstract class LargeFillAndReadThroughputBenchmarks
{
    /// <summary>
    /// The specifically set loop iteration count for the benchmarks
    /// </summary>
    private int? _setLoopCount;

    /// <summary>
    /// The specifically set loop grow amount for the benchmarks
    /// </summary>
    private int? _setLoopGrowAmount;

    /// <summary>
    /// The common helper utility for processing stream benchmarks
    /// </summary>
    protected readonly StreamUtility streamUtility = new();

    /// <summary>
    /// The data to fill the streams with
    /// </summary>
    protected byte[]? fillData;

    /// <summary>
    /// The buffer to read the data back to
    /// </summary>
    protected byte[]? readBuffer;

    /// <summary>
    /// The options to use for the MemoryStreamSlim instances
    /// </summary>
    protected MemoryStreamSlimOptions MemoryStreamSlimOptions { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = new();

    /// <summary>
    /// The number of loop iterations to perform for each benchmark
    /// </summary>
    public int LoopCount
    {
        get => _setLoopCount ?? GetDataSizeLoopCount(DataSize);
        set => _setLoopCount = (value < 1) ? null : value;
    }

    /// <summary>
    /// The amount that the data size should grow for each loop when 
    /// <see cref="GrowEachLoop"/> is <c>true</c>.
    /// </summary>
    public int LoopGrowAmount
    {
        get => _setLoopGrowAmount ?? GetDataSizeLoopGrowAmount(DataSize);
        set => _setLoopGrowAmount = (value < 1) ? null : value;
    }

    /// <summary>
    /// The different data sizes that will be used for the benchmarks
    /// </summary>
    [Params(0x2000_0000, 0xC000_0000, 0x2_8000_0000)]
    public long DataSize { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = 0xC000_0000;

    /// <summary>
    /// The different ways to create the stream instances, by specifying capacity or not
    /// </summary>
    [ParamsAllValues]
    public bool CapacityOnCreate { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

    /// <summary>
    /// Indicates if the stream should be configured to zero out buffers when released
    /// </summary>
    [ParamsAllValues]
    public bool ZeroBuffers { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

    /// <summary>
    /// Indicates if each loop iteration should grow the stream capacity
    /// </summary>
    [ParamsAllValues]
    public bool GrowEachLoop { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

    /// <summary>
    /// The different ways to create the stream instances, by specifying capacity or not
    /// </summary>
    //[ParamsAllValues]
    public bool ExponentialBufferGrowth { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

    /// <summary>
    /// Indicates if the memory stream slim should use native memory
    /// </summary>
    // We are leaving this as a parameter to allow for testing with and without native memory in the
    // future if needed, but currently the tests are showing no notable difference in performance
    // with or without native memory, so we are leaving it off by default.
    //[ParamsAllValues]
    public bool UseNativeMemory { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

    /// <summary>
    /// Helper method to get the loop grow amount based on the data size
    /// </summary>
    /// <param name="dataSize">
    /// The data size of the benchmark running.
    /// </param>
    /// <returns>
    /// The amount that the processed data size should grow for given benchmark operation run.
    /// </returns>
    protected virtual int GetDataSizeLoopGrowAmount(long dataSize) =>
        (dataSize >= int.MaxValue) ? (int.MaxValue >> 6) : ((int)dataSize / (LoopCount << 4));

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
    protected virtual int GetDataSizeLoopCount(long dataSize) => 1;

    /// <summary>
    /// Common global setup for computed values.
    /// </summary>
    protected void DoCommonGlobalSetup()
    {
        _setLoopCount ??= GetDataSizeLoopCount(DataSize);
        _setLoopGrowAmount ??= GetDataSizeLoopGrowAmount(DataSize);
    }
}
//################################################################################
