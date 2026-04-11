// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

using Xunit;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// The base class for all unit tests.
/// </summary>
public abstract class UnitTestBase : TestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitTestBase"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    protected UnitTestBase (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Assigns <see cref="TestData.RandomSource"/> to a <see cref="SeededRandomSource"/> derived from
    /// <paramref name="callerMemberName"/> so randomized test inputs reproduce across runs.
    /// Call before any use of random test data (including <c>GenerateTestDataSizes</c>).
    /// </summary>
    /// <param name="callerMemberName">
    /// Supplied by the compiler; the name of the calling test method.
    /// </param>
    protected void UseSeededRandomSourceForCurrentTest ([CallerMemberName] string? callerMemberName = null)
    {
        ArgumentNullException.ThrowIfNull(callerMemberName);
        int seed = GetStableFnv1AHash32(callerMemberName);
        RandomSource = new SeededRandomSource(seed);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Computes a stable 32-bit FNV-1a hash for <paramref name="text"/> (same value across
    /// processes and .NET versions for the same input string).
    /// </summary>
    /// <param name="text">
    /// The non-null string to hash.
    /// </param>
    /// <returns>
    /// The hash value used as an RNG seed.
    /// </returns>
    private static int GetStableFnv1AHash32 (string text)
    {
        unchecked
        {
            const uint offsetBasis = 2166136261;
            const uint prime = 16777619;
            uint hash = offsetBasis;
            foreach (char c in text)
            {
                hash ^= c;
                hash *= prime;
            }

            return (int)hash;
        }
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
