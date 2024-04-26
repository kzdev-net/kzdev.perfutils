using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// The base class for all programmatic tests.
    /// </summary>
    public abstract class TestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// The test output helper that can be used to output test messages
        /// </summary>
        protected ITestOutputHelper XUnitTestOutputHelper { get; }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        [ExcludeFromCodeCoverage]
        protected TestBase (ITestOutputHelper xUnitTestOutputHelper)
        {
            Debug.Assert(xUnitTestOutputHelper is not null, "xUnitTestOutputHelper is not null");
            XUnitTestOutputHelper = xUnitTestOutputHelper;
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
