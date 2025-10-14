// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicValKey&lt;T&gt; value type key functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_ValKeyTests : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKey_ValKeyTests"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKey_ValKeyTests (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region DateTime Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;DateTime&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DateTimeKey_CreationAndCaching_WorksCorrectly ()
    {
        DateTime testValue = new(2023, 12, 25, 10, 30, 0);
        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<DateTime>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;DateTime&gt; equality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DateTimeKey_Equality_WorksCorrectly ()
    {
        DateTime testValue = new(2023, 12, 25, 10, 30, 0);
        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;DateTime&gt; comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DateTimeKey_Comparison_WorksCorrectly ()
    {
        DateTime testValue1 = new(2023, 12, 25, 10, 30, 0);
        DateTime testValue2 = new(2023, 12, 26, 10, 30, 0);

        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue2);

        key1.CompareTo(key2).Should().BeNegative();
        key2.CompareTo(key1).Should().BePositive();
        key1.CompareTo(key1).Should().Be(0);
    }
    //--------------------------------------------------------------------------------

    #endregion DateTime Tests

    #region TimeSpan Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;TimeSpan&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_TimeSpanKey_CreationAndCaching_WorksCorrectly ()
    {
        TimeSpan testValue = TimeSpan.FromMinutes(30);
        DynamicKey key1 = DynamicValKey<TimeSpan>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<TimeSpan>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<TimeSpan>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;TimeSpan&gt; equality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_TimeSpanKey_Equality_WorksCorrectly ()
    {
        TimeSpan testValue = TimeSpan.FromHours(2);
        DynamicKey key1 = DynamicValKey<TimeSpan>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<TimeSpan>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;TimeSpan&gt; comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_TimeSpanKey_Comparison_WorksCorrectly ()
    {
        TimeSpan testValue1 = TimeSpan.FromMinutes(30);
        TimeSpan testValue2 = TimeSpan.FromHours(1);

        DynamicKey key1 = DynamicValKey<TimeSpan>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<TimeSpan>.GetKey(testValue2);

        key1.CompareTo(key2).Should().BeNegative();
        key2.CompareTo(key1).Should().BePositive();
        key1.CompareTo(key1).Should().Be(0);
    }
    //--------------------------------------------------------------------------------

    #endregion TimeSpan Tests

    #region Decimal Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;decimal&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DecimalKey_CreationAndCaching_WorksCorrectly ()
    {
        decimal testValue = 123.45m;
        DynamicKey key1 = DynamicValKey<decimal>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<decimal>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<decimal>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;decimal&gt; equality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DecimalKey_Equality_WorksCorrectly ()
    {
        decimal testValue = 999.99m;
        DynamicKey key1 = DynamicValKey<decimal>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<decimal>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;decimal&gt; comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DecimalKey_Comparison_WorksCorrectly ()
    {
        decimal testValue1 = 100.50m;
        decimal testValue2 = 200.75m;

        DynamicKey key1 = DynamicValKey<decimal>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<decimal>.GetKey(testValue2);

        key1.CompareTo(key2).Should().BeNegative();
        key2.CompareTo(key1).Should().BePositive();
        key1.CompareTo(key1).Should().Be(0);
    }
    //--------------------------------------------------------------------------------

    #endregion Decimal Tests

    #region Float Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;float&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_FloatKey_CreationAndCaching_WorksCorrectly ()
    {
        float testValue = 3.14f;
        DynamicKey key1 = DynamicValKey<float>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<float>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<float>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;float&gt; equality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_FloatKey_Equality_WorksCorrectly ()
    {
        float testValue = 2.718f;
        DynamicKey key1 = DynamicValKey<float>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<float>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;float&gt; comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_FloatKey_Comparison_WorksCorrectly ()
    {
        float testValue1 = 1.5f;
        float testValue2 = 2.5f;

        DynamicKey key1 = DynamicValKey<float>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<float>.GetKey(testValue2);

        key1.CompareTo(key2).Should().BeNegative();
        key2.CompareTo(key1).Should().BePositive();
        key1.CompareTo(key1).Should().Be(0);
    }
    //--------------------------------------------------------------------------------

    #endregion Float Tests

    #region Double Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;double&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DoubleKey_CreationAndCaching_WorksCorrectly ()
    {
        double testValue = 3.14159;
        DynamicKey key1 = DynamicValKey<double>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<double>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<double>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;double&gt; equality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DoubleKey_Equality_WorksCorrectly ()
    {
        double testValue = 2.718281828;
        DynamicKey key1 = DynamicValKey<double>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<double>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey&lt;double&gt; comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DoubleKey_Comparison_WorksCorrectly ()
    {
        double testValue1 = 1.234567;
        double testValue2 = 2.345678;

        DynamicKey key1 = DynamicValKey<double>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<double>.GetKey(testValue2);

        key1.CompareTo(key2).Should().BeNegative();
        key2.CompareTo(key1).Should().BePositive();
        key1.CompareTo(key1).Should().Be(0);
    }
    //--------------------------------------------------------------------------------

    #endregion Double Tests

    #region Custom Value Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with custom value type.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_CustomValueTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        TestValueType testValue = new(42, "test");
        DynamicKey key1 = DynamicValKey<TestValueType>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<TestValueType>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<TestValueType>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with custom value type equality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_CustomValueTypeKey_Equality_WorksCorrectly ()
    {
        TestValueType testValue1 = new(42, "test");
        TestValueType testValue2 = new(42, "test");

        DynamicKey key1 = DynamicValKey<TestValueType>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<TestValueType>.GetKey(testValue2);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with custom value type comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_CustomValueTypeKey_Comparison_WorksCorrectly ()
    {
        TestValueType testValue1 = new(10, "a");
        TestValueType testValue2 = new(20, "b");

        DynamicKey key1 = DynamicValKey<TestValueType>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<TestValueType>.GetKey(testValue2);

        int comparison = key1.CompareTo(key2);
        comparison.Should().NotBe(0);

        // Verify symmetry
        key2.CompareTo(key1).Should().Be(-comparison);
    }
    //--------------------------------------------------------------------------------

    #endregion Custom Value Type Tests

    #region Special Value Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with DateTime.MinValue.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DateTimeMinValue_WorksCorrectly ()
    {
        DateTime testValue = DateTime.MinValue;
        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with DateTime.MaxValue.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DateTimeMaxValue_WorksCorrectly ()
    {
        DateTime testValue = DateTime.MaxValue;
        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with TimeSpan.Zero.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_TimeSpanZero_WorksCorrectly ()
    {
        TimeSpan testValue = TimeSpan.Zero;
        DynamicKey key1 = DynamicValKey<TimeSpan>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<TimeSpan>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey with decimal.Zero.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_DecimalZero_WorksCorrectly ()
    {
        decimal testValue = decimal.Zero;
        DynamicKey key1 = DynamicValKey<decimal>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<decimal>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
    }
    //--------------------------------------------------------------------------------

    #endregion Special Value Tests

    #region Hash Code Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey hash code consistency.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_HashCodeConsistency_WorksCorrectly ()
    {
        DateTime testValue = new(2023, 12, 25);
        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue);

        int hash1 = key1.GetHashCode();
        int hash2 = key2.GetHashCode();

        hash1.Should().Be(hash2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey hash code distribution.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_HashCodeDistribution_WorksCorrectly ()
    {
        DateTime testValue1 = new(2023, 12, 25);
        DateTime testValue2 = new(2023, 12, 26);

        DynamicKey key1 = DynamicValKey<DateTime>.GetKey(testValue1);
        DynamicKey key2 = DynamicValKey<DateTime>.GetKey(testValue2);

        int hash1 = key1.GetHashCode();
        int hash2 = key2.GetHashCode();

        hash1.Should().NotBe(hash2);
    }
    //--------------------------------------------------------------------------------

    #endregion Hash Code Tests

    #region ToString Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey ToString functionality.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_ToString_WorksCorrectly ()
    {
        DateTime testValue = new(2023, 12, 25, 10, 30, 0);
        DynamicKey key = DynamicValKey<DateTime>.GetKey(testValue);

        string result = key.ToString();
        result.Should().NotBeNullOrEmpty();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey ToString with TimeSpan.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_TimeSpanToString_WorksCorrectly ()
    {
        TimeSpan testValue = TimeSpan.FromHours(2);
        DynamicKey key = DynamicValKey<TimeSpan>.GetKey(testValue);

        string result = key.ToString();
        result.Should().NotBeNullOrEmpty();
    }
    //--------------------------------------------------------------------------------

    #endregion ToString Tests

    #region Thread Safety Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicValKey thread safety with concurrent access.
    /// </summary>
    [Fact]
    public void UsingDynamicValKey_ThreadSafety_WorksCorrectly ()
    {
        const int threadCount = 10;
        const int iterationsPerThread = 100;

        List<Task> tasks = [];
        List<DynamicKey> allKeys = [];
        object lockObject = new();

        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterationsPerThread; j++)
                {
                    DateTime testValue = new DateTime(2023, 1, 1).AddDays(threadId * 100 + j);
                    DynamicKey key = DynamicValKey<DateTime>.GetKey(testValue);

                    lock (lockObject)
                    {
                        allKeys.Add(key);
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        allKeys.Should().HaveCount(threadCount * iterationsPerThread);
    }
    //--------------------------------------------------------------------------------

    #endregion Thread Safety Tests

    #region Helper Classes

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Test value type for value type testing.
    /// </summary>
    private struct TestValueType : IComparable<TestValueType>
    {
        public int Id { get; }
        public string Name { get; }

        public TestValueType (int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int CompareTo (TestValueType other)
        {
            int idComparison = Id.CompareTo(other.Id);
            return idComparison != 0 ? idComparison : string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public override bool Equals (object? obj)
        {
            return obj is TestValueType other && Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode ()
        {
            return HashCode.Combine(Id, Name);
        }
    }
    //--------------------------------------------------------------------------------

    #endregion Helper Classes
}
//################################################################################
