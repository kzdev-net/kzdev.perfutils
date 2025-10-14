// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;
using DynamicKeyClass = KZDev.PerfUtils.DynamicKey;

namespace KZDev.PerfUtils.Tests.DynamicKey;

//################################################################################
/// <summary>
/// Unit tests for single DynamicKeyClass types (int, long, uint, ulong, bool, string, Guid, Type, enums, objects)
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKeyClass")]
public class UsingDynamicKeyClass_SingleKeys : UnitTestBase
{
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Initializes a new instance of the <see cref="UsingDynamicKeyClass_SingleKeys"/> class.
  /// </summary>
  /// <param name="xUnitTestOutputHelper">
  /// The Xunit test output helper that can be used to output test messages
  /// </param>
  public UsingDynamicKeyClass_SingleKeys(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
  {
  }
  //--------------------------------------------------------------------------------

  #region Integer Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating an integer-based dynamic key.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetIntKey_ReturnsCorrectKey()
  {
    int testValue = 42;
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that the same integer value returns the same key instance (caching).
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetSameIntKey_ReturnsSameInstance()
  {
    int testValue = 100;
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().BeSameAs(key2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests integer key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_IntKeyEquality_WorksCorrectly()
  {
    int testValue = 200;
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests integer key comparison.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_IntKeyComparison_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(10);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(20);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key1.CompareTo(key1).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Integer Key Tests

  #region Long Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a long-based dynamic key.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetLongKey_ReturnsCorrectKey()
  {
    long testValue = 42L;
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests long key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_LongKeyEquality_WorksCorrectly()
  {
    long testValue = 200L;
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests long key comparison.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_LongKeyComparison_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(10L);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(20L);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key1.CompareTo(key1).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Long Key Tests

  #region Unsigned Integer Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating an unsigned integer-based dynamic key.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetUIntKey_ReturnsCorrectKey()
  {
    uint testValue = 42U;
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests unsigned integer key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_UIntKeyEquality_WorksCorrectly()
  {
    uint testValue = 200U;
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests unsigned integer key comparison.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_UIntKeyComparison_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(10U);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(20U);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key1.CompareTo(key1).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Unsigned Integer Key Tests

  #region Unsigned Long Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating an unsigned long-based dynamic key.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetULongKey_ReturnsCorrectKey()
  {
    ulong testValue = 42UL;
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests unsigned long key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ULongKeyEquality_WorksCorrectly()
  {
    ulong testValue = 200UL;
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests unsigned long key comparison.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ULongKeyComparison_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(10UL);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(20UL);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key1.CompareTo(key1).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Unsigned Long Key Tests

  #region Boolean Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating boolean-based dynamic keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetBoolKey_ReturnsCorrectKey()
  {
    DynamicKeyClass trueKey = DynamicKeyClass.GetKey(true);
    DynamicKeyClass falseKey = DynamicKeyClass.GetKey(false);

    trueKey.Should().NotBeNull();
    falseKey.Should().NotBeNull();
    trueKey.Should().NotBe(falseKey);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that boolean keys use cached instances.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_BoolKeyCaching_WorksCorrectly()
  {
    DynamicKeyClass trueKey1 = DynamicKeyClass.GetKey(true);
    DynamicKeyClass trueKey2 = DynamicKeyClass.GetKey(true);
    DynamicKeyClass falseKey1 = DynamicKeyClass.GetKey(false);
    DynamicKeyClass falseKey2 = DynamicKeyClass.GetKey(false);

    trueKey1.Should().BeSameAs(trueKey2);
    falseKey1.Should().BeSameAs(falseKey2);
  }
  //--------------------------------------------------------------------------------

  #endregion Boolean Key Tests

  #region String Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating string-based dynamic keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetStringKey_ReturnsCorrectKey()
  {
    string testValue = "test";
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests string key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_StringKeyEquality_WorksCorrectly()
  {
    string testValue = "test";
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests null string handling.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_NullStringKey_HandledCorrectly()
  {
    DynamicKeyClass key = DynamicKeyClass.GetKey((string?)null);

    key.Should().NotBeNull();
    key.Should().Be(DynamicKeyClass.EmptyString);
  }
  //--------------------------------------------------------------------------------

  #endregion String Key Tests

  #region Guid Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating Guid-based dynamic keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetGuidKey_ReturnsCorrectKey()
  {
    Guid testValue = Guid.NewGuid();
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests Guid key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GuidKeyEquality_WorksCorrectly()
  {
    Guid testValue = Guid.NewGuid();
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Guid Key Tests

  #region Type Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating Type-based dynamic keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetTypeKey_ReturnsCorrectKey()
  {
    Type testValue = typeof(string);
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests Type key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_TypeKeyEquality_WorksCorrectly()
  {
    Type testValue = typeof(int);
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------

  #endregion Type Key Tests

  #region Enum Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating enum-based dynamic keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetEnumKey_ReturnsCorrectKey()
  {
    DayOfWeek testValue = DayOfWeek.Monday;
    DynamicKeyClass key = DynamicKeyClass.GetEnumKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests enum key equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_EnumKeyEquality_WorksCorrectly()
  {
    DayOfWeek testValue = DayOfWeek.Friday;
    DynamicKeyClass key1 = DynamicKeyClass.GetEnumKey(testValue);
    DynamicKeyClass key2 = DynamicKeyClass.GetEnumKey(testValue);

    key1.Should().Be(key2);
    key1.GetHashCode().Should().Be(key2.GetHashCode());
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests enum key comparison.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_EnumKeyComparison_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetEnumKey(DayOfWeek.Monday);
    DynamicKeyClass key2 = DynamicKeyClass.GetEnumKey(DayOfWeek.Friday);

    key1.CompareTo(key2).Should().BeNegative();
    key2.CompareTo(key1).Should().BePositive();
    key1.CompareTo(key1).Should().Be(0);
  }
  //--------------------------------------------------------------------------------

  #endregion Enum Key Tests

  #region Object Key Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating object-based dynamic keys.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_GetObjectKey_ReturnsCorrectKey()
  {
    object testValue = new { Name = "test", Value = 42 };
    DynamicKeyClass key = DynamicKeyClass.GetKey(testValue);

    key.Should().NotBeNull();
    key.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests null object handling.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_NullObjectKey_HandledCorrectly()
  {
    DynamicKeyClass key = DynamicKeyClass.GetKey((object?)null);

    key.Should().NotBeNull();
    key.Should().Be(DynamicKeyClass.Null);
  }
  //--------------------------------------------------------------------------------

  #endregion Object Key Tests

  #region Implicit Conversion Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests implicit conversion from int to DynamicKeyClass.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ImplicitIntConversion_WorksCorrectly()
  {
    int testValue = 42;
    DynamicKeyClass key = testValue;

    key.Should().NotBeNull();
    key.Should().Be(DynamicKeyClass.GetKey(testValue));
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests implicit conversion from long to DynamicKeyClass.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ImplicitLongConversion_WorksCorrectly()
  {
    long testValue = 42L;
    DynamicKeyClass key = testValue;

    key.Should().NotBeNull();
    key.Should().Be(DynamicKeyClass.GetKey(testValue));
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests implicit conversion from string to DynamicKeyClass.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ImplicitStringConversion_WorksCorrectly()
  {
    string testValue = "test";
    DynamicKeyClass key = testValue;

    key.Should().NotBeNull();
    key.Should().Be(DynamicKeyClass.GetKey(testValue));
  }
  //--------------------------------------------------------------------------------

  #endregion Implicit Conversion Tests
}
//################################################################################
