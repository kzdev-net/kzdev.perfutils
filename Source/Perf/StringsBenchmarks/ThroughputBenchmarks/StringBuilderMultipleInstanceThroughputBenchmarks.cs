// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Benchmarks for the <see cref="StringBuilderCache"/> utility class where the builder
    /// is acquired with varying capacities, and multiple instances are kept "in-flight"
    /// </summary>
    [MemoryDiagnoser]
    public class StringBuilderMultipleInstanceThroughputBenchmarks : StringBuilderMultipleCapacityThroughputBenchmarkBase
    {
        /// <summary>
        /// The list of "in-flight" string builders
        /// </summary>
        private readonly StringBuilder?[] _activeBuilders = new StringBuilder?[20];

        /// <summary>
        /// The index of the new "in-flight" string builder
        /// </summary>
        private int _activeBuilderHeadIndex = 0;

        /// <summary>
        /// The index of the "in-flight" string builder to release.
        /// </summary>
        private int _activeBuilderTailIndex = 0;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global setup for the benchmark methods
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup () => BaseGlobalSetup();
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup () => BaseGlobalCleanup();
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets up for each running of the test
        /// </summary>
        [IterationSetup]
        public void IterationSetup ()
        {
            _activeBuilderHeadIndex = _activeBuilderTailIndex = 0;
            Array.Clear(_activeBuilders, 0, _activeBuilders.Length);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using unique StringBuilder instances
        /// </summary>
        //[Benchmark(Baseline = true, Description = "Multiple In-Flight Unique StringBuilders")]
        public void UseStringBuilder ()
        {
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                StringBuilder builder = new(GetNextCapacity());
                BuildString(builder);
                string builtString = builder.ToString();
                // Store the builder in the in-flight buffer
                _activeBuilders[_activeBuilderHeadIndex] = builder;
                _activeBuilderHeadIndex = (_activeBuilderHeadIndex + 1) % _activeBuilders.Length;
                GC.KeepAlive(builtString);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Benchmark using StringBuilderCache
        /// </summary>
        [Benchmark(Description = "Multiple In-Flight Cached StringBuilders")]
        public void UseStringBuilderCache ()
        {
            for (int loopIndex = 0; loopIndex < LoopCount; loopIndex++)
            {
                StringBuilder builder = StringBuilderCache.Acquire(GetNextCapacity());
                BuildString(builder);
                string builtString = builder.ToString();
                // Store the builder in the in-flight buffer
                _activeBuilders[_activeBuilderHeadIndex] = builder;
                _activeBuilderHeadIndex = (_activeBuilderHeadIndex + 1) % _activeBuilders.Length;
                if (_activeBuilderHeadIndex == _activeBuilderTailIndex)
                {
                    // Release the builder at the tail index
                    StringBuilderCache.Release(_activeBuilders[_activeBuilderTailIndex]!);
                    _activeBuilderTailIndex = (_activeBuilderTailIndex + 1) % _activeBuilders.Length;
                }
                GC.KeepAlive(builtString);
            }
            // Release the rest of the in-flight instances.
            while (_activeBuilderTailIndex != _activeBuilderHeadIndex)
            {
                StringBuilderCache.Release(_activeBuilders[_activeBuilderTailIndex]!);
                _activeBuilderTailIndex = (_activeBuilderTailIndex + 1) % _activeBuilders.Length;
            }
        }
        //--------------------------------------------------------------------------------
    }
}