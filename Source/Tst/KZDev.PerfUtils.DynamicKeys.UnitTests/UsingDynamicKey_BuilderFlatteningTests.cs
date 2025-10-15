// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for DynamicKeyBuilder flattening behavior with DynamicCompositeKey
/// </summary>
[Trait(TestConstants.TestTrait.Category, "DynamicKey")]
public class UsingDynamicKey_BuilderFlatteningTests : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingDynamicKey_BuilderFlatteningTests"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingDynamicKey_BuilderFlatteningTests (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Add Method Flattening Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding a DynamicCompositeKey flattens it in the builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_AddCompositeKey_FlattensCorrectly ()
    {
        DynamicKey compositeKey = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(compositeKey)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(2);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(42));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey("test"));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding a value that creates a DynamicCompositeKey flattens it.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_AddValueThatCreatesComposite_FlattensCorrectly ()
    {
        // Create a composite key using operator+
        DynamicKey compositeKey = DynamicKey.GetKey(42) + DynamicKey.GetKey("test");

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(compositeKey)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(2);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(42));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey("test"));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding a single key doesn't flatten it.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_AddSingleKey_DoesNotFlatten ()
    {
        DynamicKey singleKey = DynamicKey.GetKey(42);

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(singleKey)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeSameAs(singleKey);
    }
    //--------------------------------------------------------------------------------

    #endregion Add Method Flattening Tests

    #region Add<T> Method Flattening Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding a value that results in a DynamicCompositeKey flattens it.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_AddTValueThatCreatesComposite_FlattensCorrectly ()
    {
        // Create a composite key using operator+
        DynamicKey compositeKey = DynamicKey.GetKey(42) + DynamicKey.GetKey("test");

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(compositeKey)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(2);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(42));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey("test"));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding a primitive value doesn't create a composite.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_AddTPrimitiveValue_DoesNotCreateComposite ()
    {
        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(42)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeSameAs(DynamicKey.GetKey(42));
    }
    //--------------------------------------------------------------------------------

    #endregion Add<T> Method Flattening Tests

    #region Mixed Flattening Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests mixing single keys and composite keys in the builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_MixedSingleAndCompositeKeys_FlattensCorrectly ()
    {
        DynamicKey compositeKey = DynamicKey.Combine(
            DynamicKey.GetKey("hello"),
            DynamicKey.GetKey("world"));

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(42)                    // Single key
            .Add(compositeKey)          // Composite key (should flatten)
            .Add(true)                  // Single key
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(4);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(42));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey("hello"));
        resultComposite.Keys[2].Should().Be(DynamicKey.GetKey("world"));
        resultComposite.Keys[3].Should().Be(DynamicKey.GetKey(true));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests multiple composite keys in the builder.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_MultipleCompositeKeys_FlattensCorrectly ()
    {
        DynamicKey compositeKey1 = DynamicKey.Combine(
            DynamicKey.GetKey(1),
            DynamicKey.GetKey(2));
        DynamicKey compositeKey2 = DynamicKey.Combine(
            DynamicKey.GetKey(3),
            DynamicKey.GetKey(4));

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(compositeKey1)
            .Add(compositeKey2)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(4);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(1));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey(2));
        resultComposite.Keys[2].Should().Be(DynamicKey.GetKey(3));
        resultComposite.Keys[3].Should().Be(DynamicKey.GetKey(4));
    }
    //--------------------------------------------------------------------------------

    #endregion Mixed Flattening Tests

    #region Nested Composite Key Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that nested composite keys are flattened to a single level.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_NestedCompositeKeys_FlattensToSingleLevel ()
    {
        // Create nested composite: ((1, 2), (3, 4))
        DynamicKey innerComposite1 = DynamicKey.Combine(
            DynamicKey.GetKey(1),
            DynamicKey.GetKey(2));
        DynamicKey innerComposite2 = DynamicKey.Combine(
            DynamicKey.GetKey(3),
            DynamicKey.GetKey(4));
        DynamicKey outerComposite = DynamicKey.Combine(
            innerComposite1,
            innerComposite2);

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(outerComposite)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(4);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(1));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey(2));
        resultComposite.Keys[2].Should().Be(DynamicKey.GetKey(3));
        resultComposite.Keys[3].Should().Be(DynamicKey.GetKey(4));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that deeply nested composite keys are flattened to a single level.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_DeeplyNestedCompositeKeys_FlattensToSingleLevel ()
    {
        // Create deeply nested composite: (((1, 2), 3), 4)
        DynamicKey level1 = DynamicKey.Combine(
            DynamicKey.GetKey(1),
            DynamicKey.GetKey(2));
        DynamicKey level2 = DynamicKey.Combine(
            level1,
            DynamicKey.GetKey(3));
        DynamicKey level3 = DynamicKey.Combine(
            level2,
            DynamicKey.GetKey(4));

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(level3)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(4);
        resultComposite.Keys[0].Should().Be(DynamicKey.GetKey(1));
        resultComposite.Keys[1].Should().Be(DynamicKey.GetKey(2));
        resultComposite.Keys[2].Should().Be(DynamicKey.GetKey(3));
        resultComposite.Keys[3].Should().Be(DynamicKey.GetKey(4));
    }
    //--------------------------------------------------------------------------------

    #endregion Nested Composite Key Tests

    #region Equality Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that flattened composite keys are equal to manually created ones.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_FlattenedCompositeKeys_EqualManuallyCreated ()
    {
        DynamicKey compositeKey = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));

        DynamicKey flattened = DynamicKeyBuilder.Create()
            .Add(compositeKey)
            .Build();

        DynamicKey manual = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));

        flattened.Should().Be(manual);
        flattened.GetHashCode().Should().Be(manual.GetHashCode());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that mixed flattened composite keys are equal to manually created ones.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_MixedFlattenedCompositeKeys_EqualManuallyCreated ()
    {
        DynamicKey compositeKey = DynamicKey.Combine(
            DynamicKey.GetKey("hello"),
            DynamicKey.GetKey("world"));

        DynamicKey flattened = DynamicKeyBuilder.Create()
            .Add(42)
            .Add(compositeKey)
            .Add(true)
            .Build();

        DynamicKey manual = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("hello"),
            DynamicKey.GetKey("world"),
            DynamicKey.GetKey(true));

        flattened.Should().Be(manual);
        flattened.GetHashCode().Should().Be(manual.GetHashCode());
    }
    //--------------------------------------------------------------------------------

    #endregion Equality Tests

    #region Performance Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that flattening doesn't significantly impact performance.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_FlatteningPerformance_IsAcceptable ()
    {
        const int iterations = 1000;

        // Create a composite key
        DynamicKey compositeKey = DynamicKey.Combine(
            DynamicKey.GetKey(1),
            DynamicKey.GetKey(2),
            DynamicKey.GetKey(3));

        // Measure time for flattening
        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            DynamicKey result = DynamicKeyBuilder.Create()
                .Add(compositeKey)
                .Build();

            result.Should().NotBeNull();
        }

        stopwatch.Stop();

        // Should complete in reasonable time (less than 1 second for 1000 iterations)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
    }
    //--------------------------------------------------------------------------------

    #endregion Performance Tests

    #region Edge Case Tests

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding a single-element composite key flattens correctly.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_SingleElementCompositeKey_FlattensCorrectly ()
    {
        // This shouldn't happen in practice, but test the edge case
        DynamicKey singleElementComposite = DynamicKey.Combine(
            DynamicKey.GetKey(42));

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(singleElementComposite)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeSameAs(DynamicKey.GetKey(42));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that adding an empty builder with a composite key works.
    /// </summary>
    [Fact]
    public void UsingDynamicKeyBuilder_EmptyBuilderWithCompositeKey_WorksCorrectly ()
    {
        DynamicKey compositeKey = DynamicKey.Combine(
            DynamicKey.GetKey(42),
            DynamicKey.GetKey("test"));

        DynamicKey result = DynamicKeyBuilder.Create()
            .Add(compositeKey)
            .Build();

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicCompositeKey>();

        DynamicCompositeKey resultComposite = (DynamicCompositeKey)result;
        resultComposite.Count.Should().Be(2);
    }
    //--------------------------------------------------------------------------------

    #endregion Edge Case Tests
}
//################################################################################

