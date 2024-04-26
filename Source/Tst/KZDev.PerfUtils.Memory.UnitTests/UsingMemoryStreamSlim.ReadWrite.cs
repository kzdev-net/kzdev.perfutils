// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    public partial class UsingMemoryStreamSlim
    {
        /// <summary>
        /// The minimum number of test loops to run for the tests.
        /// </summary>
        private const int MinimumTestLoops = 100;
        /// <summary>
        /// The maximum number of test loops to run for the tests.
        /// </summary>
        private const int MaximumTestLoops = 200;

        /// <summary>
        /// The minimum number of test loops to run for the random position tests.
        /// </summary>
        private const int MinimumRandomPositionTestLoops = 500;
        /// <summary>
        /// The maximum number of test loops to run for the random position tests.
        /// </summary>
        private const int MaximumRandomPositionTestLoops = 800;
    }
    //################################################################################
}
