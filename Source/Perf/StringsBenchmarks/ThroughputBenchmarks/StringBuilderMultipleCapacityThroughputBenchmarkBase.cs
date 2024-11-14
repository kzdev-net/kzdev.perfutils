// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;

namespace MemoryStreamBenchmarks
{
    /// <summary>
    /// Base class for benchmarks for the <see cref="StringBuilderCache"/> utility class where the builder
    /// is acquired with varying capacities, built with multiple string segments and
    /// then released.
    /// </summary>
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
        protected virtual int GetNextCapacity ()
        {
            int returnValue = _useCapacities[_useCapacityIndex];
            _useCapacityIndex = (_useCapacityIndex + 1) % _useCapacities.Length;
            return returnValue;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Adds the individual string segments to the string builder. We override this
        /// method for this method to be sure that the capacity of the builder does not
        /// exceed the capacity set when the builder was acquired, because we want the
        /// builder returned to the right cache slot.
        /// </summary>
        /// <param name="builder"></param>
        protected override void BuildString (StringBuilder builder)
        {
            List<string> sourceList = _buildSourceStrings[_buildSourceIndex];
            for (int stringIndex = 0; stringIndex < sourceList.Count; stringIndex++)
            {
                string addString = sourceList[stringIndex];
                if (stringIndex > 0 && (addString.Length + builder.Length > builder.Capacity))
                {
                    builder.Append(addString.Substring(0, builder.Capacity - builder.Length));
                    break;
                }
                builder.Append(addString);
            }
            _buildSourceIndex = (_buildSourceIndex + 1) % _buildSourceStrings.Length;
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
