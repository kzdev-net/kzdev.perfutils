using System.Text;

using FluentAssertions;

using KZDev.PerfUtils.Tests;

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Strings.UnitTests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="UsingStringBuilderCache"/> class for cases when the stream
    /// is created with a buffer that is already allocated.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, "Strings")]
    public class UsingStringBuilderCache : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingStringBuilderCache"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingStringBuilderCache (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests simply getting a <see cref="StringBuilder"/> instance from the cache.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireZeroCapacityBuilder_ReturnsNewInstance ()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Should().NotBeNull();
            builder.Capacity.Should().Be(StringBuilderCache.DefaultCapacity);
        }
        //--------------------------------------------------------------------------------    

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
