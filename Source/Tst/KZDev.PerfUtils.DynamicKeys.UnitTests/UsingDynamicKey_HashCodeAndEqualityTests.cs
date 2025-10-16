// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicKey HashCode generation and Equality testing
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_HashCodeAndEqualityTests : UnitTestBase
{
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Initializes a new instance of the <see cref="UsingDynamicKey_HashCodeAndEqualityTests"/> class.
  /// </summary>
  /// <param name="xUnitTestOutputHelper">
  /// The Xunit test output helper that can be used to output test messages
  /// </param>
  public UsingDynamicKey_HashCodeAndEqualityTests(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
  {
  }
  //--------------------------------------------------------------------------------

  #region HashCode Consistency Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that the same value always produces the same hash code.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_SameValue_SameHashCode()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(42);

    int hash1 = key1.GetHashCode();
    int hash2 = key2.GetHashCode();

    hash1.Should().Be(hash2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different values produce different hash codes.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DifferentValues_DifferentHashCodes()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    int hash1 = key1.GetHashCode();
    int hash2 = key2.GetHashCode();

    hash1.Should().NotBe(hash2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code consistency across multiple calls.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_HashCodeConsistency_MultipleCalls()
  {
    DynamicKey key = DynamicKey.GetKey(42);

    int hash1 = key.GetHashCode();
    int hash2 = key.GetHashCode();
    int hash3 = key.GetHashCode();

    hash1.Should().Be(hash2);
    hash2.Should().Be(hash3);
  }
  //--------------------------------------------------------------------------------

  #endregion HashCode Consistency Tests

  #region HashCode Distribution Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code distribution for integer values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_HashCodeDistribution_Integers()
  {
    const int count = 1000;
    HashSet<int> hashCodes = [];

    for (int i = 0; i < count; i++)
    {
      DynamicKey key = DynamicKey.GetKey(i);
      int hashCode = key.GetHashCode();
      hashCodes.Add(hashCode);
    }

    // Should have good distribution (at least 80% unique hash codes)
    double distributionRatio = (double)hashCodes.Count / count;
    distributionRatio.Should().BeGreaterThan(0.8);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code distribution for string values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_HashCodeDistribution_Strings()
  {
    const int count = 1000;
    HashSet<int> hashCodes = [];

    for (int i = 0; i < count; i++)
    {
      DynamicKey key = DynamicKey.GetKey($"string_{i}");
      int hashCode = key.GetHashCode();
      hashCodes.Add(hashCode);
    }

    // Should have good distribution (at least 80% unique hash codes)
    double distributionRatio = (double)hashCodes.Count / count;
    distributionRatio.Should().BeGreaterThan(0.8);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code distribution for Guid values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_HashCodeDistribution_Guids()
  {
    const int count = 1000;
    HashSet<int> hashCodes = [];

    for (int i = 0; i < count; i++)
    {
      DynamicKey key = DynamicKey.GetKey(Guid.NewGuid());
      int hashCode = key.GetHashCode();
      hashCodes.Add(hashCode);
    }

    // Should have good distribution (at least 80% unique hash codes)
    double distributionRatio = (double)hashCodes.Count / count;
    distributionRatio.Should().BeGreaterThan(0.8);
  }
  //--------------------------------------------------------------------------------

  #endregion HashCode Distribution Tests

  #region Equality Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same values are equal.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_SameValues_AreEqual()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(42);

    key1.Should().Be(key2);
    key1.Equals(key2).Should().BeTrue();
    (key1 == key2).Should().BeTrue();
    (key1 != key2).Should().BeFalse();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different values are not equal.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DifferentValues_AreNotEqual()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    key1.Should().NotBe(key2);
    key1.Equals(key2).Should().BeFalse();
    (key1 == key2).Should().BeFalse();
    (key1 != key2).Should().BeTrue();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests equality with null.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_EqualityWithNull_WorksCorrectly()
  {
    DynamicKey key = DynamicKey.GetKey(42);

    key.Equals(null).Should().BeFalse();
    (key == null).Should().BeFalse();
    (key != null).Should().BeTrue();
    (null == key).Should().BeFalse();
    (null != key).Should().BeTrue();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests equality with same instance.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_EqualityWithSameInstance_WorksCorrectly()
  {
    DynamicKey key = DynamicKey.GetKey(42);

    key.Should().Be(key);
    key.Equals(key).Should().BeTrue();
    (key == key).Should().BeTrue();
    (key != key).Should().BeFalse();
  }
  //--------------------------------------------------------------------------------

  #endregion Equality Tests

  #region Cross-Type Equality Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different key types with same values are not equal.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CrossTypeEquality_DifferentTypesNotEqual()
  {
    DynamicKey intKey = DynamicKey.GetKey(42);
    DynamicKey stringKey = DynamicKey.GetKey("42");
    DynamicKey longKey = DynamicKey.GetKey(42L);

    intKey.Should().NotBe(stringKey);
    intKey.Should().NotBe(longKey);
    stringKey.Should().NotBe(longKey);

    intKey.Equals(stringKey).Should().BeFalse();
    intKey.Equals(longKey).Should().BeFalse();
    stringKey.Equals(longKey).Should().BeFalse();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different key types have different hash codes.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CrossTypeHashCodes_DifferentTypesDifferentHashCodes()
  {
    DynamicKey intKey = DynamicKey.GetKey(-42);
    DynamicKey stringKey = DynamicKey.GetKey("-42");
    DynamicKey longKey = DynamicKey.GetKey(-42L);

    int intHash = intKey.GetHashCode();
    int stringHash = stringKey.GetHashCode();
    int longHash = longKey.GetHashCode();

    intHash.Should().NotBe(stringHash);
    intHash.Should().NotBe(longHash);
    stringHash.Should().NotBe(longHash);
  }
  //--------------------------------------------------------------------------------

  #endregion Cross-Type Equality Tests

  #region Composite Key HashCode and Equality Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code consistency for composite keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyHashCode_Consistent()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey("test");
    DynamicKey composite1 = DynamicKey.Combine(key1, key2);
    DynamicKey composite2 = DynamicKey.Combine(key1, key2);

    int hash1 = composite1.GetHashCode();
    int hash2 = composite2.GetHashCode();

    hash1.Should().Be(hash2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests equality for composite keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyEquality_WorksCorrectly()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey("test");
    DynamicKey composite1 = DynamicKey.Combine(key1, key2);
    DynamicKey composite2 = DynamicKey.Combine(key1, key2);

    composite1.Should().Be(composite2);
    composite1.Equals(composite2).Should().BeTrue();
    (composite1 == composite2).Should().BeTrue();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different composite keys are not equal.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyEquality_DifferentCompositesNotEqual()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey("test");
    DynamicKey key3 = DynamicKey.GetKey(43);
    DynamicKey composite1 = DynamicKey.Combine(key1, key2);
    DynamicKey composite2 = DynamicKey.Combine(key3, key2);

    composite1.Should().NotBe(composite2);
    composite1.Equals(composite2).Should().BeFalse();
    (composite1 == composite2).Should().BeFalse();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code distribution for composite keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyHashCode_Distribution()
  {
    const int count = 100;
    HashSet<int> hashCodes = [];

    for (int i = 0; i < count; i++)
    {
      DynamicKey key1 = DynamicKey.GetKey(i);
      DynamicKey key2 = DynamicKey.GetKey($"string_{i}");
      DynamicKey composite = DynamicKey.Combine(key1, key2);

      int hashCode = composite.GetHashCode();
      hashCodes.Add(hashCode);
    }

    // Should have good distribution
    double distributionRatio = (double)hashCodes.Count / count;
    distributionRatio.Should().BeGreaterThan(0.8);
  }
  //--------------------------------------------------------------------------------

  #endregion Composite Key HashCode and Equality Tests

  #region Special Value HashCode and Equality Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code and equality for null values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_NullValueHashCodeAndEquality_WorksCorrectly()
  {
    DynamicKey nullKey1 = DynamicKey.GetKey((string?)null);
    DynamicKey nullKey2 = DynamicKey.GetKey((string?)null);

    nullKey1.Should().Be(nullKey2);
    nullKey1.GetHashCode().Should().Be(nullKey2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code and equality for empty string values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_EmptyStringHashCodeAndEquality_WorksCorrectly()
  {
    DynamicKey emptyKey1 = DynamicKey.GetKey(string.Empty);
    DynamicKey emptyKey2 = DynamicKey.GetKey(string.Empty);

    emptyKey1.Should().Be(emptyKey2);
    emptyKey1.GetHashCode().Should().Be(emptyKey2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code and equality for default values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DefaultValueHashCodeAndEquality_WorksCorrectly()
  {
    DynamicKey defaultKey1 = DynamicKey.GetKey(Guid.Empty);
    DynamicKey defaultKey2 = DynamicKey.GetKey(Guid.Empty);

    defaultKey1.Should().Be(defaultKey2);
    defaultKey1.GetHashCode().Should().Be(defaultKey2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Special Value HashCode and Equality Tests

  #region Dictionary Key Behavior Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that DynamicKey works correctly as Dictionary key.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_AsDictionaryKey_WorksCorrectly()
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
  /// Tests that different DynamicKeys work as separate Dictionary keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DifferentKeysAsDictionaryKeys_WorksCorrectly()
  {
    Dictionary<DynamicKey, string> dictionary = new();

    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    dictionary[key1] = "value1";
    dictionary[key2] = "value2";

    dictionary.Should().HaveCount(2);
    dictionary[key1].Should().Be("value1");
    dictionary[key2].Should().Be("value2");
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that composite DynamicKeys work as Dictionary keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeysAsDictionaryKeys_WorksCorrectly()
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

  #endregion Dictionary Key Behavior Tests

  #region HashSet Behavior Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that DynamicKey works correctly in HashSet.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_InHashSet_WorksCorrectly()
  {
    HashSet<DynamicKey> hashSet = [];

    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(42); // Same value, should be same key

    hashSet.Add(key1).Should().BeTrue();
    hashSet.Add(key2).Should().BeFalse(); // Already exists

    hashSet.Should().HaveCount(1);
    hashSet.Should().Contain(key1);
    hashSet.Should().Contain(key2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different DynamicKeys work as separate HashSet elements.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DifferentKeysInHashSet_WorksCorrectly()
  {
    HashSet<DynamicKey> hashSet = [];

    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    hashSet.Add(key1).Should().BeTrue();
    hashSet.Add(key2).Should().BeTrue();

    hashSet.Should().HaveCount(2);
    hashSet.Should().Contain(key1);
    hashSet.Should().Contain(key2);
  }
  //--------------------------------------------------------------------------------

  #endregion HashSet Behavior Tests

  #region HashCode Collision Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that hash code collisions are handled correctly.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_HashCodeCollisions_HandledCorrectly()
  {
    // Create keys that might have hash code collisions
    List<DynamicKey> keys = [];
    HashSet<int> hashCodes = [];

    // Test with various values that might collide
    for (int i = 0; i < 1000; i++)
    {
      DynamicKey key = DynamicKey.GetKey(i);
      keys.Add(key);
      hashCodes.Add(key.GetHashCode());
    }

    // Even if there are hash code collisions, equality should still work correctly
    for (int i = 0; i < keys.Count; i++)
    {
      for (int j = i + 1; j < keys.Count; j++)
      {
        if (keys[i].GetHashCode() == keys[j].GetHashCode())
        {
          // If hash codes are equal, the keys should also be equal
          keys[i].Should().Be(keys[j]);
        }
        else
        {
          // If hash codes are different, the keys should be different
          keys[i].Should().NotBe(keys[j]);
        }
      }
    }
  }
  //--------------------------------------------------------------------------------

  #endregion HashCode Collision Tests

  #region Thread Safety HashCode and Equality Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests hash code and equality consistency across threads.
  /// </summary>
  [Fact]
  public async Task UsingDynamicKey_ThreadSafetyHashCodeAndEquality_WorksCorrectly()
  {
    const int taskCount = 10;
    const int iterationsPerTask = 100;

    ConcurrentDictionary<int, List<(DynamicKey key, int hashCode)>> threadResults = new();
    List<Task> tasks = [];

    for (int i = 0; i < taskCount; i++)
    {
      tasks.Add(Task.Run(async () =>
      {
        List<(DynamicKey key, int hashCode)> results = [];

        for (int j = 0; j < iterationsPerTask; j++)
        {
          DynamicKey key = DynamicKey.GetKey(42);
          int hashCode = key.GetHashCode();
          results.Add((key, hashCode));

          await Task.Yield();
        }

        threadResults[Environment.CurrentManagedThreadId] = results;
      }));
    }

    await Task.WhenAll(tasks);

    // Verify hash code consistency within each thread
    foreach (KeyValuePair<int, List<(DynamicKey key, int hashCode)>> threadResult in threadResults)
    {
      List<(DynamicKey key, int hashCode)> results = threadResult.Value;

      // All keys in the same thread should have the same hash code
      int firstHashCode = results[0].hashCode;
      foreach ((DynamicKey key, int hashCode) in results)
      {
        hashCode.Should().Be(firstHashCode);
        key.GetHashCode().Should().Be(firstHashCode);
      }
    }

    // Verify equality consistency across threads
    List<DynamicKey> firstKeysFromEachThread = threadResults.Values
        .Select(results => results[0].key)
        .ToList();

    for (int i = 0; i < firstKeysFromEachThread.Count; i++)
    {
      for (int j = i + 1; j < firstKeysFromEachThread.Count; j++)
      {
        DynamicKey key1 = firstKeysFromEachThread[i];
        DynamicKey key2 = firstKeysFromEachThread[j];

        // They should be equal in value
        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
      }
    }
  }
  //--------------------------------------------------------------------------------

  #endregion Thread Safety HashCode and Equality Tests
}
//################################################################################

