// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

using KZDev.PerfUtils;

namespace MemoryStreamBenchmarks
{
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
                AddJob(Job.Default.WithStrategy(RunStrategy.Monitoring)
                    .WithId("Primarily Read"));
            }
        }

        /// <summary>
        /// The index into the capacities list for the current capacity to use
        /// </summary>
        [ThreadStatic]
        protected int _threadUseCapacityIndex = 0;

        //--------------------------------------------------------------------------------
        protected override int GetNextCapacity ()
        {
            int returnValue = _useCapacities[_threadUseCapacityIndex];
            _threadUseCapacityIndex = (_threadUseCapacityIndex + 1) % _useCapacities.Length;
            return returnValue;
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
                _workThreads![index] = new Thread(ThreadRun)
                {
                    IsBackground = true,
                    Name = $"Benchmark Thread {index}"
                };
                _workThreads[index]!.Start();
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
                StringBuilder builder = new(GetNextCapacity());
                BuildString(builder);
                string builtString = builder.ToString();
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
                StringBuilder builder = StringBuilderCache.Acquire(GetNextCapacity());
                BuildString(builder);
                string builtString = StringBuilderCache.GetStringAndRelease(builder);
                GC.KeepAlive(builtString);
            });
        }
        //--------------------------------------------------------------------------------
    }
}