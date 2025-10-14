// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for single DynamicKey types (int, long, uint, ulong, bool, string, Guid, Type, enums, objects)
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKeyClass_SingleKeys : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKeyClass_SingleKeys"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKeyClass_SingleKeys (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Integer Key Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating an integer-based dynamic key.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_GetIntKey_ReturnsCorrectKey ()
    {
        int testValue = 42;
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that the same integer value returns the same key instance (caching).
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_GetSameIntKey_ReturnsSameInstance ()
    {
        int testValue = 100;
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

        key1.Should().BeSameAs(key2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests integer key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_IntKeyEquality_WorksCorrectly ()
    {
        int testValue = 200;
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests integer key comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_IntKeyComparison_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(10);
        DynamicKey key2 = DynamicKey.GetKey(20);

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
    public void UsingDynamicKeyClass_GetLongKey_ReturnsCorrectKey ()
    {
        long testValue = 42L;
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests long key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_LongKeyEquality_WorksCorrectly ()
    {
        long testValue = 200L;
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests long key comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_LongKeyComparison_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(10L);
        DynamicKey key2 = DynamicKey.GetKey(20L);

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
    public void UsingDynamicKeyClass_GetUIntKey_ReturnsCorrectKey ()
    {
        uint testValue = 42U;
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests unsigned integer key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_UIntKeyEquality_WorksCorrectly ()
    {
        uint testValue = 200U;
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests unsigned integer key comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_UIntKeyComparison_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(10U);
        DynamicKey key2 = DynamicKey.GetKey(20U);

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
    public void UsingDynamicKeyClass_GetULongKey_ReturnsCorrectKey ()
    {
        ulong testValue = 42UL;
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests unsigned long key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ULongKeyEquality_WorksCorrectly ()
    {
        ulong testValue = 200UL;
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests unsigned long key comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ULongKeyComparison_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(10UL);
        DynamicKey key2 = DynamicKey.GetKey(20UL);

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
    public void UsingDynamicKeyClass_GetBoolKey_ReturnsCorrectKey ()
    {
        DynamicKey trueKey = DynamicKey.GetKey(true);
        DynamicKey falseKey = DynamicKey.GetKey(false);

        trueKey.Should().NotBeNull();
        falseKey.Should().NotBeNull();
        trueKey.Should().NotBe(falseKey);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that boolean keys use cached instances.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_BoolKeyCaching_WorksCorrectly ()
    {
        DynamicKey trueKey1 = DynamicKey.GetKey(true);
        DynamicKey trueKey2 = DynamicKey.GetKey(true);
        DynamicKey falseKey1 = DynamicKey.GetKey(false);
        DynamicKey falseKey2 = DynamicKey.GetKey(false);

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
    public void UsingDynamicKeyClass_GetStringKey_ReturnsCorrectKey ()
    {
        string testValue = "test";
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests string key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_StringKeyEquality_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests null string handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_NullStringKey_HandledCorrectly ()
    {
        DynamicKey key = DynamicKey.GetKey((string?)null);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.EmptyString);
    }
    //--------------------------------------------------------------------------------

    #endregion String Key Tests

    #region Guid Key Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating Guid-based dynamic keys.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_GetGuidKey_ReturnsCorrectKey ()
    {
        Guid testValue = Guid.NewGuid();
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests Guid key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_GuidKeyEquality_WorksCorrectly ()
    {
        Guid testValue = Guid.NewGuid();
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

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
    public void UsingDynamicKeyClass_GetTypeKey_ReturnsCorrectKey ()
    {
        Type testValue = typeof(string);
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests Type key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_TypeKeyEquality_WorksCorrectly ()
    {
        Type testValue = typeof(int);
        DynamicKey key1 = DynamicKey.GetKey(testValue);
        DynamicKey key2 = DynamicKey.GetKey(testValue);

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
    public void UsingDynamicKeyClass_GetEnumKey_ReturnsCorrectKey ()
    {
        DayOfWeek testValue = DayOfWeek.Monday;
        DynamicKey key = DynamicKey.GetEnumKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests enum key equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_EnumKeyEquality_WorksCorrectly ()
    {
        DayOfWeek testValue = DayOfWeek.Friday;
        DynamicKey key1 = DynamicKey.GetEnumKey(testValue);
        DynamicKey key2 = DynamicKey.GetEnumKey(testValue);

        key1.Should().Be(key2);
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests enum key comparison.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_EnumKeyComparison_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetEnumKey(DayOfWeek.Monday);
        DynamicKey key2 = DynamicKey.GetEnumKey(DayOfWeek.Friday);

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
    public void UsingDynamicKeyClass_GetObjectKey_ReturnsCorrectKey ()
    {
        object testValue = new { Name = "test", Value = 42 };
        DynamicKey key = DynamicKey.GetKey(testValue);

        key.Should().NotBeNull();
        key.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests null object handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_NullObjectKey_HandledCorrectly ()
    {
        DynamicKey key = DynamicKey.GetKey((object?)null);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------

    #endregion Object Key Tests

    #region Implicit Conversion Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests implicit conversion from int to DynamicKey.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ImplicitIntConversion_WorksCorrectly ()
    {
        int testValue = 42;
        DynamicKey key = testValue;

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.GetKey(testValue));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests implicit conversion from long to DynamicKey.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ImplicitLongConversion_WorksCorrectly ()
    {
        long testValue = 42L;
        DynamicKey key = testValue;

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.GetKey(testValue));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests implicit conversion from string to DynamicKey.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ImplicitStringConversion_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key = testValue;

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.GetKey(testValue));
    }
    //--------------------------------------------------------------------------------

    #endregion Implicit Conversion Tests
}
//################################################################################
