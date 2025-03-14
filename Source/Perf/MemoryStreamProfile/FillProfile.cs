// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using JetBrains.Profiler.Api;

using MemoryStreamBenchmarks;

namespace MemoryStreamProfile;

internal static class FillProfile
{
    public static void RunProfile (string[] args)
    {
        SegmentedFillAndReadThroughputBenchmarks fillTest = new()
        {
            DataSize = 0xC80_0000,
            LoopCount = 200, //1_000_000,
            CapacityOnCreate = true,
            GrowEachLoop = false,
            ZeroBuffers = false
        };
        fillTest.GlobalSetup();

        // Determine which stream class type to use.
        if (args.Any(arg => arg.Equals("std", StringComparison.InvariantCultureIgnoreCase)))
        {
            Console.WriteLine($@"Running {fillTest.GetType().Name}.{nameof(SegmentedFillAndReadThroughputBenchmarks.UseMemoryStream)}");
            // Run through once to warm things up.
            fillTest.UseMemoryStream();

            // Start profiling if the collector is running.
            MemoryProfiler.CollectAllocations(true);
            MeasureProfiler.StartCollectingData();

            TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();
            fillTest.UseMemoryStream();
            fillTest.UseMemoryStream();
            fillTest.UseMemoryStream();
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
            Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
        }
        else if (args.Any(arg => arg.Equals("recycle", StringComparison.InvariantCultureIgnoreCase)))
        {
            Console.WriteLine($@"Running {fillTest.GetType().Name}.{nameof(SegmentedFillAndReadThroughputBenchmarks.UseRecyclableMemoryStream)}");
            // Run through once to warm things up.
            fillTest.UseRecyclableMemoryStream();

            // Start profiling if the collector is running.
            MemoryProfiler.CollectAllocations(true);
            MeasureProfiler.StartCollectingData();

            TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();
            fillTest.UseRecyclableMemoryStream();
            fillTest.UseRecyclableMemoryStream();
            fillTest.UseRecyclableMemoryStream();
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
            Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
        }
        else
        {
            Console.WriteLine($@"Running {fillTest.GetType().Name}.{nameof(SegmentedFillAndReadThroughputBenchmarks.UseMemoryStreamSlim)}");
            // Run through once to warm things up.
            fillTest.UseMemoryStreamSlim();

            // Start profiling if the collector is running.
            MemoryProfiler.CollectAllocations(true);
            MeasureProfiler.StartCollectingData();

            TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();
            fillTest.UseMemoryStreamSlim();
            fillTest.UseMemoryStreamSlim();
            fillTest.UseMemoryStreamSlim();
            TimeSpan elapsed = stopwatch.Elapsed;
            TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
            Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
            //MemoryStreamSlim.ReleaseMemoryBuffers();
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

        Stopwatch waitStopWatch = Stopwatch.StartNew();
        bool finalGcRun = false;

        Console.WriteLine(@"Press 'S' to stop");
        while (Console.ReadKey().Key != ConsoleKey.S)
        {
            if (finalGcRun)
                continue;
            if (waitStopWatch.ElapsedMilliseconds > 100_000)
            {
                GC.Collect(GC.MaxGeneration);
                finalGcRun = true;
                Console.WriteLine(@"Final GC run");
            }
            // Do nothing
        }
    }
}
