// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using JetBrains.Profiler.Api;

using MemoryStreamBenchmarks;

namespace MemoryStreamProfile
{
    internal static class LargeFillProfile
    {
        public static void RunProfile (string[] args)
        {
            LargeSegmentedFillAndReadThroughputBenchmarks fillTest = new()
            {
                DataSize = 0xC000_0000,
                LoopCount = 2,
                CapacityOnCreate = true,
                GrowEachLoop = true,
                ExponentialBufferGrowth = true,
                ZeroBuffers = false
            };
            fillTest.GlobalSetup();

            // Determine which stream class type to use.
            if (args.Any(arg => arg.Equals("recycle", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine($@"Running {fillTest.GetType().Name}.{nameof(LargeSegmentedFillAndReadThroughputBenchmarks.UseLargeRecyclableMemoryStream)}");
                // Run through once to warm things up.
                fillTest.UseLargeRecyclableMemoryStream();

                // Start profiling if the collector is running.
                MemoryProfiler.CollectAllocations(true);
                MeasureProfiler.StartCollectingData();

                TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
                Stopwatch stopwatch = Stopwatch.StartNew();
                fillTest.UseLargeRecyclableMemoryStream();
                fillTest.UseLargeRecyclableMemoryStream();
                fillTest.UseLargeRecyclableMemoryStream();
                TimeSpan elapsed = stopwatch.Elapsed;
                TimeSpan processorTime = Process.GetCurrentProcess().TotalProcessorTime - startProcessorTime;
                Console.WriteLine($@"Elapsed time: {elapsed}, Processor time: {processorTime}");
            }
            else
            {
                Console.WriteLine($@"Running {fillTest.GetType().Name}.{nameof(LargeSegmentedFillAndReadThroughputBenchmarks.UseLargeMemoryStreamSlim)}");
                // Run through once to warm things up.
                fillTest.UseLargeMemoryStreamSlim();

                // Start profiling if the collector is running.
                MemoryProfiler.CollectAllocations(true);
                MeasureProfiler.StartCollectingData();

                TimeSpan startProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
                Stopwatch stopwatch = Stopwatch.StartNew();
                fillTest.UseLargeMemoryStreamSlim();
                fillTest.UseLargeMemoryStreamSlim();
                fillTest.UseLargeMemoryStreamSlim();
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
}
