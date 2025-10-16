// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicKey multi-key generic functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_MultiKeyTests : UnitTestBase
{
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Initializes a new instance of the <see cref="UsingDynamicKey_MultiKeyTests"/> class.
  /// </summary>
  /// <param name="xUnitTestOutputHelper">
  /// The Xunit test output helper that can be used to output test messages
  /// </param>
  public UsingDynamicKey_MultiKeyTests (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
  {
  }
  //--------------------------------------------------------------------------------

  #region Two-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from two different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TwoKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test");

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same two-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TwoKeys_SameValuesEqual ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, "test");
    DynamicKey key2 = DynamicKey.GetKey(42, "test");

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different two-key combinations produce different keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TwoKeys_DifferentValuesNotEqual ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, "test");
    DynamicKey key2 = DynamicKey.GetKey(43, "test");
    DynamicKey key3 = DynamicKey.GetKey(42, "test2");

    key1.Should().NotBe(key2);
    key1.Should().NotBe(key3);
    key2.Should().NotBe(key3);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests two-key combination with mixed types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TwoKeys_MixedTypes ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, true);
    DynamicKey key2 = DynamicKey.GetKey("hello", 3.14);
    DynamicKey key3 = DynamicKey.GetKey(Guid.NewGuid(), DateTime.Now);

    key1.Should().NotBeNull();
    key2.Should().NotBeNull();
    key3.Should().NotBeNull();

    key1.Should().NotBe(key2);
    key1.Should().NotBe(key3);
    key2.Should().NotBe(key3);
  }
  //--------------------------------------------------------------------------------

  #endregion Two-Key Tests

  #region Three-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from three different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_ThreeKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same three-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_ThreeKeys_SameValuesEqual ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests three-key combination with mixed types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_ThreeKeys_MixedTypes ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", false);
    DynamicKey key3 = DynamicKey.GetKey(42, "test2", true);

    key1.Should().NotBe(key2);
    key1.Should().NotBe(key3);
    key2.Should().NotBe(key3);
  }
  //--------------------------------------------------------------------------------

  #endregion Three-Key Tests

  #region Four-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from four different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_FourKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same four-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_FourKeys_SameValuesEqual ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Four-Key Tests

  #region Five-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from five different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_FiveKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid());

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same five-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_FiveKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Five-Key Tests

  #region Six-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from six different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_SixKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same six-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_SixKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Six-Key Tests

  #region Seven-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from seven different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_SevenKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(1));

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same seven-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_SevenKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    TimeSpan timeSpan = TimeSpan.FromHours(1);
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Seven-Key Tests

  #region Eight-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from eight different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_EightKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(1), typeof(string));

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same eight-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_EightKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    TimeSpan timeSpan = TimeSpan.FromHours(1);
    Type type = typeof(string);
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Eight-Key Tests

  #region Nine-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from nine different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_NineKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(1), typeof(string), 100L);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same nine-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_NineKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    TimeSpan timeSpan = TimeSpan.FromHours(1);
    Type type = typeof(string);
    long longValue = 100L;
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Nine-Key Tests

  #region Ten-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from ten different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TenKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(1), typeof(string), 100L, 200U);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same ten-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TenKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    TimeSpan timeSpan = TimeSpan.FromHours(1);
    Type type = typeof(string);
    long longValue = 100L;
    uint uintValue = 200U;
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue, uintValue);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue, uintValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Ten-Key Tests

  #region Eleven-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from eleven different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_ElevenKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(1), typeof(string), 100L, 200U, 300UL);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same eleven-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_ElevenKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    TimeSpan timeSpan = TimeSpan.FromHours(1);
    Type type = typeof(string);
    long longValue = 100L;
    uint uintValue = 200U;
    ulong ulongValue = 300UL;
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue, uintValue, ulongValue);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue, uintValue, ulongValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Eleven-Key Tests

  #region Twelve-Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a key from twelve different types.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TwelveKeys_WorksCorrectly ()
  {
    DynamicKey key = DynamicKey.GetKey(42, "test", true, 3.14, Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(1), typeof(string), 100L, 200U, 300UL, 400.5m);

    key.Should().NotBeNull();
    key.Should().BeOfType<DynamicCompositeKey>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same twelve-key combinations produce equal keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_TwelveKeys_SameValuesEqual ()
  {
    Guid guid = Guid.NewGuid();
    DateTime dateTime = DateTime.Now;
    TimeSpan timeSpan = TimeSpan.FromHours(1);
    Type type = typeof(string);
    long longValue = 100L;
    uint uintValue = 200U;
    ulong ulongValue = 300UL;
    decimal decimalValue = 400.5m;
    DynamicKey key1 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue, uintValue, ulongValue, decimalValue);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true, 3.14, guid, dateTime, timeSpan, type, longValue, uintValue, ulongValue, decimalValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Twelve-Key Tests

  #region Null Value Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests multi-key creation with null values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_WithNullValues ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, (string?)null);
    DynamicKey key2 = DynamicKey.GetKey(42, (string?)null);

    key1.Should().NotBeNull();
    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests multi-key creation with mixed null and non-null values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_MixedNullAndNonNullValues ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, (string?)null, true);
    DynamicKey key2 = DynamicKey.GetKey(42, (string?)null, true);
    DynamicKey key3 = DynamicKey.GetKey(42, (string?)null, false);

    key1.Should().NotBeNull();
    key1.Should().Be(key2);
    key1.Should().NotBe(key3);
  }
  //--------------------------------------------------------------------------------

  #endregion Null Value Tests

  #region Enum Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests multi-key creation with enum values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_WithEnums ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, StringComparison.OrdinalIgnoreCase);
    DynamicKey key2 = DynamicKey.GetKey(42, StringComparison.OrdinalIgnoreCase);
    DynamicKey key3 = DynamicKey.GetKey(42, StringComparison.Ordinal);

    key1.Should().NotBeNull();
    key1.Should().Be(key2);
    key1.Should().NotBe(key3);
  }
  //--------------------------------------------------------------------------------

  #endregion Enum Tests

  #region Dictionary Key Behavior Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that multi-keys work correctly as Dictionary keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_AsDictionaryKeys ()
  {
    Dictionary<DynamicKey, string> dictionary = new();

    DynamicKey key1 = DynamicKey.GetKey(42, "test", true);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true); // Same values

    dictionary[key1] = "value1";
    dictionary[key2] = "value2"; // Should overwrite

    dictionary.Should().HaveCount(1);
    dictionary[key1].Should().Be("value2");
    dictionary[key2].Should().Be("value2");
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different multi-keys work as separate Dictionary keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_DifferentKeysAsDictionaryKeys ()
  {
    Dictionary<DynamicKey, string> dictionary = new();

    DynamicKey key1 = DynamicKey.GetKey(42, "test", true);
    DynamicKey key2 = DynamicKey.GetKey(43, "test", true);
    DynamicKey key3 = DynamicKey.GetKey(42, "test2", true);

    dictionary[key1] = "value1";
    dictionary[key2] = "value2";
    dictionary[key3] = "value3";

    dictionary.Should().HaveCount(3);
    dictionary[key1].Should().Be("value1");
    dictionary[key2].Should().Be("value2");
    dictionary[key3].Should().Be("value3");
  }
  //--------------------------------------------------------------------------------

  #endregion Dictionary Key Behavior Tests

  #region HashSet Behavior Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that multi-keys work correctly in HashSet.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_InHashSet ()
  {
    HashSet<DynamicKey> hashSet = [];

    DynamicKey key1 = DynamicKey.GetKey(42, "test", true);
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true); // Same values

    hashSet.Add(key1).Should().BeTrue();
    hashSet.Add(key2).Should().BeFalse(); // Already exists

    hashSet.Should().HaveCount(1);
    hashSet.Should().Contain(key1);
    hashSet.Should().Contain(key2);
  }
  //--------------------------------------------------------------------------------

  #endregion HashSet Behavior Tests

  #region Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests comparison of multi-keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_Comparison ()
  {
    DynamicKey key1 = DynamicKey.GetKey(10, "a");
    DynamicKey key2 = DynamicKey.GetKey(20, "a");
    DynamicKey key3 = DynamicKey.GetKey(10, "b");

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key1.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key1).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests comparison of multi-keys with different lengths.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_ComparisonDifferentLengths ()
  {
    DynamicKey key1 = DynamicKey.GetKey(42, "test");
    DynamicKey key2 = DynamicKey.GetKey(42, "test", true);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion Comparison Tests

  #region Performance Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests performance of multi-key creation.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_Performance ()
  {
    const int iterations = 10000;
    List<DynamicKey> keys = new(iterations);

    // Warm up
    for (int i = 0; i < 100; i++)
    {
      DynamicKey.GetKey(i, $"test_{i}", i % 2 == 0);
    }

    // Measure
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
    {
      DynamicKey key = DynamicKey.GetKey(i, $"test_{i}", i % 2 == 0);
      keys.Add(key);
    }
    stopwatch.Stop();

    keys.Should().HaveCount(iterations);

    // Should complete in reasonable time (less than 1 second for 10k iterations)
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);

    TestWriteLine($"Created {iterations} multi-keys in {stopwatch.ElapsedMilliseconds}ms");
  }
  //--------------------------------------------------------------------------------

  #endregion Performance Tests

  #region Thread Safety Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests thread safety of multi-key creation.
  /// </summary>
  [Fact]
  public async Task UsingDynamicKey_GetKey_MultiKeys_ThreadSafety ()
  {
    const int taskCount = 10;
    const int iterationsPerTask = 100;

    ConcurrentDictionary<int, List<DynamicKey>> threadResults = new();
    List<Task> tasks = [];

    for (int i = 0; i < taskCount; i++)
    {
      int taskId = i; // Capture for closure
      tasks.Add(Task.Run(async () =>
      {
        for (int j = 0; j < iterationsPerTask; j++)
        {
          DynamicKey key = DynamicKey.GetKey(taskId, $"test_{j}", j % 2 == 0);
          List<DynamicKey> keyList = threadResults.GetOrAdd(Environment.CurrentManagedThreadId, _ => []);
          keyList.Add(key);

          // Make this truly asynchronous to test multi-thread behavior
          await Task.Yield();
        }
      }));
    }

    await Task.WhenAll(tasks);

    foreach (KeyValuePair<int, List<DynamicKey>> threadResult in threadResults)
    {
      List<DynamicKey> keys = threadResult.Value;

      // All keys should be unique within the thread
      HashSet<DynamicKey> uniqueKeys = new(keys);
      uniqueKeys.Should().HaveCount(keys.Count);
    }
  }
  //--------------------------------------------------------------------------------

  #endregion Thread Safety Tests

  #region Edge Case Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests multi-key creation with edge case values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_EdgeCases ()
  {
    // Test with minimum/maximum values
    DynamicKey key1 = DynamicKey.GetKey(int.MinValue, int.MaxValue);
    DynamicKey key2 = DynamicKey.GetKey(int.MinValue, int.MaxValue);

    key1.Should().Be(key2);

    // Test with special floating point values
    DynamicKey key3 = DynamicKey.GetKey(double.NaN, double.PositiveInfinity);
    DynamicKey key4 = DynamicKey.GetKey(double.NaN, double.PositiveInfinity);

    key3.Should().Be(key4);

    // Test with empty strings
    DynamicKey key5 = DynamicKey.GetKey(string.Empty, string.Empty);
    DynamicKey key6 = DynamicKey.GetKey(string.Empty, string.Empty);

    key5.Should().Be(key6);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests multi-key creation with very long parameter lists.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_MaximumParameters ()
  {
    // Test with all 12 parameters
    DynamicKey key1 = DynamicKey.GetKey(
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
    DynamicKey key2 = DynamicKey.GetKey(
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

    key1.Should().NotBeNull();
    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Edge Case Tests

  #region Equivalence with Manual Combination Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that multi-key methods produce equivalent results to manual combination.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GetKey_MultiKeys_EquivalentToManualCombination ()
  {
    // Test 2-key combination
    DynamicKey multiKey2 = DynamicKey.GetKey(42, "test");
    DynamicKey manualKey2 = DynamicKey.Combine(DynamicKey.GetKey(42), DynamicKey.GetKey("test"));

    multiKey2.Should().Be(manualKey2);
    multiKey2.GetHashCode().Should().Be(manualKey2.GetHashCode());

    // Test 3-key combination
    DynamicKey multiKey3 = DynamicKey.GetKey(42, "test", true);
    DynamicKey manualKey3 = DynamicKey.Combine(
        DynamicKey.GetKey(42),
        DynamicKey.GetKey("test"),
        DynamicKey.GetKey(true));

    multiKey3.Should().Be(manualKey3);
    multiKey3.GetHashCode().Should().Be(manualKey3.GetHashCode());

    // Test 4-key combination
    DynamicKey multiKey4 = DynamicKey.GetKey(42, "test", true, 314L);
    DynamicKey manualKey4 = DynamicKey.Combine(
        DynamicKey.GetKey(42),
        DynamicKey.GetKey("test"),
        DynamicKey.GetKey(true),
        DynamicKey.GetKey(314L));

    multiKey4.Should().Be(manualKey4);
    multiKey4.GetHashCode().Should().Be(manualKey4.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Equivalence with Manual Combination Tests
}
//################################################################################
