// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Base class for benchmarks for the <see cref="StringBuilderCache"/> utility class
    /// </summary>
    [MemoryDiagnoser]
    public abstract class StringBuilderThroughputBenchmarkBase
    {
        /// <summary>
        /// The source strings that will be used for building benchmark strings
        /// </summary>
        protected List<string>[] _buildSourceStrings = [[], [], [], [], [], [], [], [], []];

        /// <summary>
        /// The current list that is being used to build the strings
        /// </summary>
        protected int _buildSourceIndex = 0;

        /// <summary>
        /// The specifically set loop iteration count for the benchmarks
        /// </summary>
        protected int? _setLoopCount;

        /// <summary>
        /// The number of loop iterations to perform for each benchmark
        /// </summary>
        public virtual int LoopCount
        {
            get => _setLoopCount ?? 500_000;
            set => _setLoopCount = (value < 1) ? null : value;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Adds the individual string segments to the string builder
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void BuildString (StringBuilder builder)
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
        protected virtual void BaseGlobalSetup ()
        {
            for (int buildSourceIndex = 0; buildSourceIndex < _buildSourceStrings.Length; buildSourceIndex++)
            {
                int runningStringLength = 0;
                List<string> buildList = _buildSourceStrings[buildSourceIndex];
                for (int stringIndex = 0; stringIndex < 50; stringIndex++)
                {
                    string nextString = TestData.GetRandomString(TestData.SecureRandomSource, 10, 50);
                    if ((runningStringLength += nextString.Length) >= StringBuilderCache.MaxCachedCapacity)
                        break;
                    buildList.Add(nextString);
                }
            }
            _buildSourceIndex = 0;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        protected virtual void BaseGlobalCleanup ()
        {
            _buildSourceStrings = Array.Empty<List<string>>();
        }
        //--------------------------------------------------------------------------------
    }
}