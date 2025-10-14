// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicRefKey&lt;T&gt; reference type key functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_RefKeyTests : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKey_RefKeyTests"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKey_RefKeyTests (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Basic Reference Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey&lt;object&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_ObjectKey_CreationAndCaching_WorksCorrectly ()
    {
        object testValue = new { Name = "test", Value = 42 };
        DynamicKey key1 = DynamicRefKey<object>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<object>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<object>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey&lt;string&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_StringKey_CreationAndCaching_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key1 = DynamicRefKey<string>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<string>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicStringKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey&lt;List&lt;int&gt;&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_ListKey_CreationAndCaching_WorksCorrectly ()
    {
        List<int> testValue = [1, 2, 3, 4, 5];
        DynamicKey key1 = DynamicRefKey<List<int>>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<List<int>>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<List<int>>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey&lt;Dictionary&lt;string, int&gt;&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_DictionaryKey_CreationAndCaching_WorksCorrectly ()
    {
        Dictionary<string, int> testValue = new() { ["a"] = 1, ["b"] = 2 };
        DynamicKey key1 = DynamicRefKey<Dictionary<string, int>>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<Dictionary<string, int>>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<Dictionary<string, int>>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Basic Reference Type Tests

    #region Custom Class Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey with custom class.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_CustomClassKey_CreationAndCaching_WorksCorrectly ()
    {
        TestClass testValue = new() { Id = 42, Name = "test" };
        DynamicKey key1 = DynamicRefKey<TestClass>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<TestClass>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<TestClass>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey with custom class equality.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_CustomClassKey_Equality_WorksCorrectly ()
    {
        TestClass testValue1 = new() { Id = 42, Name = "test" };
        TestClass testValue2 = new() { Id = 42, Name = "test" };

        DynamicKey key1 = DynamicRefKey<TestClass>.GetKey(testValue1);
        DynamicKey key2 = DynamicRefKey<TestClass>.GetKey(testValue2);

        // Different instances should create different keys
        key1.Should().NotBe(key2);
        key1.GetHashCode().Should().NotBe(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Custom Class Tests

    #region Null Handling Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey null handling.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_NullHandling_WorksCorrectly ()
    {
        DynamicKey key = DynamicRefKey<object>.GetKey(null!);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey string null handling.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_StringNullHandling_WorksCorrectly ()
    {
        DynamicKey key = DynamicRefKey<string>.GetKey(null!);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------

    #endregion Null Handling Tests

    #region Equality Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey equality with same reference.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_SameReferenceEquality_WorksCorrectly ()
    {
        object testValue = new { Name = "test" };
        DynamicKey key1 = DynamicRefKey<object>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<object>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey equality with different references.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_DifferentReferenceEquality_WorksCorrectly ()
    {
        object testValue1 = new { Name = "test" };
        object testValue2 = new { Name = "test" };

        DynamicKey key1 = DynamicRefKey<object>.GetKey(testValue1);
        DynamicKey key2 = DynamicRefKey<object>.GetKey(testValue2);

        // Different object instances should create different keys
        key1.Should().NotBeSameAs(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey equality with string values.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_StringEquality_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key1 = DynamicRefKey<string>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<string>.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Equality Tests

    #region Comparison Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey comparison with same type.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_SameTypeComparison_WorksCorrectly ()
    {
        string testValue1 = "apple";
        string testValue2 = "banana";

        DynamicKey key1 = DynamicRefKey<string>.GetKey(testValue1);
        DynamicKey key2 = DynamicRefKey<string>.GetKey(testValue2);

        int comparison = key1.CompareTo(key2);
        comparison.Should().NotBe(0);

        // Verify symmetry
        key2.CompareTo(key1).Should().Be(-comparison);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey comparison with same reference.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_SameReferenceComparison_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key1 = DynamicRefKey<string>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<string>.GetKey(testValue);

        key1.CompareTo(key2).Should().Be(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey comparison with different types.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_DifferentTypeComparison_WorksCorrectly ()
    {
        string testValue1 = "test";
        object testValue2 = new { Name = "test" };

        DynamicKey key1 = DynamicRefKey<string>.GetKey(testValue1);
        DynamicKey key2 = DynamicRefKey<object>.GetKey(testValue2);

        int comparison = key1.CompareTo(key2);
        comparison.Should().NotBe(0);
    }
    //--------------------------------------------------------------------------------

    #endregion Comparison Tests

    #region Hash Code Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey hash code consistency.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_HashCodeConsistency_WorksCorrectly ()
    {
        object testValue = new { Name = "test" };
        DynamicKey key1 = DynamicRefKey<object>.GetKey(testValue);
        DynamicKey key2 = DynamicRefKey<object>.GetKey(testValue);

        int hash1 = key1.GetHashCode();
        int hash2 = key2.GetHashCode();

        hash1.Should().Be(hash2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey hash code distribution.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_HashCodeDistribution_WorksCorrectly ()
    {
        object testValue1 = new { Name = "test1" };
        object testValue2 = new { Name = "test2" };

        DynamicKey key1 = DynamicRefKey<object>.GetKey(testValue1);
        DynamicKey key2 = DynamicRefKey<object>.GetKey(testValue2);

        int hash1 = key1.GetHashCode();
        int hash2 = key2.GetHashCode();

        hash1.Should().NotBe(hash2);
    }
    //--------------------------------------------------------------------------------

    #endregion Hash Code Tests

    #region ToString Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey ToString functionality.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_ToString_WorksCorrectly ()
    {
        object testValue = new { Name = "test", Value = 42 };
        DynamicKey key = DynamicRefKey<object>.GetKey(testValue);

        string result = key.ToString();
        result.Should().NotBeNullOrEmpty();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey ToString with string value.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_StringToString_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key = DynamicRefKey<string>.GetKey(testValue);

        string result = key.ToString();
        result.Should().NotBeNullOrEmpty();
    }
    //--------------------------------------------------------------------------------

    #endregion ToString Tests

    #region Thread Safety Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicRefKey thread safety with concurrent access.
    /// </summary>
    [Fact]
    public void UsingDynamicRefKey_ThreadSafety_WorksCorrectly ()
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
                    object testValue = new { ThreadId = threadId, Iteration = j };
                    DynamicKey key = DynamicRefKey<object>.GetKey(testValue);

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
    /// Test class for reference type testing.
    /// </summary>
    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    //--------------------------------------------------------------------------------

    #endregion Helper Classes
}
//################################################################################
