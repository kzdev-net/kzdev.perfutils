// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicKey IComparable functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_ComparisonTests : UnitTestBase
{
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Initializes a new instance of the <see cref="UsingDynamicKey_ComparisonTests"/> class.
  /// </summary>
  /// <param name="xUnitTestOutputHelper">
  /// The Xunit test output helper that can be used to output test messages
  /// </param>
  public UsingDynamicKey_ComparisonTests(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
  {
  }
  //--------------------------------------------------------------------------------

  #region Basic Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that same values compare as equal.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_SameValues_CompareAsEqual()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(42);

    key1.CompareTo(key2).Should().Be(0);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that different values compare correctly.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DifferentValues_CompareCorrectly()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
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
  /// <summary>
  /// Tests comparison with same instance.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_ComparisonWithSameInstance_WorksCorrectly()
  {
    DynamicKey key = DynamicKey.GetKey(42);

    key.CompareTo(key).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Basic Comparison Tests

  #region Integer Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests integer comparison ordering.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_IntegerComparison_Ordering()
  {
    DynamicKey key1 = DynamicKey.GetKey(10);
    DynamicKey key2 = DynamicKey.GetKey(20);
    DynamicKey key3 = DynamicKey.GetKey(30);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key2.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key2).Should().BePositive();
    key1.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key1).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests negative integer comparison.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_NegativeIntegerComparison_WorksCorrectly()
  {
    DynamicKey key1 = DynamicKey.GetKey(-10);
    DynamicKey key2 = DynamicKey.GetKey(10);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion Integer Comparison Tests

  #region Long Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests long comparison ordering.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_LongComparison_Ordering()
  {
    DynamicKey key1 = DynamicKey.GetKey(10L);
    DynamicKey key2 = DynamicKey.GetKey(20L);
    DynamicKey key3 = DynamicKey.GetKey(30L);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key2.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key2).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion Long Comparison Tests

  #region String Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests string comparison ordering.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_StringComparison_Ordering()
  {
    DynamicKey key1 = DynamicKey.GetKey("apple");
    DynamicKey key2 = DynamicKey.GetKey("banana");
    DynamicKey key3 = DynamicKey.GetKey("cherry");

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key2.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key2).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests string comparison with case sensitivity.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_StringComparison_CaseSensitive()
  {
    DynamicKey key1 = DynamicKey.GetKey("Apple");
    DynamicKey key2 = DynamicKey.GetKey("apple");

    key1.CompareTo(key2).Should().NotBe(0);
    key2.CompareTo(key1).Should().NotBe(0);
  }
  //--------------------------------------------------------------------------------

  #endregion String Comparison Tests

  #region Guid Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests Guid comparison ordering.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_GuidComparison_Ordering()
  {
    Guid guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
    Guid guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
    Guid guid3 = Guid.Parse("00000000-0000-0000-0000-000000000003");

    DynamicKey key1 = DynamicKey.GetKey(guid1);
    DynamicKey key2 = DynamicKey.GetKey(guid2);
    DynamicKey key3 = DynamicKey.GetKey(guid3);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key2.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key2).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion Guid Comparison Tests

  #region Boolean Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests boolean comparison ordering.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_BooleanComparison_Ordering()
  {
    DynamicKey falseKey = DynamicKey.GetKey(false);
    DynamicKey trueKey = DynamicKey.GetKey(true);

    falseKey.CompareTo(trueKey).Should().BeNegative();
    trueKey.CompareTo(falseKey).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion Boolean Comparison Tests

  #region DateTime Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests DateTime comparison ordering.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_DateTimeComparison_Ordering()
  {
    DateTime date1 = new(2023, 1, 1);
    DateTime date2 = new(2023, 1, 2);
    DateTime date3 = new(2023, 1, 3);

    DynamicKey key1 = DynamicKey.GetValueKey(date1);
    DynamicKey key2 = DynamicKey.GetValueKey(date2);
    DynamicKey key3 = DynamicKey.GetValueKey(date3);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key2.CompareTo(key3).Should().BeNegative();
    key3.CompareTo(key2).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion DateTime Comparison Tests

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
    DynamicKey longKey = DynamicKey.GetKey(42L);

    // Different types should not compare as equal
    intKey.CompareTo(stringKey).Should().NotBe(0);
    intKey.CompareTo(longKey).Should().NotBe(0);
    stringKey.CompareTo(longKey).Should().NotBe(0);

    // Verify symmetry
    stringKey.CompareTo(intKey).Should().Be(-intKey.CompareTo(stringKey));
    longKey.CompareTo(intKey).Should().Be(-intKey.CompareTo(longKey));
    longKey.CompareTo(stringKey).Should().Be(-stringKey.CompareTo(longKey));
  }
  //--------------------------------------------------------------------------------

  #endregion Cross-Type Comparison Tests

  #region Composite Key Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests composite key comparison by length.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyComparison_ByLength()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey("test");
    DynamicKey key3 = DynamicKey.GetKey(true);

    DynamicKey twoElement = DynamicKey.Combine(key1, key2);
    DynamicKey threeElement = DynamicKey.Combine(key1, key2, key3);

    twoElement.CompareTo(threeElement).Should().BeNegative();
    threeElement.CompareTo(twoElement).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests composite key comparison by element values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyComparison_ByElements()
  {
    DynamicKey key1 = DynamicKey.GetKey(10);
    DynamicKey key2 = DynamicKey.GetKey("test");
    DynamicKey key3 = DynamicKey.GetKey(20);
    DynamicKey key4 = DynamicKey.GetKey("test");

    DynamicKey composite1 = DynamicKey.Combine(key1, key2);
    DynamicKey composite2 = DynamicKey.Combine(key3, key4);

    composite1.CompareTo(composite2).Should().BeNegative();
    composite2.CompareTo(composite1).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests composite key comparison with same elements.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeKeyComparison_SameElements()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey("test");

    DynamicKey composite1 = DynamicKey.Combine(key1, key2);
    DynamicKey composite2 = DynamicKey.Combine(key1, key2);

    composite1.CompareTo(composite2).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Composite Key Comparison Tests

  #region Sorting Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that DynamicKeys can be sorted correctly.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_Sorting_WorksCorrectly()
  {
    List<DynamicKey> keys = [
        DynamicKey.GetKey(30),
            DynamicKey.GetKey(10),
            DynamicKey.GetKey(20),
            DynamicKey.GetKey(50),
            DynamicKey.GetKey(40)
    ];

    keys.Sort();

    keys[0].Should().Be(DynamicKey.GetKey(10));
    keys[1].Should().Be(DynamicKey.GetKey(20));
    keys[2].Should().Be(DynamicKey.GetKey(30));
    keys[3].Should().Be(DynamicKey.GetKey(40));
    keys[4].Should().Be(DynamicKey.GetKey(50));
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that string DynamicKeys can be sorted correctly.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_StringSorting_WorksCorrectly()
  {
    List<DynamicKey> keys = [
        DynamicKey.GetKey("cherry"),
            DynamicKey.GetKey("apple"),
            DynamicKey.GetKey("banana"),
            DynamicKey.GetKey("date"),
            DynamicKey.GetKey("elderberry")
    ];

    keys.Sort();

    keys[0].Should().Be(DynamicKey.GetKey("apple"));
    keys[1].Should().Be(DynamicKey.GetKey("banana"));
    keys[2].Should().Be(DynamicKey.GetKey("cherry"));
    keys[3].Should().Be(DynamicKey.GetKey("date"));
    keys[4].Should().Be(DynamicKey.GetKey("elderberry"));
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that composite DynamicKeys can be sorted correctly.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_CompositeSorting_WorksCorrectly()
  {
    List<DynamicKey> keys = [
        DynamicKey.Combine(DynamicKey.GetKey(30), DynamicKey.GetKey("c")),
            DynamicKey.Combine(DynamicKey.GetKey(10), DynamicKey.GetKey("a")),
            DynamicKey.Combine(DynamicKey.GetKey(20), DynamicKey.GetKey("b")),
            DynamicKey.Combine(DynamicKey.GetKey(10), DynamicKey.GetKey("b")),
            DynamicKey.Combine(DynamicKey.GetKey(20), DynamicKey.GetKey("a"))
    ];

    keys.Sort();

    keys[0].Should().Be(DynamicKey.Combine(DynamicKey.GetKey(10), DynamicKey.GetKey("a")));
    keys[1].Should().Be(DynamicKey.Combine(DynamicKey.GetKey(10), DynamicKey.GetKey("b")));
    keys[2].Should().Be(DynamicKey.Combine(DynamicKey.GetKey(20), DynamicKey.GetKey("a")));
    keys[3].Should().Be(DynamicKey.Combine(DynamicKey.GetKey(20), DynamicKey.GetKey("b")));
    keys[4].Should().Be(DynamicKey.Combine(DynamicKey.GetKey(30), DynamicKey.GetKey("c")));
  }
  //--------------------------------------------------------------------------------

  #endregion Sorting Tests

  #region Comparison Consistency Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that comparison is consistent across multiple calls.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_ComparisonConsistency_MultipleCalls()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    int comparison1 = key1.CompareTo(key2);
    int comparison2 = key1.CompareTo(key2);
    int comparison3 = key1.CompareTo(key2);

    comparison1.Should().Be(comparison2);
    comparison2.Should().Be(comparison3);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that comparison is symmetric.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_ComparisonSymmetry_WorksCorrectly()
  {
    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    int comparison1 = key1.CompareTo(key2);
    int comparison2 = key2.CompareTo(key1);

    comparison1.Should().Be(-comparison2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that comparison is transitive.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_ComparisonTransitivity_WorksCorrectly()
  {
    DynamicKey key1 = DynamicKey.GetKey(10);
    DynamicKey key2 = DynamicKey.GetKey(20);
    DynamicKey key3 = DynamicKey.GetKey(30);

    int comparison12 = key1.CompareTo(key2);
    int comparison23 = key2.CompareTo(key3);
    int comparison13 = key1.CompareTo(key3);

    // If key1 < key2 and key2 < key3, then key1 < key3
    if (comparison12 < 0 && comparison23 < 0)
    {
      comparison13.Should().BeNegative();
    }
  }
  //--------------------------------------------------------------------------------

  #endregion Comparison Consistency Tests

  #region Edge Case Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests comparison with minimum and maximum values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_ComparisonMinMaxValues_WorksCorrectly()
  {
    DynamicKey minKey = DynamicKey.GetKey(int.MinValue);
    DynamicKey maxKey = DynamicKey.GetKey(int.MaxValue);

    minKey.CompareTo(maxKey).Should().BeNegative();
    maxKey.CompareTo(minKey).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests comparison with zero values.
  /// </summary>
  [Fact]
  public void UsingDynamicKey_ComparisonZeroValues_WorksCorrectly()
  {
    DynamicKey zeroKey = DynamicKey.GetKey(0);
    DynamicKey positiveKey = DynamicKey.GetKey(1);
    DynamicKey negativeKey = DynamicKey.GetKey(-1);

    zeroKey.CompareTo(positiveKey).Should().BeNegative();
    zeroKey.CompareTo(negativeKey).Should().BePositive();
    negativeKey.CompareTo(zeroKey).Should().BeNegative();
    positiveKey.CompareTo(zeroKey).Should().BePositive();
  }
  //--------------------------------------------------------------------------------

  #endregion Edge Case Comparison Tests

  #region Thread Safety Comparison Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests comparison consistency across threads.
  /// </summary>
  [Fact]
  public async Task UsingDynamicKey_ThreadSafetyComparison_WorksCorrectly()
  {
    const int taskCount = 10;
    const int iterationsPerTask = 100;

    ConcurrentDictionary<int, List<int>> threadResults = new();
    List<Task> tasks = [];

    DynamicKey key1 = DynamicKey.GetKey(42);
    DynamicKey key2 = DynamicKey.GetKey(43);

    for (int i = 0; i < taskCount; i++)
    {
      tasks.Add(Task.Run(async () =>
      {
        List<int> comparisons = [];

        for (int j = 0; j < iterationsPerTask; j++)
        {
          int comparison = key1.CompareTo(key2);
          comparisons.Add(comparison);

          await Task.Yield();
        }

        threadResults[Environment.CurrentManagedThreadId] = comparisons;
      }));
    }

    await Task.WhenAll(tasks);

    // Verify comparison consistency across threads
    int expectedComparison = key1.CompareTo(key2);

    foreach (KeyValuePair<int, List<int>> threadResult in threadResults)
    {
      List<int> comparisons = threadResult.Value;

      foreach (int comparison in comparisons)
      {
        comparison.Should().Be(expectedComparison);
      }
    }
  }
  //--------------------------------------------------------------------------------

  #endregion Thread Safety Comparison Tests
}
//################################################################################

