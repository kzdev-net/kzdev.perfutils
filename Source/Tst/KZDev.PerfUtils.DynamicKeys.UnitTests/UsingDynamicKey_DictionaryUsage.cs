// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for using DynamicKey as Dictionary and HashSet keys
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKeyClass_DictionaryUsage : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKeyClass_DictionaryUsage"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKeyClass_DictionaryUsage (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Dictionary Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests using single DynamicKey as Dictionary key.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_SingleKeyAsDictionaryKey_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");

        dictionary[key1] = "value1";
        dictionary[key2] = "value2";

        dictionary.Should().HaveCount(2);
        dictionary[key1].Should().Be("value1");
        dictionary[key2].Should().Be("value2");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that same key values work correctly in Dictionary.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_SameKeyValuesInDictionary_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey(42); // Same value, should be same key

        dictionary[key1] = "value1";
        dictionary[key2] = "value2"; // Should overwrite

        dictionary.Should().HaveCount(1);
        dictionary[key1].Should().Be("value2");
        dictionary[key2].Should().Be("value2");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests using composite DynamicKey as Dictionary key.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_CompositeKeyAsDictionaryKey_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        DynamicKey composite1 = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));
        DynamicKey composite2 = DynamicKey.Combine(
            DynamicKey.GetKey(100),
            DynamicKey.GetKey("other"));

        dictionary[composite1] = "value1";
        dictionary[composite2] = "value2";

        dictionary.Should().HaveCount(2);
        dictionary[composite1].Should().Be("value1");
        dictionary[composite2].Should().Be("value2");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that same composite key values work correctly in Dictionary.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_SameCompositeKeyValuesInDictionary_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        DynamicKey composite1 = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));
        DynamicKey composite2 = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test")); // Same values, should be same key

        dictionary[composite1] = "value1";
        dictionary[composite2] = "value2"; // Should overwrite

        dictionary.Should().HaveCount(1);
        dictionary[composite1].Should().Be("value2");
        dictionary[composite2].Should().Be("value2");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests Dictionary.ContainsKey with DynamicKey.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_DictionaryContainsKey_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        DynamicKey key = DynamicKey.GetKey(42);
        dictionary[key] = "value";

        dictionary.ContainsKey(key).Should().BeTrue();
        dictionary.ContainsKey(DynamicKey.GetKey(42)).Should().BeTrue();
        dictionary.ContainsKey(DynamicKey.GetKey(100)).Should().BeFalse();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests Dictionary.TryGetValue with DynamicKey.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_DictionaryTryGetValue_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        DynamicKey key = DynamicKey.GetKey(42);
        dictionary[key] = "value";

        dictionary.TryGetValue(key, out string? value).Should().BeTrue();
        value.Should().Be("value");

        dictionary.TryGetValue(DynamicKey.GetKey(100), out string? notFound).Should().BeFalse();
        notFound.Should().BeNull();
    }
    //--------------------------------------------------------------------------------

    #endregion Dictionary Tests

    #region HashSet Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests using single DynamicKey as HashSet element.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_SingleKeyAsHashSetElement_WorksCorrectly ()
    {
        HashSet<DynamicKey> hashSet = [];

        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");

        hashSet.Add(key1).Should().BeTrue();
        hashSet.Add(key2).Should().BeTrue();
        hashSet.Add(key1).Should().BeFalse(); // Already exists

        hashSet.Should().HaveCount(2);
        hashSet.Should().Contain(key1);
        hashSet.Should().Contain(key2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests using composite DynamicKey as HashSet element.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_CompositeKeyAsHashSetElement_WorksCorrectly ()
    {
        HashSet<DynamicKey> hashSet = [];

        DynamicKey composite1 = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));
        DynamicKey composite2 = DynamicKey.Combine(
            DynamicKey.GetKey(100),
            DynamicKey.GetKey("other"));

        hashSet.Add(composite1).Should().BeTrue();
        hashSet.Add(composite2).Should().BeTrue();
        hashSet.Add(composite1).Should().BeFalse(); // Already exists

        hashSet.Should().HaveCount(2);
        hashSet.Should().Contain(composite1);
        hashSet.Should().Contain(composite2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that same composite key values work correctly in HashSet.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_SameCompositeKeyValuesInHashSet_WorksCorrectly ()
    {
        HashSet<DynamicKey> hashSet = [];

        DynamicKey composite1 = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));
        DynamicKey composite2 = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test")); // Same values, should be same key

        hashSet.Add(composite1).Should().BeTrue();
        hashSet.Add(composite2).Should().BeFalse(); // Already exists

        hashSet.Should().HaveCount(1);
        hashSet.Should().Contain(composite1);
        hashSet.Should().Contain(composite2);
    }
    //--------------------------------------------------------------------------------

    #endregion HashSet Tests

    #region Mixed Key Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests using mixed key types in the same Dictionary.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_MixedKeyTypesInDictionary_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        // Single keys
        dictionary[DynamicKey.GetKey(42)] = "int";
        dictionary[DynamicKey.GetKey("test")] = "string";
        dictionary[DynamicKey.GetKey(true)] = "bool";

        // Composite keys
        dictionary[DynamicKey.Combine(
            DynamicKey.GetKey(100),
            DynamicKey.GetKey("composite"))] = "composite";

        dictionary.Should().HaveCount(4);
        dictionary[DynamicKey.GetKey(42)].Should().Be("int");
        dictionary[DynamicKey.GetKey("test")].Should().Be("string");
        dictionary[DynamicKey.GetKey(true)].Should().Be("bool");
        dictionary[DynamicKey.Combine(
            DynamicKey.GetKey(100),
            DynamicKey.GetKey("composite"))].Should().Be("composite");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests using mixed key types in the same HashSet.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_MixedKeyTypesInHashSet_WorksCorrectly ()
    {
        HashSet<DynamicKey> hashSet = [];

        // Single keys
        hashSet.Add(DynamicKey.GetKey(42)).Should().BeTrue();
        hashSet.Add(DynamicKey.GetKey("test")).Should().BeTrue();
        hashSet.Add(DynamicKey.GetKey(true)).Should().BeTrue();

        // Composite keys
        hashSet.Add(DynamicKey.Combine(
            DynamicKey.GetKey(100),
            DynamicKey.GetKey("composite"))).Should().BeTrue();

        hashSet.Should().HaveCount(4);
        hashSet.Should().Contain(DynamicKey.GetKey(42));
        hashSet.Should().Contain(DynamicKey.GetKey("test"));
        hashSet.Should().Contain(DynamicKey.GetKey(true));
        hashSet.Should().Contain(DynamicKey.Combine(
            DynamicKey.GetKey(100),
            DynamicKey.GetKey("composite")));
    }
    //--------------------------------------------------------------------------------

    #endregion Mixed Key Type Tests

    #region Performance Simulation Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests multiple operations with DynamicKey in Dictionary to simulate real usage.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_MultipleDictionaryOperations_WorksCorrectly ()
    {
        Dictionary<DynamicKey, string> dictionary = new();

        // Add multiple entries
        for (int i = 0; i < 100; i++)
        {
            DynamicKey key = DynamicKey.Combine(
                DynamicKey.GetKey(i),
                DynamicKey.GetKey($"item{i}"));
            dictionary[key] = $"value{i}";
        }

        dictionary.Should().HaveCount(100);

        // Verify all entries
        for (int i = 0; i < 100; i++)
        {
            DynamicKey key = DynamicKey.Combine(
                DynamicKey.GetKey(i),
                DynamicKey.GetKey($"item{i}"));
            dictionary[key].Should().Be($"value{i}");
        }

        // Remove some entries
        for (int i = 0; i < 50; i++)
        {
            DynamicKey key = DynamicKey.Combine(
                DynamicKey.GetKey(i),
                DynamicKey.GetKey($"item{i}"));
            dictionary.Remove(key).Should().BeTrue();
        }

        dictionary.Should().HaveCount(50);
    }
    //--------------------------------------------------------------------------------

    #endregion Performance Simulation Tests
}
//################################################################################
