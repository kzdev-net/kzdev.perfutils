// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for composite DynamicKey functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKeyClass_CompositeKeys : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKeyClass_CompositeKeys"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKeyClass_CompositeKeys (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Two-Element Composite Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a two-element composite key using operator+.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_TwoElementComposite_OperatorPlus_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey composite = key1 + key2;

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a two-element composite key using Combine method.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_TwoElementComposite_Combine_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey composite = DynamicKey.Combine(key1, key2);

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a two-element composite key using builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_TwoElementComposite_Builder_WorksCorrectly ()
    {
        DynamicKey composite = DynamicKeyBuilder.Create()
            .AddInt(42)
            .AddString("test")
            .Build();

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that the same two-element composite returns the same instance (caching).
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_TwoElementComposite_Caching_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");

        DynamicKey composite1 = DynamicKey.Combine(key1, key2);
        DynamicKey composite2 = DynamicKey.Combine(key1, key2);

        composite1.Should().BeSameAs(composite2);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests two-element composite equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_TwoElementComposite_Equality_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");

        DynamicKey composite1 = DynamicKey.Combine(key1, key2);
        DynamicKey composite2 = DynamicKey.Combine(key1, key2);

        composite1.Should().Be(composite2);
        composite1.GetHashCode().Should().Be(composite2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Two-Element Composite Tests

    #region Three-Element Composite Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a three-element composite key using operator+.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ThreeElementComposite_OperatorPlus_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey key3 = DynamicKey.GetKey(true);
        DynamicKey composite = key1 + key2 + key3;

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a three-element composite key using Combine method.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ThreeElementComposite_Combine_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey key3 = DynamicKey.GetKey(true);
        DynamicKey composite = DynamicKey.Combine(key1, key2, key3);

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a three-element composite key using builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ThreeElementComposite_Builder_WorksCorrectly ()
    {
        DynamicKey composite = DynamicKeyBuilder.Create()
            .AddInt(42)
            .AddString("test")
            .AddBool(true)
            .Build();

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests three-element composite equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_ThreeElementComposite_Equality_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey key3 = DynamicKey.GetKey(true);

        DynamicKey composite1 = DynamicKey.Combine(key1, key2, key3);
        DynamicKey composite2 = DynamicKey.Combine(key1, key2, key3);

        composite1.Should().Be(composite2);
        composite1.GetHashCode().Should().Be(composite2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Three-Element Composite Tests

    #region Five-Element Composite Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a five-element composite key using Combine method.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_FiveElementComposite_Combine_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey key3 = DynamicKey.GetKey(true);
        DynamicKey key4 = DynamicKey.GetKey(Guid.NewGuid());
        DynamicKey key5 = DynamicKey.GetKey(typeof(string));
        DynamicKey composite = DynamicKey.Combine(key1, key2, key3, key4, key5);

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating a five-element composite key using builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_FiveElementComposite_Builder_WorksCorrectly ()
    {
        Guid testGuid = Guid.NewGuid();
        DynamicKey composite = DynamicKeyBuilder.Create()
            .AddInt(42)
            .AddString("test")
            .AddBool(true)
            .AddGuid(testGuid)
            .AddType(typeof(string))
            .Build();

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests five-element composite equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_FiveElementComposite_Equality_WorksCorrectly ()
    {
        Guid testGuid = Guid.NewGuid();
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");
        DynamicKey key3 = DynamicKey.GetKey(true);
        DynamicKey key4 = DynamicKey.GetKey(testGuid);
        DynamicKey key5 = DynamicKey.GetKey(typeof(string));

        DynamicKey composite1 = DynamicKey.Combine(key1, key2, key3, key4, key5);
        DynamicKey composite2 = DynamicKey.Combine(key1, key2, key3, key4, key5);

        composite1.Should().Be(composite2);
        composite1.GetHashCode().Should().Be(composite2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Five-Element Composite Tests

    #region Mixed Type Composite Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests creating composite keys with mixed types.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_MixedTypeComposite_WorksCorrectly ()
    {
        DynamicKey composite = DynamicKeyBuilder.Create()
            .AddInt(42)
            .AddLong(100L)
            .AddUInt(200U)
            .AddULong(300UL)
            .AddBool(true)
            .AddString("test")
            .AddGuid(Guid.NewGuid())
            .AddType(typeof(int))
            .AddEnum(DayOfWeek.Monday)
            .Build();

        composite.Should().NotBeNull();
        composite.Should().BeAssignableTo<DynamicKey>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests mixed type composite equality.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_MixedTypeComposite_Equality_WorksCorrectly ()
    {
        Guid testGuid = Guid.NewGuid();

        DynamicKey composite1 = DynamicKeyBuilder.Create()
            .AddInt(42)
            .AddString("test")
            .AddGuid(testGuid)
            .Build();

        DynamicKey composite2 = DynamicKeyBuilder.Create()
            .AddInt(42)
            .AddString("test")
            .AddGuid(testGuid)
            .Build();

        composite1.Should().Be(composite2);
        composite1.GetHashCode().Should().Be(composite2.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Mixed Type Composite Tests

    #region Composite Comparison Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests composite key comparison by length.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_CompositeComparison_ByLength_WorksCorrectly ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);
        DynamicKey key2 = DynamicKey.GetKey("test");

        DynamicKey twoElement = DynamicKey.Combine(key1, key2);
        DynamicKey threeElement = DynamicKey.Combine(key1, key2, DynamicKey.GetKey(true));

        twoElement.CompareTo(threeElement).Should().BeNegative();
        threeElement.CompareTo(twoElement).Should().BePositive();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests composite key comparison by element values.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_CompositeComparison_ByElements_WorksCorrectly ()
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

    #endregion Composite Comparison Tests

    #region Error Handling Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that combining with null keys throws exception.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_CompositeWithNullKey_ThrowsException ()
    {
        DynamicKey key1 = DynamicKey.GetKey(42);

        this.Invoking(_ => key1 + null!)
            .Should().Throw<ArgumentNullException>();

        this.Invoking(_ => null! + key1)
            .Should().Throw<ArgumentNullException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that empty Combine throws exception.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_EmptyCombine_ThrowsException ()
    {
        this.Invoking(_ => DynamicKey.Combine())
            .Should().Throw<ArgumentException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that building empty builder throws exception.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyClass_EmptyBuilder_ThrowsException ()
    {
        this.Invoking(_ => DynamicKeyBuilder.Create().Build())
            .Should().Throw<InvalidOperationException>();
    }
    //--------------------------------------------------------------------------------

    #endregion Error Handling Tests
}
//################################################################################
