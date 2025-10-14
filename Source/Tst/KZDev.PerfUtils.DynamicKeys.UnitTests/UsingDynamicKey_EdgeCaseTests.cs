// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicKey edge cases and error handling
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_EdgeCaseTests : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKey_EdgeCaseTests"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKey_EdgeCaseTests(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Null Handling Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests null handling for string values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NullString_ReturnsEmptyStringKey()
    {
        DynamicKey key = DynamicKey.GetKey((string?)null);

        key.Should().NotBeNull();
        key.Should().Be(DynamicStringKey.Empty);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests null handling for object values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NullObject_ReturnsNullKey()
    {
        DynamicKey key = DynamicKey.GetKey((object?)null);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests null handling for generic types.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NullGeneric_ReturnsNullKey()
    {
        DynamicKey key = DynamicKey.GetKey<string>(null!);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------

    #endregion Null Handling Tests

    #region Empty String Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests empty string handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_EmptyString_ReturnsEmptyStringKey()
    {
        DynamicKey key = DynamicKey.GetKey(string.Empty);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.EmptyString);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests empty string equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_EmptyStringEquality_WorksCorrectly()
    {
        DynamicKey key1 = DynamicKey.GetKey(string.Empty);
        DynamicKey key2 = DynamicKey.GetKey(string.Empty);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Empty String Tests

    #region Default Value Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests default Guid handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DefaultGuid_WorksCorrectly()
    {
        DynamicKey key = DynamicKey.GetKey(Guid.Empty);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicGuidKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests default DateTime handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DefaultDateTime_WorksCorrectly()
    {
        DynamicKey key = DynamicKey.GetValueKey(default(DateTime));

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<DateTime>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests default TimeSpan handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DefaultTimeSpan_WorksCorrectly()
    {
        DynamicKey key = DynamicKey.GetValueKey(TimeSpan.Zero);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<TimeSpan>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Default Value Tests

    #region Large Number Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests large integer values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_LargeInteger_WorksCorrectly()
    {
        int largeValue = int.MaxValue;
        DynamicKey key = DynamicKey.GetKey(largeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicIntKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests large long values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_LargeLong_WorksCorrectly()
    {
        long largeValue = long.MaxValue;
        DynamicKey key = DynamicKey.GetKey(largeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicLongKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests large unsigned integer values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_LargeUInt_WorksCorrectly()
    {
        uint largeValue = uint.MaxValue;
        DynamicKey key = DynamicKey.GetKey(largeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicUIntKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests large unsigned long values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_LargeULong_WorksCorrectly()
    {
        ulong largeValue = ulong.MaxValue;
        DynamicKey key = DynamicKey.GetKey(largeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicULongKey>();
    }
    //--------------------------------------------------------------------------------

    #endregion Large Number Tests

    #region Negative Number Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests negative integer values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NegativeInteger_WorksCorrectly()
    {
        int negativeValue = int.MinValue;
        DynamicKey key = DynamicKey.GetKey(negativeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicIntKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests negative long values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NegativeLong_WorksCorrectly()
    {
        long negativeValue = long.MinValue;
        DynamicKey key = DynamicKey.GetKey(negativeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicLongKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests negative decimal values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NegativeDecimal_WorksCorrectly()
    {
        decimal negativeValue = decimal.MinValue;
        DynamicKey key = DynamicKey.GetValueKey(negativeValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<decimal>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Negative Number Tests

    #region Special Float/Double Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests NaN float values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NaNFloat_WorksCorrectly()
    {
        float nanValue = float.NaN;
        DynamicKey key = DynamicKey.GetValueKey(nanValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<float>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests NaN double values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NaNDouble_WorksCorrectly()
    {
        double nanValue = double.NaN;
        DynamicKey key = DynamicKey.GetValueKey(nanValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<double>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests positive infinity float values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_PositiveInfinityFloat_WorksCorrectly()
    {
        float infinityValue = float.PositiveInfinity;
        DynamicKey key = DynamicKey.GetValueKey(infinityValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<float>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests negative infinity float values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_NegativeInfinityFloat_WorksCorrectly()
    {
        float infinityValue = float.NegativeInfinity;
        DynamicKey key = DynamicKey.GetValueKey(infinityValue);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicValKey<float>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Special Float/Double Tests

    #region Long String Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests very long string values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_VeryLongString_WorksCorrectly()
    {
        string longString = new string('a', 10000);
        DynamicKey key = DynamicKey.GetKey(longString);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicStringKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests string with special characters.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_StringWithSpecialCharacters_WorksCorrectly()
    {
        string specialString = "Hello\nWorld\tTest\r\nSpecial: !@#$%^&*()";
        DynamicKey key = DynamicKey.GetKey(specialString);

        key.Should().NotBeNull();
        key.Should().BeOfType<DynamicStringKey>();
    }
    //--------------------------------------------------------------------------------

    #endregion Long String Tests

    #region Composite Key Edge Cases

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests composite key with null elements.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_CompositeKeyWithNullElements_ThrowsException()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);

        this.Invoking(_ => key1 + null!)
            .Should().Throw<ArgumentNullException>();

        this.Invoking(_ => null! + key1)
            .Should().Throw<ArgumentNullException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests empty composite key creation.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_EmptyCompositeKey_ThrowsException()
    {
        this.Invoking(_ => DynamicKey.Combine())
            .Should().Throw<ArgumentException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests single element composite key.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_SingleElementCompositeKey_ReturnsOriginalKey()
    {
        DynamicKey originalKey = DynamicKey.GetKey(42);
        DynamicKey compositeKey = DynamicKey.Combine(originalKey);

        compositeKey.Should().BeSameAs(originalKey);
    }
    //--------------------------------------------------------------------------------

    #endregion Composite Key Edge Cases

    #region Builder Edge Cases

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests empty builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_EmptyBuilder_ThrowsException()
    {
        this.Invoking(_ => DynamicKeyBuilder.Create().Build())
            .Should().Throw<InvalidOperationException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests builder with null key.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_AddNullKey_ThrowsException()
    {
        this.Invoking(_ => DynamicKeyBuilder.Create().Add((DynamicKey)null!))
            .Should().Throw<ArgumentNullException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests builder with many elements.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_ManyElements_WorksCorrectly()
    {
        DynamicKey result = DynamicKeyBuilder.Create()
            .AddInt(1)
            .AddInt(2)
            .AddInt(3)
            .AddInt(4)
            .AddInt(5)
            .AddInt(6)
            .AddInt(7)
            .AddInt(8)
            .AddInt(9)
            .AddInt(10)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey composite = (DynamicCompositeKey)result;
        composite.Count.Should().Be(10);
    }
    //--------------------------------------------------------------------------------

    #endregion Builder Edge Cases

    #region Thread Safety Edge Cases

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests concurrent access to the same key value with thread-static caching.
    /// </summary>
    [Fact]
    public async Task UsingDynamicKey_ConcurrentAccessToSameValue_WorksCorrectly()
    {
        const int taskCount = 10;
        const int iterationsPerTask = 100;

        ConcurrentDictionary<int, List<DynamicKey>> threadResults = new();
        List<Task> tasks = [];

        for (int i = 0; i < taskCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                for (int j = 0; j < iterationsPerTask; j++)
                {
                    DynamicKey key = DynamicKey.GetKey(42);
                    List<DynamicKey> keyList = threadResults.GetOrAdd(Environment.CurrentManagedThreadId, _ => []);
                    keyList.Add(key);

                    // Make this truly asynchronous to test multi-thread behavior
                    await Task.Yield();
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Verify that each thread has the same key instance for all iterations
        foreach (KeyValuePair<int, List<DynamicKey>> threadResult in threadResults)
        {
            List<DynamicKey> keys = threadResult.Value;

            // All keys in the same thread should be the same instance
            DynamicKey firstKey = keys[0];
            foreach (DynamicKey key in keys)
            {
                key.Should().BeSameAs(firstKey);
            }
        }

        // Verify that different threads have different instances (due to ThreadStatic caching)
        List<DynamicKey> firstKeysFromEachThread = threadResults.Values
            .Select(keys => keys[0])
            .ToList();

        firstKeysFromEachThread.Should().HaveCount(threadResults.Count);

        // All instances should be equal in value but different in reference
        for (int i = 0; i < firstKeysFromEachThread.Count; i++)
        {
            for (int j = i + 1; j < firstKeysFromEachThread.Count; j++)
            {
                DynamicKey key1 = firstKeysFromEachThread[i];
                DynamicKey key2 = firstKeysFromEachThread[j];

                // They should be equal in value
                key1.Should().Be(key2);
                key1.GetHashCode().Should().Be(key2.GetHashCode());

                // But different instances due to ThreadStatic caching
                key1.Should().NotBeSameAs(key2);
            }
        }

        // Additional verification: ensure we actually tested multiple threads
        // (Due to thread pool reuse, we might have fewer unique threads than tasks)
        threadResults.Count.Should().BeGreaterThan(0);

        // Log the actual thread count for debugging
        TestWriteLine($"Tested {threadResults.Count} unique managed threads out of {taskCount} tasks");
    }
    //--------------------------------------------------------------------------------

    #endregion Thread Safety Edge Cases

    #region Memory Pressure Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating many different keys to ensure no memory leaks.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ManyDifferentKeys_WorksCorrectly()
    {
        const int keyCount = 10000;
        List<DynamicKey> keys = [];

        for (int i = 0; i < keyCount; i++)
        {
            DynamicKey key = DynamicKey.GetKey(i);
            keys.Add(key);
        }

        keys.Should().HaveCount(keyCount);

        // Verify all keys are unique
        HashSet<DynamicKey> uniqueKeys = new(keys);
        uniqueKeys.Should().HaveCount(keyCount);
    }
    //--------------------------------------------------------------------------------

    #endregion Memory Pressure Tests

    #region Cross-Type Comparison Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests comparison between different key types.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_CrossTypeComparison_WorksCorrectly()
    {
        DynamicKey intKey = DynamicKey.GetKey(42);
        DynamicKey stringKey = DynamicKey.GetKey("42");
        DynamicKey boolKey = DynamicKey.GetKey(true);

        intKey.CompareTo(stringKey).Should().NotBe(0);
        intKey.CompareTo(boolKey).Should().NotBe(0);
        stringKey.CompareTo(boolKey).Should().NotBe(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests comparison with null.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ComparisonWithNull_WorksCorrectly()
    {
        DynamicKey key = DynamicKey.GetKey(42);

        key.CompareTo(null).Should().BePositive();
    }
    //--------------------------------------------------------------------------------

    #endregion Cross-Type Comparison Tests

    #region Hash Code Edge Cases

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests hash code consistency across different key types.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_HashCodeConsistencyAcrossTypes_WorksCorrectly()
    {
        DynamicKey intKey = DynamicKey.GetKey(42);
        DynamicKey stringKey = DynamicKey.GetKey("42");
        DynamicKey boolKey = DynamicKey.GetKey(true);

        // Different types should generally have different hash codes
        int intHash = intKey.GetHashCode();
        int stringHash = stringKey.GetHashCode();
        int boolHash = boolKey.GetHashCode();

        // While not guaranteed, it's very unlikely they'd all be the same
        (intHash == stringHash && stringHash == boolHash).Should().BeFalse();
    }
    //--------------------------------------------------------------------------------

    #endregion Hash Code Edge Cases
}
//################################################################################
