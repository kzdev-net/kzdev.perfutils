﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using KZDev.PerfUtils;

namespace StringsBenchmarks;

/// <summary>
/// Benchmarks for the <see cref="StringBuilderCache"/> utility class where there 
/// are multiple threads that are acquiring, building and releasing string builders.
/// </summary>
[Config(typeof(Config))]
[MemoryDiagnoser]
public class StringBuilderMultipleThreadThroughputBenchmarks : StringBuilderMultipleThreadThroughputBenchmarkBase
{
    private class Config : ManualConfig
    {
        public Config ()
        {
            AddJob(Job.Default
                .WithMaxIterationCount(count: 30)
                .WithMaxWarmupCount(count: 20)
                .WithId("Multithread StringBuilderCache"));
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Overriding this back to the original implementation where we build out a string full size
    /// regardless of the capacity of the builder.
    /// </summary>
    /// <param name="builder"></param>
    protected override void BuildString (StringBuilder builder)
    {
        List<string> sourceList = buildSourceStrings[_buildSourceIndex];
        for (int stringIndex = 0; stringIndex < sourceList.Count; stringIndex++)
        {
            builder.Append(sourceList[stringIndex]);
        }
        _buildSourceIndex = (_buildSourceIndex + 1) % buildSourceStrings.Length;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// The method to do the thread run operations
    /// </summary>
    private static void ThreadRun (object? state)
    {
        WaitHandle[] waitHandles = { ThreadShutdownSignal.WaitHandle, ThreadsCanRunSignal.WaitHandle };

        try
        {
            while (true)
            {
                ThreadReadyCountDown!.Signal();
                int waitResult = WaitHandle.WaitAny(waitHandles);
                if (waitResult == 0)
                    return;
                ThreadControl control = _currentThreadControl!;

                for (int loop = 0; loop < control.LoopCount; loop++)
                {
                    control.Callback();
                }

                ThreadRunCountDown!.Signal();
                ThreadsCanResetSignal.Wait();
            }
        }
        finally
        {
            ThreadShutdownCountDown!.Signal();
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets up our set of test threads
    /// </summary>
    protected override void SetupThreads ()
    {
        ThreadShutdownCountDown!.Reset();
        for (int index = 0; index < ThreadCount; index++)
        {
            workThreads![index] = new Thread(ThreadRun)
            {
                IsBackground = true,
                Name = $"Benchmark Thread {index}"
            };
            workThreads[index]!.Start();
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using unique StringBuilder instances
    /// </summary>
    [Benchmark(Baseline = true, Description = "Multi-Thread Unique StringBuilders")]
    public void UseStringBuilder ()
    {
        RunThreads(LoopCount, () =>
        {
            StringBuilder builder = new();
            BuildString(builder);
            string builtString = builder.ToString();
            // So the built string is not optimized away...
            GC.KeepAlive(builtString);
        });
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Benchmark using StringBuilderCache
    /// </summary>
    [Benchmark(Description = "Multi-Thread Cached StringBuilders")]
    public void UseStringBuilderCache ()
    {
        RunThreads(LoopCount, () =>
        {
            using StringBuilderScope builderScope =
                StringBuilderCache.GetScope();
            BuildString(builderScope);
            string builtString = builderScope.ToString();
            // So the built string is not optimized away...
            GC.KeepAlive(builtString);
        });
    }
    //--------------------------------------------------------------------------------
}
