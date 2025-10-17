// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

/// <summary>
///   Comprehensive tests to verify that composite keys built using different approaches
///   (operator+, key builder, Combine factory method, and multi-key generic GetKey methods)
///   produce equivalent hash codes and pass equality tests when built from the same constituent pieces.
/// </summary>
public class UsingDynamicKey_CompositeKeyEquivalenceTests
{
    #region Test Data Generation

    /// <summary>
    ///   Generates a comprehensive set of test data combinations for testing composite key equivalence.
    /// </summary>
    /// <returns>
    ///   A collection of test data tuples containing the constituent values and expected composite key.
    /// </returns>
    public static IEnumerable<object[]> GetCompositeKeyTestData ()
    {
        // Test data for 2-element composites
        yield return [new object[] { 42, "test" }, 2];
        yield return [new object[] { "hello", 123L }, 2];
        yield return [new object[] { true, Guid.NewGuid() }, 2];
        yield return [new object[] { typeof(string), 456U }, 2];
        yield return [new object[] { DateTime.Now.Date, 789UL }, 2];

        // Test data for 3-element composites
        yield return [new object[] { 42, "test", true }, 3];
        yield return [new object[] { "hello", 123L, Guid.NewGuid() }, 3];
        yield return [new object[] { true, typeof(string), 456U }, 3];
        yield return [new object[] { DateTime.Now.Date, 789UL, "world" }, 3];
        yield return [new object[] { 999, TimeSpan.FromHours(1), false }, 3];

        // Test data for 4-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid() }, 4];
        yield return [new object[] { "hello", 123L, typeof(string), 456U }, 4];
        yield return [new object[] { true, DateTime.Now.Date, 789UL, "world" }, 4];
        yield return [new object[] { 999, TimeSpan.FromHours(1), false, typeof(int) }, 4];

        // Test data for 5-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string) }, 5];
        yield return [new object[] { "hello", 123L, DateTime.Now.Date, 456U, false }, 5];
        yield return [new object[] { true, 789UL, "world", TimeSpan.FromMinutes(30), typeof(long) }, 5];

        // Test data for 6-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L }, 6];
        yield return [new object[] { "hello", DateTime.Now.Date, 456U, false, TimeSpan.FromSeconds(45), typeof(bool) }, 6];

        // Test data for 7-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L, DateTime.Now.Date }, 7];
        yield return [new object[] { "hello", 456U, false, TimeSpan.FromMilliseconds(500), typeof(int), 789UL, true }, 7];

        // Test data for 8-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L, DateTime.Now.Date, 456U }, 8];
        yield return [new object[] { "hello", false, TimeSpan.FromTicks(1000), typeof(long), 789UL, true, Guid.NewGuid(), 999 }, 8];

        // Test data for 9-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L, DateTime.Now.Date, 456U, false }, 9];
        yield return [new object[] { "hello", TimeSpan.FromDays(1), typeof(bool), 789UL, true, Guid.NewGuid(), 999, DateTime.Now, "world" }, 9
        ];

        // Test data for 10-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L, DateTime.Now.Date, 456U, false, TimeSpan.FromHours(2) }, 10
        ];
        yield return [new object[] { "hello", 789UL, true, Guid.NewGuid(), 999, DateTime.Now, "world", typeof(int), false, 123L }, 10];

