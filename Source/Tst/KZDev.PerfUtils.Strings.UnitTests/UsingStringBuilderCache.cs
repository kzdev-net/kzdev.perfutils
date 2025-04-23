using System.Collections.Concurrent;
using System.Text;

using FluentAssertions;

using KZDev.PerfUtils.Tests;
#pragma warning disable HAA0601

namespace KZDev.PerfUtils.Strings.UnitTests;

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
    /// Tests getting a <see cref="StringBuilder"/> instance from the cache, building a
    /// substring and returning the instance.
    /// </summary>
    [Fact]
    public void UsingStringBuilderCache_BuildSubStringAndRelease_ReturnsProperString ()
    {
        string expected = GetRandomString(20, 30);
        int startIndex = GetTestInteger(1, expected.Length >> 1);
        int length = GetTestInteger(1, expected.Length - startIndex - 1);

        StringBuilder builder = StringBuilderCache.Acquire();
        builder.Append(expected);
        string builtSubString = StringBuilderCache.GetStringAndRelease(builder, startIndex, length);
        builtSubString.Should().Be(expected.Substring(startIndex, length));
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
        // Be sure our test starts with a cleared cache
        StringBuilderCache._threadCache = null;
        StringBuilderCache._globalCache = null;
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
        // Be sure our test starts with a cleared cache
        StringBuilderCache._threadCache = null;
        StringBuilderCache._globalCache = null;
        string expected = GetRandomString(10, StringBuilderCache.DefaultCapacity);

        StringBuilder builder = StringBuilderCache.Acquire();
        builder.Append(expected);
        StringBuilderCache.Release(builder);

        StringBuilder?[]? checkCacheList = StringBuilderCache._threadCache;

        checkCacheList.Should().NotBeNull();
        checkCacheList.Should().Contain(builder);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests getting a <see cref="StringBuilder"/> instance from the cache for all the
    /// expected cacheable sizes, building a string and returning the instance, and verifying
    /// that they are stored in the internal <see cref="StringBuilderCache"/> cache list.
    /// </summary>
    [Fact]
    public void UsingStringBuilderCache_AcquireBuildStringOfDifferentCapacitySizesAndRelease_InstancesStoredInCache ()
    {
        // Be sure our test starts with a cleared cache
        StringBuilderCache._threadCache = null;
        StringBuilderCache._globalCache = null;
        string addString = GetRandomString(10, StringBuilderCache.DefaultCapacity);
        int getCapacity = StringBuilderCache.DefaultCapacity;
        List<StringBuilder> usedBuilders = [];

        while (getCapacity <= StringBuilderCache.MaxCachedCapacity)
        {
            StringBuilder builder = StringBuilderCache.Acquire(getCapacity);
            builder.Append(addString);
            usedBuilders.Add(builder);
            StringBuilderCache.Release(builder);
            getCapacity <<= 1;
        }
        StringBuilder?[]? checkCacheList = StringBuilderCache._threadCache;

        // Display the value of each entry in the check cache list
        for (int checkIndex = 0; checkIndex < checkCacheList!.Length; checkIndex++)
        {
            TestWriteLine($"Cache list entry {checkIndex} is {(checkCacheList[checkIndex] is StringBuilder builder ? builder.ToString() : "<<null>>")}");
        }
        checkCacheList.Should().BeEquivalentTo(usedBuilders);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests getting a <see cref="StringBuilder"/> instance from the cache for all the
    /// expected cacheable sizes, building a string and returning the instance, and verifying
    /// that they are stored in the internal <see cref="StringBuilderCache"/> cache list.
    /// </summary>
    [Fact]
    public void UsingStringBuilderCache_AcquireBuildStringOfMaxCapacityAndRelease_InstancesStoredInCache ()
    {
        // Be sure our test starts with a cleared cache
        StringBuilderCache._threadCache = null;
        StringBuilderCache._globalCache = null;
        string addString = GetRandomString(10, StringBuilderCache.DefaultCapacity);
        // We only use the capacity value to indicate the different cache slots
        // that we will use, but we always get the max capacity
        int checkCapacity = StringBuilderCache.DefaultCapacity;
        List<StringBuilder> usedBuilders = new();

        while (checkCapacity <= StringBuilderCache.MaxCachedCapacity)
        {
            StringBuilder builder = 
                StringBuilderCache.Acquire(StringBuilderCache.MaxCachedCapacity);
            builder.Append(addString);
            usedBuilders.Add(builder);
            checkCapacity <<= 1;
        }
        usedBuilders.ForEach(builder => StringBuilderCache.Release(builder));
        StringBuilder?[]? checkCacheList = StringBuilderCache._threadCache;
        // The cache list should contain all the builders we used
        checkCacheList.Should().BeEquivalentTo(usedBuilders);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests getting multiple <see cref="StringBuilder"/> instances from the cache for all the
    /// expected cacheable sizes, building a string and returning the instances, and verifying
    /// that they are stored in the internal <see cref="StringBuilderCache"/> cache lists.
    /// </summary>
    [Fact]
    public void UsingStringBuilderCache_AcquireMultipleBuildStringOfDifferentCapacitySizesAndRelease_InstancesStoredInCache ()
    {
        // Be sure our test starts with a cleared cache
        StringBuilderCache._threadCache = null;
        StringBuilderCache._globalCache = null;
        int perCapacityCount = StringBuilderCache.MaxGlobalCacheCount + 1;
        List<StringBuilder> perCapacityList = new(perCapacityCount);
        string addString = GetRandomString(10, StringBuilderCache.DefaultCapacity);
        int getCapacity = StringBuilderCache.DefaultCapacity;
        List<StringBuilder> usedBuilders = [];

        while (getCapacity <= StringBuilderCache.MaxCachedCapacity)
        {
            perCapacityList.Clear();
            for (int getBuildInstance = 0; getBuildInstance < perCapacityCount; getBuildInstance++)
            {
                StringBuilder builder = StringBuilderCache.Acquire(getCapacity);
                builder.Append(addString);
                usedBuilders.Add(builder);
                perCapacityList.Add(builder);
            }
            // Now, release them all, they should be placed in the thread static cache first,
            // then into the global cache
            foreach (StringBuilder builder in perCapacityList)
            {
                StringBuilderCache.Release(builder);
            }
            getCapacity <<= 1;
        }
        StringBuilder?[]? threadCacheList = StringBuilderCache._threadCache;
        threadCacheList.Should().NotBeNull();
        ConcurrentBag<StringBuilder>?[]? globalCacheList = StringBuilderCache._globalCache;
        globalCacheList.Should().NotBeNull();
        StringBuilder?[] checkCacheList = threadCacheList!.Where(builder => builder is not null)
            .Concat(globalCacheList!.SelectMany(list => list ?? [])).ToArray();

        // Display the value of each entry in the check cache list
        for (int checkIndex = 0; checkIndex < checkCacheList.Length; checkIndex++)
        {
            TestWriteLine($"Cache list entry {checkIndex} is {(checkCacheList[checkIndex] is { } builder ? builder.ToString() : "<<null>>")}");
        }
        checkCacheList.Should().BeEquivalentTo(usedBuilders);
    }
    //--------------------------------------------------------------------------------    

    //================================================================================

    #endregion Test Methods
}
//################################################################################
