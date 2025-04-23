// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for the <see cref="MemoryStreamSlim"/> class.
/// </summary>
[Trait(TestConstants.TestTrait.Category, "Memory")]
public partial class UsingMemoryStreamSlim : UsingMemoryStreamSlimUnitTestBase
{
    /// <summary>
    /// The minimum number of test loops to run for the tests.
    /// </summary>
    private const int MinimumTestLoops = 50;
    /// <summary>
    /// The maximum number of test loops to run for the tests.
    /// </summary>
    private const int MaximumTestLoops = 100;

    /// <summary>
    /// The minimum number of test loops to run for the random position tests.
    /// </summary>
    private const int MinimumRandomPositionTestLoops = 400;
    /// <summary>
    /// The maximum number of test loops to run for the random position tests.
    /// </summary>
    private const int MaximumRandomPositionTestLoops = 600;
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
