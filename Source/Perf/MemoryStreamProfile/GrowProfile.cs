// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using JetBrains.Profiler.Api;

using KZDev.PerfUtils;

using MemoryStreamBenchmarks;

namespace MemoryStreamProfile;

internal static class GrowProfile
{
    private const int ProfileLoopCount = 500;

    public static void RunProfile (string[] args)
    {
        Thread.Sleep(2000);
        ContinuousGrowFillAndReadThroughputBenchmarks growTest = new()
        {
            ZeroBuffers = false
        };
        growTest.GlobalSetup();

        // Determine which stream class type to use.
        if (args.Any(arg => arg.Equals("std", StringComparison.InvariantCultureIgnoreCase)))
        {
            Console.WriteLine($@"Running {growTest.GetType().Name}.{nameof(ContinuousGrowFillAndReadThroughputBenchmarks.UseMemoryStream)}");
            // Run through once to warm things up.
            growTest.UseMemoryStream();

            // Start profiling if the collector is running.
            MemoryProfiler.CollectAllocations(true);
            MeasureProfiler.StartCollectingData();

            TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int loopIndex = 0; loopIndex < ProfileLoopCount; loopIndex++)
            {
                growTest.UseMemoryStream();
            }
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
            Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
        }
        else if (args.Any(arg => arg.Equals("recycle", StringComparison.InvariantCultureIgnoreCase)))
        {
            Console.WriteLine($@"Running {growTest.GetType().Name}.{nameof(ContinuousGrowFillAndReadThroughputBenchmarks.UseRecyclableMemoryStream)}");
            // Run through once to warm things up.
            growTest.UseRecyclableMemoryStream();

            // Start profiling if the collector is running.
            MemoryProfiler.CollectAllocations(true);
            MeasureProfiler.StartCollectingData();

            TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int loopIndex = 0; loopIndex < ProfileLoopCount; loopIndex++)
            {
                growTest.UseRecyclableMemoryStream();
            }
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
            Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
        }
        else
        {
            Console.WriteLine($@"Running {growTest.GetType().Name}.{nameof(ContinuousGrowFillAndReadThroughputBenchmarks.UseMemoryStreamSlim)}");
            // Run through once to warm things up.
            growTest.UseMemoryStreamSlim();

            // Start profiling if the collector is running.
            MemoryProfiler.CollectAllocations(true);
            MeasureProfiler.StartCollectingData();

            TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int loopIndex = 0; loopIndex < ProfileLoopCount; loopIndex++)
            {
                growTest.UseMemoryStreamSlim();
            }
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
            Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
            MemoryStreamSlim.ReleaseMemoryBuffers();
        }

        GC.Collect(GC.MaxGeneration);
        GC.WaitForPendingFinalizers();
        Thread.Sleep(1000);
        GC.Collect(GC.MaxGeneration);
        Thread.Sleep(1000);

        MemoryProfiler.GetSnapshot();
        MemoryProfiler.CollectAllocations(false);
        MeasureProfiler.StopCollectingData();
        MeasureProfiler.SaveData();

        //Console.WriteLine(@"Press 'S' to stop");
        //while (Console.ReadKey().Key != ConsoleKey.S)
        //{
        //    // Do nothing
        //}
    }
}
