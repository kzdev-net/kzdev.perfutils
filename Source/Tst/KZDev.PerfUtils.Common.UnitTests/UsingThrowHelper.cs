// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using KZDev.PerfUtils.Tests;

using Xunit.Abstractions;

using ThrowHelper = KZDev.PerfUtils.Helpers.ThrowHelper;

namespace KZDev.PerfUtils.Common.UnitTests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="ThrowHelper"/> class.
    /// </summary>
    public class UsingThrowHelper : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingThrowHelper"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingThrowHelper (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        // TODO - Add tests for the ThrowHelper class here.
    }
    //################################################################################
}