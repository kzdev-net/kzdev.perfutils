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
        /// Tests trying to acquire a <see cref="StringBuilder"/> instance from the cache with a
        /// negative capacity, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireNegativeCapacityBuilder_ThrowsException ()
        {
            this.Invoking(_ => StringBuilderCache.Acquire(-1))
                .Should().Throw<ArgumentOutOfRangeException>();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests trying to return a <see cref="StringBuilder"/> instance to the cache that is
        /// a null reference, which should throw an exception.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_ReturnNullStringBuilder_ThrowsException ()
        {
            this.Invoking(_ => StringBuilderCache.Release(null!))
                .Should().Throw<ArgumentNullException>();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests calling <see cref="StringBuilderCache.GetStringAndRelease(StringBuilder)"/> with
        /// a null reference for the <see cref="StringBuilder"/> instance, which should throw an
        /// exception.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_GetStringAndReturnNullStringBuilder_ThrowsException ()
        {
            this.Invoking(_ => StringBuilderCache.GetStringAndRelease(null!))
                .Should().Throw<ArgumentNullException>();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests simply getting a <see cref="StringBuilder"/> instance from the cache.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireZeroCapacityBuilder_ReturnsNewInstance ()
        {
            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Should().NotBeNull();
            builder.Capacity.Should().BeGreaterThanOrEqualTo(StringBuilderCache.DefaultCapacity);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance from the cache, building a
        /// string and returning the instance.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_BuildStringAndRelease_ReturnsProperString ()
        {
            string expected = GetRandomString(10, 20);

            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Append(expected);
            string builtString = StringBuilderCache.GetStringAndRelease(builder);
            builtString.Should().Be(expected);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance from the cache with a 
        /// capacity of "1", building a string and returning the instance.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireWithCapacityOneBuildStringAndRelease_ReturnsProperString ()
        {
            string expected = GetRandomString(10, 20);

            StringBuilder builder = StringBuilderCache.Acquire(1);
            builder.Append(expected);
            string builtString = StringBuilderCache.GetStringAndRelease(builder);
            builtString.Should().Be(expected);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance for a capacity
        /// that has a capacity larger than the 'maximum', and verifies that an instance
        /// is still returned.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireOverMaxCapacity_ReturnsStringBuilder()
        {
            StringBuilder builder = StringBuilderCache.Acquire(StringBuilderCache.MaxCachedCapacity * 2);
            builder.Should().NotBeNull();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance for a capacity
        /// that has a capacity larger than the 'maximum' and releases that instance and
        /// verifies that the operation is handled well - no exceptions (even though the
        /// instance is not cached).
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireOverMaxCapacityAndRelease_OperationAccepted ()
        {
            StringBuilder builder = StringBuilderCache.Acquire(StringBuilderCache.MaxCachedCapacity * 2);
            StringBuilderCache.Release(builder);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance from the cache, building a
        /// string and returning the instance, then acquiring another instance and the same
        /// instance should be returned
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireBuildStringAndRelease_ReacquireGetsSameInstance ()
        {
            string expected = GetRandomString(10, 20);

            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Append(expected);
            StringBuilderCache.Release(builder);

            StringBuilder nextBuilder = StringBuilderCache.Acquire();
            nextBuilder.Should().BeSameAs(builder);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance from the cache, building a
        /// string and getting the string from the builder which should be the expected string.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireBuildStringAndRelease_GetExpectedString ()
        {
            string expected = string.Empty;

            StringBuilder builder = StringBuilderCache.Acquire();
            for (int loopIndex = 0; loopIndex < 20; loopIndex++)
            {
                string addString = GetRandomString(2, 10);
                builder.Append(addString);
                expected += addString;
            }
            string builtString = StringBuilderCache.GetStringAndRelease(builder);

            builtString.Should().Be(expected);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests getting a <see cref="StringBuilder"/> instance from the cache, building a
        /// string and returning the instance, and verifying that the instance is stored in the
        /// internal <see cref="StringBuilderCache"/> cache list.
        /// </summary>
        [Fact]
        public void UsingStringBuilderCache_AcquireBuildStringAndRelease_InstanceStoredInCache ()
        {
            string expected = GetRandomString(10, 20);

            StringBuilder builder = StringBuilderCache.Acquire();
            builder.Append(expected);
            StringBuilderCache.Release(builder);

            StringBuilder?[]? checkCacheList = StringBuilderCache._threadCachedInstances;

            checkCacheList.Should().NotBeNull();
            checkCacheList.Should().Contain(builder);
        }
        //--------------------------------------------------------------------------------    

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
