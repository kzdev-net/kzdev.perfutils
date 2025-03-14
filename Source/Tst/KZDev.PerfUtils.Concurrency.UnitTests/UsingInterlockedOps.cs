// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for the <see cref="InterlockedOps"/> class.
/// </summary>
[Trait(TestConstants.TestTrait.Category, TestConstants.TestCategory.Concurrency)]
[ExcludeFromCodeCoverage]
public partial class UsingInterlockedOps : UnitTestBase
{
    /// <summary>
    /// The number of times to loop through the contention tests
    /// </summary>
    private const int ContentionTestLoopCount = 100_000;

    /// <summary>
    /// The number of times we loop through for simple condition based tests (non-contention)
    /// </summary>
    private const int ConditionTestLoopCount = 1_000_000;

    /// <summary>
    /// The number of test loops to run for simple bit manipulation operations
    /// </summary>
    private const int BitManagementTestLoopCount = 1_000_000;

    /// <summary>
    /// A test integer value that is used to test the <see cref="InterlockedOps"/> methods
    /// with contention. This value is shared between threads, and we are counting on the
    /// xunit test runner to treat all tests in this class as a collection so that there
    /// is only one instance of this value since no two tests will run at the same time.
    /// </summary>
    private static int _testContentionInteger = 0;

    /// <summary>
    /// A test unsigned integer value that is used to test the <see cref="InterlockedOps"/> methods
    /// with contention. This value is shared between threads, and we are counting on the
    /// xunit test runner to treat all tests in this class as a collection so that there
    /// is only one instance of this value since no two tests will run at the same time.
    /// </summary>
    private static uint _testContentionUnsignedInteger = 0;

    /// <summary>
    /// A test long integer value that is used to test the <see cref="InterlockedOps"/> methods
    /// with contention. This value is shared between threads, and we are counting on the
    /// xunit test runner to treat all tests in this class as a collection so that there
    /// is only one instance of this value since no two tests will run at the same time.
    /// </summary>
    private static long _testContentionLongInteger = 0;

    /// <summary>
    /// A test unsigned long integer value that is used to test the <see cref="InterlockedOps"/> methods
    /// with contention. This value is shared between threads, and we are counting on the
    /// xunit test runner to treat all tests in this class as a collection so that there
    /// is only one instance of this value since no two tests will run at the same time.
    /// </summary>
    private static ulong _testContentionUnsignedLongInteger = 0;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingInterlockedOps"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingInterlockedOps (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
