// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;
using DynamicKeyClass = KZDev.PerfUtils.DynamicKey;

namespace KZDev.PerfUtils.Tests.DynamicKey;

//################################################################################
/// <summary>
/// Unit tests for using DynamicKeyClass as Dictionary and HashSet keys
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKeyClass")]
public class UsingDynamicKeyClass_DictionaryUsage : UnitTestBase
{
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Initializes a new instance of the <see cref="UsingDynamicKeyClass_DictionaryUsage"/> class.
  /// </summary>
  /// <param name="xUnitTestOutputHelper">
  /// The Xunit test output helper that can be used to output test messages
  /// </param>
  public UsingDynamicKeyClass_DictionaryUsage(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
  {
  }
  //--------------------------------------------------------------------------------

  #region Dictionary Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests using single DynamicKeyClass as Dictionary key.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_SingleKeyAsDictionaryKey_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");

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
  public void UsingDynamicKeyClass_SameKeyValuesInDictionary_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(42); // Same value, should be same key

    dictionary[key1] = "value1";
    dictionary[key2] = "value2"; // Should overwrite

    dictionary.Should().HaveCount(1);
    dictionary[key1].Should().Be("value2");
    dictionary[key2].Should().Be("value2");
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests using composite DynamicKeyClass as Dictionary key.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_CompositeKeyAsDictionaryKey_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(42),
        DynamicKeyClass.GetKey("test"));
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(100),
        DynamicKeyClass.GetKey("other"));

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
  public void UsingDynamicKeyClass_SameCompositeKeyValuesInDictionary_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(42),
        DynamicKeyClass.GetKey("test"));
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(42),
        DynamicKeyClass.GetKey("test")); // Same values, should be same key

    dictionary[composite1] = "value1";
    dictionary[composite2] = "value2"; // Should overwrite

    dictionary.Should().HaveCount(1);
    dictionary[composite1].Should().Be("value2");
    dictionary[composite2].Should().Be("value2");
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests Dictionary.ContainsKey with DynamicKeyClass.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_DictionaryContainsKey_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    DynamicKeyClass key = DynamicKeyClass.GetKey(42);
    dictionary[key] = "value";

    dictionary.ContainsKey(key).Should().BeTrue();
    dictionary.ContainsKey(DynamicKeyClass.GetKey(42)).Should().BeTrue();
    dictionary.ContainsKey(DynamicKeyClass.GetKey(100)).Should().BeFalse();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests Dictionary.TryGetValue with DynamicKeyClass.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_DictionaryTryGetValue_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    DynamicKeyClass key = DynamicKeyClass.GetKey(42);
    dictionary[key] = "value";

    dictionary.TryGetValue(key, out string? value).Should().BeTrue();
    value.Should().Be("value");

    dictionary.TryGetValue(DynamicKeyClass.GetKey(100), out string? notFound).Should().BeFalse();
    notFound.Should().BeNull();
  }
  //--------------------------------------------------------------------------------

  #endregion Dictionary Tests

  #region HashSet Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests using single DynamicKeyClass as HashSet element.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_SingleKeyAsHashSetElement_WorksCorrectly()
  {
    HashSet<DynamicKeyClass> hashSet = new();

    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");

    hashSet.Add(key1).Should().BeTrue();
    hashSet.Add(key2).Should().BeTrue();
    hashSet.Add(key1).Should().BeFalse(); // Already exists

    hashSet.Should().HaveCount(2);
    hashSet.Should().Contain(key1);
    hashSet.Should().Contain(key2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests using composite DynamicKeyClass as HashSet element.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_CompositeKeyAsHashSetElement_WorksCorrectly()
  {
    HashSet<DynamicKeyClass> hashSet = new();

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(42),
        DynamicKeyClass.GetKey("test"));
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(100),
        DynamicKeyClass.GetKey("other"));

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
  public void UsingDynamicKeyClass_SameCompositeKeyValuesInHashSet_WorksCorrectly()
  {
    HashSet<DynamicKeyClass> hashSet = new();

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(42),
        DynamicKeyClass.GetKey("test"));
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(42),
        DynamicKeyClass.GetKey("test")); // Same values, should be same key

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
  public void UsingDynamicKeyClass_MixedKeyTypesInDictionary_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    // Single keys
    dictionary[DynamicKeyClass.GetKey(42)] = "int";
    dictionary[DynamicKeyClass.GetKey("test")] = "string";
    dictionary[DynamicKeyClass.GetKey(true)] = "bool";

    // Composite keys
    dictionary[DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(100),
        DynamicKeyClass.GetKey("composite"))] = "composite";

    dictionary.Should().HaveCount(4);
    dictionary[DynamicKeyClass.GetKey(42)].Should().Be("int");
    dictionary[DynamicKeyClass.GetKey("test")].Should().Be("string");
    dictionary[DynamicKeyClass.GetKey(true)].Should().Be("bool");
    dictionary[DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(100),
        DynamicKeyClass.GetKey("composite"))].Should().Be("composite");
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests using mixed key types in the same HashSet.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_MixedKeyTypesInHashSet_WorksCorrectly()
  {
    HashSet<DynamicKeyClass> hashSet = new();

    // Single keys
    hashSet.Add(DynamicKeyClass.GetKey(42)).Should().BeTrue();
    hashSet.Add(DynamicKeyClass.GetKey("test")).Should().BeTrue();
    hashSet.Add(DynamicKeyClass.GetKey(true)).Should().BeTrue();

    // Composite keys
    hashSet.Add(DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(100),
        DynamicKeyClass.GetKey("composite"))).Should().BeTrue();

    hashSet.Should().HaveCount(4);
    hashSet.Should().Contain(DynamicKeyClass.GetKey(42));
    hashSet.Should().Contain(DynamicKeyClass.GetKey("test"));
    hashSet.Should().Contain(DynamicKeyClass.GetKey(true));
    hashSet.Should().Contain(DynamicKeyClass.Combine(
        DynamicKeyClass.GetKey(100),
        DynamicKeyClass.GetKey("composite")));
  }
  //--------------------------------------------------------------------------------

  #endregion Mixed Key Type Tests

  #region Performance Simulation Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests multiple operations with DynamicKeyClass in Dictionary to simulate real usage.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_MultipleDictionaryOperations_WorksCorrectly()
  {
    Dictionary<DynamicKeyClass, string> dictionary = new();

    // Add multiple entries
    for (int i = 0; i < 100; i++)
    {
      DynamicKeyClass key = DynamicKeyClass.Combine(
          DynamicKeyClass.GetKey(i),
          DynamicKeyClass.GetKey($"item{i}"));
      dictionary[key] = $"value{i}";
    }

    dictionary.Should().HaveCount(100);

    // Verify all entries
    for (int i = 0; i < 100; i++)
    {
      DynamicKeyClass key = DynamicKeyClass.Combine(
          DynamicKeyClass.GetKey(i),
          DynamicKeyClass.GetKey($"item{i}"));
      dictionary[key].Should().Be($"value{i}");
    }

    // Remove some entries
    for (int i = 0; i < 50; i++)
    {
      DynamicKeyClass key = DynamicKeyClass.Combine(
          DynamicKeyClass.GetKey(i),
          DynamicKeyClass.GetKey($"item{i}"));
      dictionary.Remove(key).Should().BeTrue();
    }

    dictionary.Should().HaveCount(50);
  }
  //--------------------------------------------------------------------------------

  #endregion Performance Simulation Tests
}
//################################################################################
