// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Benchmarks for the <see cref="StringBuilderCache"/> utility class where the builder
    /// is acquired with varying capacities, built with multiple string segments and
    /// then released.
    /// </summary>
    [MemoryDiagnoser]
    public abstract class StringBuilderMultipleCapacityThroughputBenchmarkBase : StringBuilderThroughputBenchmarkBase
    {
        /// <summary>
        /// The set of capacities to use for the string builders
        /// </summary>
        protected int[] _useCapacities = [];

        /// <summary>
        /// The index into the capacities list for the current capacity to use
        /// </summary>
        protected int _useCapacityIndex = 0;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the capacity to use for the next acquire of a StringBuilder
        /// </summary>
        /// <returns></returns>
        protected int GetNextCapacity ()
        {
            int returnValue = _useCapacities[_useCapacityIndex];
            _useCapacityIndex = (_useCapacityIndex + 1) % _useCapacities.Length;
            return returnValue;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global setup for the benchmark methods
        /// </summary>
        protected override void BaseGlobalSetup ()
        {
            base.BaseGlobalSetup();
            _useCapacities = Enumerable.Range(0, 10_000)
                .Select(_ => TestData.GetTestInteger(TestData.SecureRandomSource, 0, StringBuilderCache.MaxCachedCapacity)).ToArray();
            _useCapacityIndex = 0;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Common global cleanup for the benchmark methods
        /// </summary>
        protected override void BaseGlobalCleanup ()
        {
            base.BaseGlobalCleanup();
            _useCapacities = [];
        }
        //--------------------------------------------------------------------------------
    }
}