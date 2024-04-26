// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, "Memory")]
    public partial class UsingMemoryStreamSlim : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSlim"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamSlim (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