        // Test data for 11-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L, DateTime.Now.Date, 456U, false, TimeSpan.FromHours(2), 789UL }, 11
        ];
        yield return [new object[] { "hello", true, Guid.NewGuid(), 999, DateTime.Now, "world", typeof(int), false, 123L, TimeSpan.FromMinutes(15), typeof(bool) }, 11
        ];

        // Test data for 12-element composites
        yield return [new object[] { 42, "test", true, Guid.NewGuid(), typeof(string), 123L, DateTime.Now.Date, 456U, false, TimeSpan.FromHours(2), 789UL, "final" }, 12
        ];
        yield return [new object[] { "hello", true, Guid.NewGuid(), 999, DateTime.Now, "world", typeof(int), false, 123L, TimeSpan.FromMinutes(15), typeof(bool), 456U }, 12
        ];
    }

    #endregion

    #region 2-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from two values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The two values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 2 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void TwoElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 2) return; // Skip non-2-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 3-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from three values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The three values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 3 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void ThreeElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 3) return; // Skip non-3-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 4-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from four values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The four values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 4 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void FourElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 4) return; // Skip non-4-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 5-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from five values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The five values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 5 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void FiveElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 5) return; // Skip non-5-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 6-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from six values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The six values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 6 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void SixElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 6) return; // Skip non-6-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 7-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from seven values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The seven values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 7 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void SevenElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 7) return; // Skip non-7-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];
        object value7 = values[6];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6) + DynamicKey.GetKey(value7); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Add(value7).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6), DynamicKey.GetKey(value7)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6, value7); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 8-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from eight values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The eight values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 8 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void EightElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 8) return; // Skip non-8-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];
        object value7 = values[6];
        object value8 = values[7];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6) + DynamicKey.GetKey(value7) + DynamicKey.GetKey(value8); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Add(value7).Add(value8).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6), DynamicKey.GetKey(value7), DynamicKey.GetKey(value8)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6, value7, value8); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 9-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from nine values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The nine values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 9 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void NineElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 9) return; // Skip non-9-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];
        object value7 = values[6];
        object value8 = values[7];
        object value9 = values[8];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6) + DynamicKey.GetKey(value7) + DynamicKey.GetKey(value8) + DynamicKey.GetKey(value9); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Add(value7).Add(value8).Add(value9).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6), DynamicKey.GetKey(value7), DynamicKey.GetKey(value8), DynamicKey.GetKey(value9)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6, value7, value8, value9); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 10-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from ten values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The ten values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 10 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void TenElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 10) return; // Skip non-10-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];
        object value7 = values[6];
        object value8 = values[7];
        object value9 = values[8];
        object value10 = values[9];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6) + DynamicKey.GetKey(value7) + DynamicKey.GetKey(value8) + DynamicKey.GetKey(value9) + DynamicKey.GetKey(value10); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Add(value7).Add(value8).Add(value9).Add(value10).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6), DynamicKey.GetKey(value7), DynamicKey.GetKey(value8), DynamicKey.GetKey(value9), DynamicKey.GetKey(value10)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 11-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from eleven values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The eleven values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 11 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void ElevenElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 11) return; // Skip non-11-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];
        object value7 = values[6];
        object value8 = values[7];
        object value9 = values[8];
        object value10 = values[9];
        object value11 = values[10];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6) + DynamicKey.GetKey(value7) + DynamicKey.GetKey(value8) + DynamicKey.GetKey(value9) + DynamicKey.GetKey(value10) + DynamicKey.GetKey(value11); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Add(value7).Add(value8).Add(value9).Add(value10).Add(value11).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6), DynamicKey.GetKey(value7), DynamicKey.GetKey(value8), DynamicKey.GetKey(value9), DynamicKey.GetKey(value10), DynamicKey.GetKey(value11)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region 12-Element Composite Tests

    /// <summary>
    ///   Verifies that composite keys built from twelve values using different approaches
    ///   (operator+, builder, Combine, and multi-key generic) produce equivalent hash codes
    ///   and pass equality tests when constructed from the same constituent values.
    /// </summary>
    /// <param name="values">The twelve values to use for constructing the composite key.</param>
    /// <param name="expectedCount">The expected number of elements (should be 12 for this test).</param>
    [Theory]
    [MemberData(nameof(GetCompositeKeyTestData))]
    public void TwelveElementComposite_AllApproaches_ProduceEquivalentResults (object[] values, int expectedCount)
    {
        if (expectedCount != 12) return; // Skip non-12-element tests

        // Arrange
        object value1 = values[0];
        object value2 = values[1];
        object value3 = values[2];
        object value4 = values[3];
        object value5 = values[4];
        object value6 = values[5];
        object value7 = values[6];
        object value8 = values[7];
        object value9 = values[8];
        object value10 = values[9];
        object value11 = values[10];
        object value12 = values[11];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4) + DynamicKey.GetKey(value5) + DynamicKey.GetKey(value6) + DynamicKey.GetKey(value7) + DynamicKey.GetKey(value8) + DynamicKey.GetKey(value9) + DynamicKey.GetKey(value10) + DynamicKey.GetKey(value11) + DynamicKey.GetKey(value12); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Add(value5).Add(value6).Add(value7).Add(value8).Add(value9).Add(value10).Add(value11).Add(value12).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4), DynamicKey.GetKey(value5), DynamicKey.GetKey(value6), DynamicKey.GetKey(value7), DynamicKey.GetKey(value8), DynamicKey.GetKey(value9), DynamicKey.GetKey(value10), DynamicKey.GetKey(value11), DynamicKey.GetKey(value12)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12); // Multi-key generic

        // Assert - All approaches should produce equivalent results
        key1.GetHashCode().Should().Be(key2.GetHashCode(), "operator+ and builder should produce same hash code");
        key1.GetHashCode().Should().Be(key3.GetHashCode(), "operator+ and Combine should produce same hash code");
        key1.GetHashCode().Should().Be(key4.GetHashCode(), "operator+ and multi-key generic should produce same hash code");

        key1.Equals(key2).Should().BeTrue("operator+ and builder should be equal");
        key1.Equals(key3).Should().BeTrue("operator+ and Combine should be equal");
        key1.Equals(key4).Should().BeTrue("operator+ and multi-key generic should be equal");

        key2.Equals(key3).Should().BeTrue("builder and Combine should be equal");
        key2.Equals(key4).Should().BeTrue("builder and multi-key generic should be equal");
        key3.Equals(key4).Should().BeTrue("Combine and multi-key generic should be equal");
    }

    #endregion

    #region Dictionary Usage Tests

    /// <summary>
    ///   Verifies that composite keys built using different approaches can be used interchangeably
    ///   as dictionary keys, ensuring that equivalent keys access the same dictionary entries
    ///   regardless of how they were constructed.
    /// </summary>
    [Fact]
    public void CompositeKeysFromDifferentApproaches_WorkAsDictionaryKeys ()
    {
        // Arrange
        var testData = new { UserId = 42, SessionId = "abc123", IsAdmin = true, Timestamp = DateTime.Now.Date };
        Dictionary<DynamicKey, string> dictionary = new();

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(testData.UserId) + DynamicKey.GetKey(testData.SessionId) + DynamicKey.GetKey(testData.IsAdmin) + DynamicKey.GetKey(testData.Timestamp); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(testData.UserId).Add(testData.SessionId).Add(testData.IsAdmin).Add(testData.Timestamp).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(testData.UserId), DynamicKey.GetKey(testData.SessionId), DynamicKey.GetKey(testData.IsAdmin), DynamicKey.GetKey(testData.Timestamp)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(testData.UserId, testData.SessionId, testData.IsAdmin, testData.Timestamp); // Multi-key generic

        // Add to dictionary using first key
        dictionary[key1] = "UserData";
        Debug.WriteLine($"Key1 HashCode: {key1.GetHashCode()}");
        Debug.WriteLine($"Key2 HashCode: {key2.GetHashCode()}");

        // Assert - All equivalent keys should access the same dictionary entry
        dictionary.ContainsKey(key1).Should().BeTrue("operator+ key should be in dictionary");
        dictionary.ContainsKey(key2).Should().BeTrue("builder key should be in dictionary");
        dictionary.ContainsKey(key3).Should().BeTrue("Combine key should be in dictionary");
        dictionary.ContainsKey(key4).Should().BeTrue("multi-key generic key should be in dictionary");

        dictionary[key1].Should().Be("UserData", "operator+ key should return correct value");
        dictionary[key2].Should().Be("UserData", "builder key should return correct value");
        dictionary[key3].Should().Be("UserData", "Combine key should return correct value");
        dictionary[key4].Should().Be("UserData", "multi-key generic key should return correct value");
    }

    /// <summary>
    ///   Verifies that composite keys built using different approaches can be used interchangeably
    ///   as hash set elements, ensuring that equivalent keys are treated as the same element
    ///   regardless of how they were constructed.
    /// </summary>
    [Fact]
    public void CompositeKeysFromDifferentApproaches_WorkAsHashSetKeys ()
    {
        // Arrange
        var testData = new { UserId = 42, SessionId = "abc123", IsAdmin = true, Timestamp = DateTime.Now.Date };
        HashSet<DynamicKey> hashSet = [];

        // Act - Build composite keys using different approaches
        DynamicKey key1 = DynamicKey.GetKey(testData.UserId) + DynamicKey.GetKey(testData.SessionId) + DynamicKey.GetKey(testData.IsAdmin) + DynamicKey.GetKey(testData.Timestamp); // Operator+
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(testData.UserId).Add(testData.SessionId).Add(testData.IsAdmin).Add(testData.Timestamp).Build(); // Builder
        DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(testData.UserId), DynamicKey.GetKey(testData.SessionId), DynamicKey.GetKey(testData.IsAdmin), DynamicKey.GetKey(testData.Timestamp)); // Combine
        DynamicKey key4 = DynamicKey.GetKey(testData.UserId, testData.SessionId, testData.IsAdmin, testData.Timestamp); // Multi-key generic

        // Add to hash set using first key
        hashSet.Add(key1);

        // Assert - All equivalent keys should be considered present in the hash set
        hashSet.Contains(key1).Should().BeTrue("operator+ key should be in hash set");
        hashSet.Contains(key2).Should().BeTrue("builder key should be in hash set");
        hashSet.Contains(key3).Should().BeTrue("Combine key should be in hash set");
        hashSet.Contains(key4).Should().BeTrue("multi-key generic key should be in hash set");

        // Adding equivalent keys should not increase the count
        int initialCount = hashSet.Count;
        hashSet.Add(key2);
        hashSet.Add(key3);
        hashSet.Add(key4);
        hashSet.Count.Should().Be(initialCount, "adding equivalent keys should not increase hash set count");
    }

    #endregion

    #region Performance and Stress Tests

    /// <summary>
    ///   Performs a stress test by generating a large number (1000) of random composite keys
    ///   using all four construction approaches and verifies that equivalent keys consistently
    ///   produce the same hash codes and pass equality tests across all approaches.
    /// </summary>
    [Fact]
    public void LargeNumberOfCompositeKeys_AllApproachesProduceConsistentResults ()
    {
        // Arrange
        const int numberOfTests = 1000;
        Random random = new(42); // Fixed seed for reproducible tests
        List<(int HashCode1, int HashCode2, int HashCode3, int HashCode4, bool AllEqual)> testResults = [];

        // Act - Generate many random composite keys and test equivalence
        for (int i = 0; i < numberOfTests; i++)
        {
            object[] values = GenerateRandomValues(random, 4); // 4-element composites
            object value1 = values[0];
            object value2 = values[1];
            object value3 = values[2];
            object value4 = values[3];

            DynamicKey key1 = DynamicKey.GetKey(value1) + DynamicKey.GetKey(value2) + DynamicKey.GetKey(value3) + DynamicKey.GetKey(value4); // Operator+
            DynamicKey key2 = DynamicKeyBuilder.Create().Add(value1).Add(value2).Add(value3).Add(value4).Build(); // Builder
            DynamicKey key3 = DynamicKey.Combine(DynamicKey.GetKey(value1), DynamicKey.GetKey(value2), DynamicKey.GetKey(value3), DynamicKey.GetKey(value4)); // Combine
            DynamicKey key4 = DynamicKey.GetKey(value1, value2, value3, value4); // Multi-key generic

            int hash1 = key1.GetHashCode();
            int hash2 = key2.GetHashCode();
            int hash3 = key3.GetHashCode();
            int hash4 = key4.GetHashCode();

            bool allEqual = hash1 == hash2 && hash2 == hash3 && hash3 == hash4 &&
                            key1.Equals(key2) && key2.Equals(key3) && key3.Equals(key4);

            testResults.Add((hash1, hash2, hash3, hash4, allEqual));
        }

        // Assert - All tests should pass
        List<(int HashCode1, int HashCode2, int HashCode3, int HashCode4, bool AllEqual)> failedTests = testResults.Where(r => !r.AllEqual).ToList();
        failedTests.Should().BeEmpty($"All {numberOfTests} composite key equivalence tests should pass. Failed tests: {string.Join(", ", failedTests.Take(10))}");
    }

    /// <summary>
    ///   Verifies that composite keys can be constructed using mixed approaches (e.g., combining
    ///   operator+ with builder pattern) and still produce equivalent results to keys built
    ///   using a single approach with the same constituent values.
    /// </summary>
    [Fact]
    public void MixedApproachCompositeKeys_ProduceConsistentResults ()
    {
        // Arrange
        object[] baseValues = [42, "test", true, Guid.NewGuid(), typeof(string)];

        // Act - Create composite keys using mixed approaches
        DynamicKey key1 = DynamicKey.GetKey(baseValues[0]) + DynamicKey.GetKey(baseValues[1]) + DynamicKey.GetKey(baseValues[2]); // Operator+ for first 3
        DynamicKey key2 = DynamicKeyBuilder.Create().Add(baseValues[3]).Add(baseValues[4]).Build(); // Builder for last 2
        DynamicKey combinedKey = key1 + key2; // Combine the two approaches

        DynamicKey directKey = DynamicKey.GetKey(baseValues[0], baseValues[1], baseValues[2], baseValues[3], baseValues[4]); // Direct multi-key

        // Assert - Mixed approaches should produce equivalent results
        combinedKey.GetHashCode().Should().Be(directKey.GetHashCode(), "mixed approach and direct approach should produce same hash code");
        combinedKey.Equals(directKey).Should().BeTrue("mixed approach and direct approach should be equal");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    ///   Generates an array of random values for testing composite key equivalence.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="count">The number of values to generate.</param>
    /// <returns>An array of random values of different types.</returns>
    private static object[] GenerateRandomValues (Random random, int count)
    {
        object[] values = new object[count];
        Type[] types = [typeof(int), typeof(string), typeof(bool), typeof(Guid), typeof(Type), typeof(DateTime), typeof(TimeSpan)];

        for (int i = 0; i < count; i++)
        {
            Type type = types[random.Next(types.Length)];
            values[i] = GenerateRandomValue(random, type);
        }

        return values;
    }

    /// <summary>
    ///   Generates a random value of the specified type.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="type">The type of value to generate.</param>
    /// <returns>A random value of the specified type.</returns>
    private static object GenerateRandomValue (Random random, Type type)
    {
        return type switch
        {
            Type t when t == typeof(int) => random.Next(-1000, 1000),
            Type t when t == typeof(string) => $"test{random.Next(1000)}",
            Type t when t == typeof(bool) => random.Next(2) == 1,
            Type t when t == typeof(Guid) => Guid.NewGuid(),
            Type t when t == typeof(Type) => typeof(string),
            Type t when t == typeof(DateTime) => DateTime.Now.Date.AddDays(random.Next(-365, 365)),
            Type t when t == typeof(TimeSpan) => TimeSpan.FromMinutes(random.Next(1, 1440)),
            _ => throw new ArgumentException($"Unsupported type: {type}")
        };
    }

    #endregion
}
