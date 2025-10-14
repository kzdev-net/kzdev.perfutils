// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils;
using KZDev.PerfUtils.Tests;
using DynamicKeyClass = KZDev.PerfUtils.DynamicKey;

namespace KZDev.PerfUtils.Tests.DynamicKey;

//################################################################################
/// <summary>
/// Unit tests for composite DynamicKeyClass functionality
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKeyClass")]
public class UsingDynamicKeyClass_CompositeKeys : UnitTestBase
{
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Initializes a new instance of the <see cref="UsingDynamicKeyClass_CompositeKeys"/> class.
  /// </summary>
  /// <param name="xUnitTestOutputHelper">
  /// The Xunit test output helper that can be used to output test messages
  /// </param>
  public UsingDynamicKeyClass_CompositeKeys(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
  {
  }
  //--------------------------------------------------------------------------------

  #region Two-Element Composite Tests

  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a two-element composite key using operator+.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_TwoElementComposite_OperatorPlus_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass composite = key1 + key2;

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a two-element composite key using Combine method.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_TwoElementComposite_Combine_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass composite = DynamicKeyClass.Combine(key1, key2);

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a two-element composite key using builder.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_TwoElementComposite_Builder_WorksCorrectly()
  {
    DynamicKeyClass composite = DynamicKeyBuilder.Create()
        .AddInt(42)
        .AddString("test")
        .Build();

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that the same two-element composite returns the same instance (caching).
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_TwoElementComposite_Caching_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(key1, key2);
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(key1, key2);

    composite1.Should().BeSameAs(composite2);
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests two-element composite equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_TwoElementComposite_Equality_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(key1, key2);
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(key1, key2);

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
  public void UsingDynamicKeyClass_ThreeElementComposite_OperatorPlus_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass key3 = DynamicKeyClass.GetKey(true);
    DynamicKeyClass composite = key1 + key2 + key3;

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a three-element composite key using Combine method.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ThreeElementComposite_Combine_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass key3 = DynamicKeyClass.GetKey(true);
    DynamicKeyClass composite = DynamicKeyClass.Combine(key1, key2, key3);

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a three-element composite key using builder.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ThreeElementComposite_Builder_WorksCorrectly()
  {
    DynamicKeyClass composite = DynamicKeyBuilder.Create()
        .AddInt(42)
        .AddString("test")
        .AddBool(true)
        .Build();

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests three-element composite equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_ThreeElementComposite_Equality_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass key3 = DynamicKeyClass.GetKey(true);

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(key1, key2, key3);
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(key1, key2, key3);

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
  public void UsingDynamicKeyClass_FiveElementComposite_Combine_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass key3 = DynamicKeyClass.GetKey(true);
    DynamicKeyClass key4 = DynamicKeyClass.GetKey(Guid.NewGuid());
    DynamicKeyClass key5 = DynamicKeyClass.GetKey(typeof(string));
    DynamicKeyClass composite = DynamicKeyClass.Combine(key1, key2, key3, key4, key5);

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests creating a five-element composite key using builder.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_FiveElementComposite_Builder_WorksCorrectly()
  {
    Guid testGuid = Guid.NewGuid();
    DynamicKeyClass composite = DynamicKeyBuilder.Create()
        .AddInt(42)
        .AddString("test")
        .AddBool(true)
        .AddGuid(testGuid)
        .AddType(typeof(string))
        .Build();

    composite.Should().NotBeNull();
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests five-element composite equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_FiveElementComposite_Equality_WorksCorrectly()
  {
    Guid testGuid = Guid.NewGuid();
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass key3 = DynamicKeyClass.GetKey(true);
    DynamicKeyClass key4 = DynamicKeyClass.GetKey(testGuid);
    DynamicKeyClass key5 = DynamicKeyClass.GetKey(typeof(string));

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(key1, key2, key3, key4, key5);
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(key1, key2, key3, key4, key5);

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
  public void UsingDynamicKeyClass_MixedTypeComposite_WorksCorrectly()
  {
    DynamicKeyClass composite = DynamicKeyBuilder.Create()
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
    composite.Should().BeAssignableTo<DynamicKeyClass>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests mixed type composite equality.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_MixedTypeComposite_Equality_WorksCorrectly()
  {
    Guid testGuid = Guid.NewGuid();

    DynamicKeyClass composite1 = DynamicKeyBuilder.Create()
        .AddInt(42)
        .AddString("test")
        .AddGuid(testGuid)
        .Build();

    DynamicKeyClass composite2 = DynamicKeyBuilder.Create()
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
  public void UsingDynamicKeyClass_CompositeComparison_ByLength_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");

    DynamicKeyClass twoElement = DynamicKeyClass.Combine(key1, key2);
    DynamicKeyClass threeElement = DynamicKeyClass.Combine(key1, key2, DynamicKeyClass.GetKey(true));

    twoElement.CompareTo(threeElement).Should().BeNegative();
    threeElement.CompareTo(twoElement).Should().BePositive();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests composite key comparison by element values.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_CompositeComparison_ByElements_WorksCorrectly()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(10);
    DynamicKeyClass key2 = DynamicKeyClass.GetKey("test");
    DynamicKeyClass key3 = DynamicKeyClass.GetKey(20);
    DynamicKeyClass key4 = DynamicKeyClass.GetKey("test");

    DynamicKeyClass composite1 = DynamicKeyClass.Combine(key1, key2);
    DynamicKeyClass composite2 = DynamicKeyClass.Combine(key3, key4);

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
  public void UsingDynamicKeyClass_CompositeWithNullKey_ThrowsException()
  {
    DynamicKeyClass key1 = DynamicKeyClass.GetKey(42);

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
  public void UsingDynamicKeyClass_EmptyCombine_ThrowsException()
  {
    this.Invoking(_ => DynamicKeyClass.Combine())
        .Should().Throw<ArgumentException>();
  }
  //--------------------------------------------------------------------------------
  /// <summary>
  /// Tests that building empty builder throws exception.
  /// </summary>
  [Fact]
  public void UsingDynamicKeyClass_EmptyBuilder_ThrowsException()
  {
    this.Invoking(_ => DynamicKeyBuilder.Create().Build())
        .Should().Throw<InvalidOperationException>();
  }
  //--------------------------------------------------------------------------------

  #endregion Error Handling Tests
}
//################################################################################
