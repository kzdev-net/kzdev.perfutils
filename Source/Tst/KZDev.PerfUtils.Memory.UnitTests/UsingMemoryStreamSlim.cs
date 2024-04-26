using System.Diagnostics.CodeAnalysis;

using KZDev.PerfUtils.Tests;

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Memory.UnitTests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    public class UsingMemoryStreamSlim : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamSlim"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        [ExcludeFromCodeCoverage]
        public UsingMemoryStreamSlim (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
