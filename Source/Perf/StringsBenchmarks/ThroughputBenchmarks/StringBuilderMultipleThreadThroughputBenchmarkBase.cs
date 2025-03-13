// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
// ReSharper disable InconsistentNaming
#pragma warning disable CA2211
#pragma warning disable IDE1006 // Naming Styles

namespace StringsBenchmarks
{
    /// <summary>
    /// Base class for benchmarks for the <see cref="StringBuilderCache"/> utility class where the builder
    /// is acquired with varying capacities, built with multiple string segments and
    /// then released across multiple threads
    /// </summary>
    public abstract class StringBuilderMultipleThreadThroughputBenchmarkBase : StringBuilderThroughputBenchmarkBase
    {

        /// <summary>
        /// Information passed to the start of each thread
        /// </summary>
        protected record ThreadControl (int LoopCount, Action Callback);

        /// <summary>
        /// Indicates when a thread can start
        /// </summary>
        protected static readonly ManualResetEventSlim ThreadsCanRunSignal = new(false);

        /// <summary>
        /// Indicates when a thread can start
        /// </summary>
        protected static readonly ManualResetEventSlim ThreadsCanResetSignal = new(false);

        /// <summary>
        /// Indicates when a thread can start
        /// </summary>
        protected static readonly ManualResetEventSlim ThreadShutdownSignal = new(false);

        /// <summary>
        /// A countdown event for when all the threads are ready to run
        /// </summary>
        protected static volatile CountdownEvent? ThreadReadyCountDown = null;

        /// <summary>
        /// A countdown event for when all the threads have shutdown
        /// </summary>
        protected static volatile CountdownEvent? ThreadShutdownCountDown = null;

        /// <summary>
        /// A countdown event for when all the threads have run their loop
        /// </summary>
        protected static volatile CountdownEvent? ThreadRunCountDown = null;

        /// <summary>
        /// The currently active thread control
        /// </summary>
        protected static volatile ThreadControl? _currentThreadControl;

        /// <summary>
        /// The working threads for the benchmarks
        /// </summary>
        protected Thread?[]? workThreads;

        /// <summary>
        /// The number of threads to use
        /// </summary>
        [ParamsSource(nameof(ThreadCountValues))]
        public int ThreadCount { get; set; }

        /// <summary>
        /// The number of loop iterations to perform for each benchmark
        /// </summary>
        public override int LoopCount
        {
            get => setLoopCount ?? 50_000;
            set => setLoopCount = (value < 1) ? null : value;
        }

        /// <summary>
        /// The source for the number of threads to use
        /// </summary>
        public IEnumerable<int> ThreadCountValues
        {
            get
            {
                int processorCount = Environment.ProcessorCount;
                yield return Math.Min(4, processorCount);
                if (processorCount > 9)
                    yield return processorCount / 2;
                yield return processorCount;
                yield return processorCount * 2;
                if (processorCount < 17)
                    yield return processorCount * 4;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets up our set of test threads
        /// </summary>
        protected abstract void SetupThreads ();
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Shuts down the running threads
        /// </summary>
        private void ShutdownThreads ()
        {
            ThreadShutdownSignal.Set();
            ThreadShutdownCountDown!.Wait();
            for (int clearIndex = 0; clearIndex < ThreadCount; clearIndex++)
            {
                workThreads![clearIndex] = null;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Starts a running of the threads with the given parameters
        /// </summary>
        /// <param name="loopCount"></param>
        /// <param name="callback"></param>
        protected void RunThreads (int loopCount, Action callback)
        {
            _currentThreadControl = new ThreadControl(loopCount, callback);
            // Tell the threads they can run
            ThreadsCanRunSignal.Set();
            // Wait for all the threads to finish their loop
            ThreadRunCountDown!.Wait();
            // Make sure the threads don't loop again until we're ready
            ThreadsCanRunSignal.Reset();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global setup for the benchmark methods
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup ()
        {
            BaseGlobalSetup();
            workThreads = new Thread?[ThreadCount];
            ThreadRunCountDown = new CountdownEvent(ThreadCount);
            ThreadShutdownCountDown = new CountdownEvent(ThreadCount);
            ThreadReadyCountDown = new CountdownEvent(ThreadCount);
            ThreadsCanRunSignal.Reset();
            ThreadsCanResetSignal.Reset();
            ThreadShutdownSignal.Reset();
            SetupThreads();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup ()
        {
            ThreadReadyCountDown!.Wait();
            ShutdownThreads();
            BaseGlobalCleanup();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets up for each running of the test
        /// </summary>
        [IterationSetup]
        public void IterationSetup ()
        {
            ThreadReadyCountDown!.Wait();

            // Make sure the threads don't loop until we're ready for that
            ThreadsCanResetSignal.Reset();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Cleans up after each running of the test
        /// </summary>
        [IterationCleanup]
        public void IterationCleanup ()
        {
            ThreadReadyCountDown!.Reset();
            ThreadRunCountDown!.Reset();
            // Let the threads set up to wait for the next loop
            ThreadsCanResetSignal.Set();
        }
        //--------------------------------------------------------------------------------
    }
}
#pragma warning restore IDE1006 // Naming Styles
