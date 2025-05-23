﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;

namespace StringsBenchmarks;

/// <summary>
/// Benchmarks for the <see cref="StringBuilderCache"/> utility class where the builder
/// is acquired with default capacity, built with multiple string segments and
/// then released.
/// </summary>
[MemoryDiagnoser]
public class StringBuilderThroughputBenchmarks : StringBuilderThroughputBenchmarkBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Common global setup for the benchmark methods
    /// </summary>
    [GlobalSetup]
    public void GlobalSetup ()
    {
        BaseGlobalSetup();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Common global cleanup for the benchmark methods
    /// </summary>
    [GlobalCleanup]
    public void GlobalCleanup () => BaseGlobalCleanup();
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using unique StringBuilder instances
    /// </summary>
    [Benchmark(Baseline = true, Description = "Unique StringBuilders")]
    public void UseStringBuilder ()
    {
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            StringBuilder builder = new();
            BuildString(builder);
            string builtString = builder.ToString();
            // So the built string is not optimized away...
            GC.KeepAlive(builtString);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using StringBuilderCache
    /// </summary>
    [Benchmark(Description = "Cached StringBuilders")]
    public void UseStringBuilderCache ()
    {
        for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
        {
            using StringBuilderScope builderScope =
                StringBuilderCache.GetScope();
            BuildString(builderScope);
            string builtString = builderScope.ToString();
            // So the built string is not optimized away...
            GC.KeepAlive(builtString);
        }
    }
    //--------------------------------------------------------------------------------
}
