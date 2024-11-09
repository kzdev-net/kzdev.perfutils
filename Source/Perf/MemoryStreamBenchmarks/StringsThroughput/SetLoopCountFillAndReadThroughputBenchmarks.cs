// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Benchmarks for the <see cref="MemoryStreamSlim"/> utility class where the stream 
    /// is filled and read in segments with a set loop count.
    /// </summary>
    [MemoryDiagnoser]
    public class StringBuilderThroughputBenchmarks
    {
        /// <summary>
        /// The source strings that will be used for building benchmark strings
        /// </summary>
        private List<string>[] _buildSourceStrings = [[], [], [], [], [], [], [], [], []];

        /// <summary>
        /// The current list that is being used to build the strings
        /// </summary>
        private int _buildSourceIndex = 0;

        /// <summary>
        /// The specifically set loop iteration count for the benchmarks
        /// </summary>
        private int? _setLoopCount;

        /// <summary>
        /// The number of loop iterations to perform for each benchmark
        /// </summary>
        public int LoopCount
        {
            get => _setLoopCount ?? 50;
            set => _setLoopCount = (value < 1) ? null : value;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Adds the individual string segments to the string builder
        /// </summary>
        /// <param name="builder"></param>
        private void BuildString (StringBuilder builder)
        {
            List<string> sourceList = _buildSourceStrings[_buildSourceIndex];
            for (int stringIndex = 0; stringIndex < sourceList.Count; stringIndex++)
            {
                builder.Append(sourceList[stringIndex]);
            }
            _buildSourceIndex = (_buildSourceIndex + 1) % _buildSourceStrings.Length;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global setup for the benchmark methods
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup ()
        {
            for (int buildSourceIndex = 0; buildSourceIndex < _buildSourceStrings.Length; buildSourceIndex++)
            {
                List<string> buildList = _buildSourceStrings[buildSourceIndex];
                for (int stringIndex = 0; stringIndex < 200; stringIndex++)
                {
                    buildList.Add(TestData.GetRandomString(TestData.SecureRandomSource, 50, 512));
                }
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup ()
        {
            _buildSourceStrings = Array.Empty<List<string>>();
        }
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
                StringBuilder builder = StringBuilderCache.Acquire();
                BuildString(builder);
                string builtString = StringBuilderCache.GetStringAndRelease(builder);
                GC.KeepAlive(builtString);
            }
        }
        //--------------------------------------------------------------------------------
    }
}