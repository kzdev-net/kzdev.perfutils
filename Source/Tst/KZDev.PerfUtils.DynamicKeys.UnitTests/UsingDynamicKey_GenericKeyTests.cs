// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicKey&lt;T&gt; generic key functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_GenericKeyTests : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKey_GenericKeyTests"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKey_GenericKeyTests (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Primitive Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;int&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_IntGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        int testValue = 42;
        DynamicKey key1 = DynamicKey<int>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<int>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicIntKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;long&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_LongGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        long testValue = 42L;
        DynamicKey key1 = DynamicKey<long>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<long>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicLongKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;uint&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_UIntGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        uint testValue = 42U;
        DynamicKey key1 = DynamicKey<uint>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<uint>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicUIntKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;ulong&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ULongGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        ulong testValue = 42UL;
        DynamicKey key1 = DynamicKey<ulong>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<ulong>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicULongKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;bool&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_BoolGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        DynamicKey trueKey1 = DynamicKey<bool>.GetKey(true);
        DynamicKey trueKey2 = DynamicKey<bool>.GetKey(true);
        DynamicKey falseKey1 = DynamicKey<bool>.GetKey(false);
        DynamicKey falseKey2 = DynamicKey<bool>.GetKey(false);

        trueKey1.Should().NotBeNull();
        trueKey1.Should().BeSameAs(trueKey2);
        trueKey1.Should().BeOfType<DynamicBoolKey>();

        falseKey1.Should().NotBeNull();
        falseKey1.Should().BeSameAs(falseKey2);
        falseKey1.Should().BeOfType<DynamicBoolKey>();

        trueKey1.Should().NotBe(falseKey1);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;string&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_StringGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        string testValue = "test";
        DynamicKey key1 = DynamicKey<string>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<string>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicStringKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;Guid&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_GuidGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        Guid testValue = Guid.NewGuid();
        DynamicKey key1 = DynamicKey<Guid>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<Guid>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicGuidKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;Type&gt; creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_TypeGenericKey_CreationAndCaching_WorksCorrectly ()
    {
        Type testValue = typeof(string);
        DynamicKey key1 = DynamicKey<Type>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<Type>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicTypeKey>();
    }
    //--------------------------------------------------------------------------------

    #endregion Primitive Type Tests

    #region Enum Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;DayOfWeek&gt; enum creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DayOfWeekEnumKey_CreationAndCaching_WorksCorrectly ()
    {
        DayOfWeek testValue = DayOfWeek.Monday;
        DynamicKey key1 = DynamicKey<DayOfWeek>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<DayOfWeek>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicEnumKey<DayOfWeek>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;ConsoleColor&gt; enum creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ConsoleColorEnumKey_CreationAndCaching_WorksCorrectly ()
    {
        ConsoleColor testValue = ConsoleColor.Red;
        DynamicKey key1 = DynamicKey<ConsoleColor>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<ConsoleColor>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicEnumKey<ConsoleColor>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;FileAttributes&gt; enum creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_FileAttributesEnumKey_CreationAndCaching_WorksCorrectly ()
    {
        FileAttributes testValue = FileAttributes.ReadOnly;
        DynamicKey key1 = DynamicKey<FileAttributes>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<FileAttributes>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicEnumKey<FileAttributes>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Enum Type Tests

    #region Value Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;DateTime&gt; value type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DateTimeValueTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        DateTime testValue = DateTime.Now;
        DynamicKey key1 = DynamicKey<DateTime>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<DateTime>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<DateTime>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;TimeSpan&gt; value type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_TimeSpanValueTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        TimeSpan testValue = TimeSpan.FromMinutes(30);
        DynamicKey key1 = DynamicKey<TimeSpan>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<TimeSpan>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<TimeSpan>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;decimal&gt; value type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DecimalValueTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        decimal testValue = 123.45m;
        DynamicKey key1 = DynamicKey<decimal>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<decimal>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<decimal>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;float&gt; value type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_FloatValueTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        float testValue = 3.14f;
        DynamicKey key1 = DynamicKey<float>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<float>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<float>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;double&gt; value type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DoubleValueTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        double testValue = 3.14159;
        DynamicKey key1 = DynamicKey<double>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<double>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicValKey<double>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Value Type Tests

    #region Reference Type Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;object&gt; reference type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ObjectReferenceTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        object testValue = new { Name = "test", Value = 42 };
        DynamicKey key1 = DynamicKey<object>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<object>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<object>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;List&lt;int&gt;&gt; reference type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ListReferenceTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        List<int> testValue = [1, 2, 3, 4, 5];
        DynamicKey key1 = DynamicKey<List<int>>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<List<int>>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<List<int>>>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;Dictionary&lt;string, int&gt;&gt; reference type creation and caching.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DictionaryReferenceTypeKey_CreationAndCaching_WorksCorrectly ()
    {
        Dictionary<string, int> testValue = new() { ["a"] = 1, ["b"] = 2 };
        DynamicKey key1 = DynamicKey<Dictionary<string, int>>.GetKey(testValue);
        DynamicKey key2 = DynamicKey<Dictionary<string, int>>.GetKey(testValue);

        key1.Should().NotBeNull();
        key1.Should().BeSameAs(key2);
        key1.Should().BeOfType<DynamicRefKey<Dictionary<string, int>>>();
    }
    //--------------------------------------------------------------------------------

    #endregion Reference Type Tests

    #region Null Handling Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;string&gt; null handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_StringNullHandling_WorksCorrectly ()
    {
        DynamicKey key = DynamicKey<string>.GetKey(null!);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;object&gt; null handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ObjectNullHandling_WorksCorrectly ()
    {
        DynamicKey key = DynamicKey<object>.GetKey(null!);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests DynamicKey&lt;List&lt;int&gt;&gt; null handling.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_ListNullHandling_WorksCorrectly ()
    {
        DynamicKey key = DynamicKey<List<int>>.GetKey(null!);

        key.Should().NotBeNull();
        key.Should().Be(DynamicKey.Null);
    }
    //--------------------------------------------------------------------------------

    #endregion Null Handling Tests

    #region DynamicKey Passthrough Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that passing a DynamicKey to DynamicKey&lt;DynamicKey&gt; returns the same instance.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DynamicKeyPassthrough_ReturnsSameInstance ()
    {
        DynamicKey originalKey = DynamicKey.GetKey(42);
        DynamicKey passthroughKey = DynamicKey<DynamicKey>.GetKey(originalKey);

        passthroughKey.Should().BeSameAs(originalKey);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that passing a DynamicKey to DynamicKey&lt;object&gt; returns the same instance.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_DynamicKeyAsObject_ReturnsSameInstance ()
    {
        DynamicKey originalKey = DynamicKey.GetKey(42);
        DynamicKey passthroughKey = DynamicKey<object>.GetKey(originalKey);

        passthroughKey.Should().BeSameAs(originalKey);
    }
    //--------------------------------------------------------------------------------

    #endregion DynamicKey Passthrough Tests

    #region Equality and Comparison Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests equality between different generic key types with same values.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_GenericKeyEquality_WorksCorrectly ()
    {
        DynamicKey intKey = DynamicKey<int>.GetKey(42);
        DynamicKey longKey = DynamicKey<long>.GetKey(42L);
        DynamicKey stringKey = DynamicKey<string>.GetKey("42");

        intKey.Should().NotBe(longKey);
        intKey.Should().NotBe(stringKey);
        longKey.Should().NotBe(stringKey);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests comparison between different generic key types.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_GenericKeyComparison_WorksCorrectly ()
    {
        DynamicKey intKey = DynamicKey<int>.GetKey(10);
        DynamicKey longKey = DynamicKey<long>.GetKey(20L);

        intKey.CompareTo(longKey).Should().NotBe(0);
        longKey.CompareTo(intKey).Should().NotBe(0);
    }
    //--------------------------------------------------------------------------------

    #endregion Equality and Comparison Tests

    #region Hash Code Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that same values produce same hash codes across different generic types.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_GenericKeyHashCodes_AreConsistent ()
    {
        DynamicKey intKey1 = DynamicKey<int>.GetKey(42);
        DynamicKey intKey2 = DynamicKey<int>.GetKey(42);

        intKey1.GetHashCode().Should().Be(intKey2.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that different values produce different hash codes.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_GenericKeyHashCodes_AreDifferentForDifferentValues ()
    {
        DynamicKey intKey1 = DynamicKey<int>.GetKey(42);
        DynamicKey intKey2 = DynamicKey<int>.GetKey(100);

        intKey1.GetHashCode().Should().NotBe(intKey2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Hash Code Tests

    #region ToString Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests ToString for generic keys.
    /// </summary>
    [Fact]
    public void UsingDynamicKey_GenericKeyToString_WorksCorrectly ()
    {
        DynamicKey intKey = DynamicKey<int>.GetKey(42);
        DynamicKey stringKey = DynamicKey<string>.GetKey("test");
        DynamicKey enumKey = DynamicKey<DayOfWeek>.GetKey(DayOfWeek.Monday);

        intKey.ToString().Should().NotBeNullOrEmpty();
        stringKey.ToString().Should().NotBeNullOrEmpty();
        enumKey.ToString().Should().NotBeNullOrEmpty();
    }
    //--------------------------------------------------------------------------------

    #endregion ToString Tests
}
//################################################################################

