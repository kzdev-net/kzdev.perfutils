// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

using KZDev.PerfUtils.Tests;

using ThrowHelper = KZDev.PerfUtils.Helpers.ThrowHelper;

namespace KZDev.PerfUtils.Common.UnitTests;

//################################################################################
/// <summary>
/// Unit tests for the <see cref="ThrowHelper"/> class.
/// </summary>
public class UsingThrowHelper : UnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingThrowHelper"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingThrowHelper(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------

    #region Test Methods

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum"/> contract.
    /// </summary>
    [Fact]
    public void NeedNonNegNum_WithArgumentName_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "length";

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(ArgumentName))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain(ArgumentName).And.Contain("greater than or equal to zero");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween"/> contract.
    /// </summary>
    [Fact]
    public void NeedBetween_WithBounds_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "offset";
        const long Minimum = 1L;
        const long Maximum = 10L;

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(ArgumentName, Minimum, Maximum))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain(ArgumentName).And.Contain(Minimum.ToString()).And.Contain(Maximum.ToString());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRangeException_NeedMinValue"/> contract.
    /// </summary>
    [Fact]
    public void NeedMinValue_WithMinimum_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "capacity";
        const long Minimum = 42L;

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedMinValue(ArgumentName, Minimum))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain(ArgumentName).And.Contain(Minimum.ToString());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException"/> contract.
    /// </summary>
    [Fact]
    public void IndexMustBeLess_WithArgumentName_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "index";

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(ArgumentName))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("Index was out of range");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException"/> contract.
    /// </summary>
    [Fact]
    public void IndexMustBeLessOrEqual_WithArgumentName_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "index";

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(ArgumentName))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("less than or equal to the size");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentException_InvalidSeekOrigin"/> contract.
    /// </summary>
    [Fact]
    public void InvalidSeekOrigin_WithArgumentName_ThrowsArgumentException()
    {
        const string ArgumentName = "origin";

        ArgumentException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentException_InvalidSeekOrigin(ArgumentName))
            .Should().Throw<ArgumentException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("Invalid seek origin");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentException_InvalidOffsetLength"/> contract.
    /// </summary>
    [Fact]
    public void InvalidOffsetLength_OnInvoke_ThrowsArgumentException()
    {
        ArgumentException exception = this.Invoking(_ => ThrowHelper.ThrowArgumentException_InvalidOffsetLength())
            .Should().Throw<ArgumentException>().Which;

        exception.Message.Should().Contain("Offset and length were out of bounds");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentException_DestinationTooShort"/> contract.
    /// </summary>
    [Fact]
    public void DestinationTooShort_WithArgumentName_ThrowsArgumentException()
    {
        const string ArgumentName = "destination";

        ArgumentException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentException_DestinationTooShort(ArgumentName))
            .Should().Throw<ArgumentException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("Destination is too short");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentException_SourceStreamMustBeReadable"/> contract.
    /// </summary>
    [Fact]
    public void SourceStreamMustBeReadable_WithArgumentName_ThrowsArgumentException()
    {
        const string ArgumentName = "source";

        ArgumentException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentException_SourceStreamMustBeReadable(ArgumentName))
            .Should().Throw<ArgumentException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("readable");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentException_SourceStreamMustBeSeekable"/> contract.
    /// </summary>
    [Fact]
    public void SourceStreamMustBeSeekable_WithArgumentName_ThrowsArgumentException()
    {
        const string ArgumentName = "source";

        ArgumentException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentException_SourceStreamMustBeSeekable(ArgumentName))
            .Should().Throw<ArgumentException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("seekable");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentException_ContainedValueIsNotComparable"/> contract.
    /// </summary>
    [Fact]
    public void ContainedValueIsNotComparable_WithArgumentName_ThrowsArgumentException()
    {
        const string ArgumentName = "value";

        ArgumentException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentException_ContainedValueIsNotComparable(ArgumentName))
            .Should().Throw<ArgumentException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("not comparable");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowInvalidOperationException_StreamClosed"/> contract.
    /// </summary>
    [Fact]
    public void StreamClosed_OnInvoke_ThrowsInvalidOperationException()
    {
        this.Invoking(_ => ThrowHelper.ThrowInvalidOperationException_StreamClosed())
            .Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Contain("closed Stream");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowObjectDisposedException_StreamClosed"/> contract.
    /// </summary>
    [Fact]
    public void StreamDisposed_WithObjectName_ThrowsObjectDisposedException()
    {
        const string ObjectName = "MemoryStreamSlim";

        ObjectDisposedException exception = this.Invoking(_ =>
                ThrowHelper.ThrowObjectDisposedException_StreamClosed(ObjectName))
            .Should().Throw<ObjectDisposedException>().Which;

        exception.ObjectName.Should().Be(ObjectName);
        exception.Message.Should().Contain("closed Stream");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowNotSupportedException_UnwritableStream"/> contract.
    /// </summary>
    [Fact]
    public void UnwritableStream_OnInvoke_ThrowsNotSupportedException()
    {
        this.Invoking(_ => ThrowHelper.ThrowNotSupportedException_UnwritableStream())
            .Should().Throw<NotSupportedException>()
            .Which.Message.Should().Contain("does not support writing");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowNotSupportedException_InvalidMemoryStreamSlimMode"/> contract.
    /// </summary>
    [Fact]
    public void InvalidMemoryStreamSlimMode_WithFixedMode_ThrowsNotSupportedException()
    {
        this.Invoking(_ => ThrowHelper.ThrowNotSupportedException_InvalidMemoryStreamSlimMode(MemoryStreamSlimMode.Fixed))
            .Should().Throw<NotSupportedException>()
            .Which.Message.Should().Contain("Fixed").And.Contain("MemoryStreamSlim");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowNotSupportedException_InvalidMemoryStreamSlimMode"/> for dynamic mode.
    /// </summary>
    [Fact]
    public void InvalidMemoryStreamSlimMode_WithDynamicMode_ThrowsNotSupportedException()
    {
        this.Invoking(_ => ThrowHelper.ThrowNotSupportedException_InvalidMemoryStreamSlimMode(MemoryStreamSlimMode.Dynamic))
            .Should().Throw<NotSupportedException>()
            .Which.Message.Should().Contain("Dynamic").And.Contain("MemoryStreamSlim");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowNotSupportedException_FeatureNotSupported"/> contract.
    /// </summary>
    [Fact]
    public void FeatureNotSupported_OnInvoke_ThrowsNotSupportedException()
    {
        this.Invoking(_ => ThrowHelper.ThrowNotSupportedException_FeatureNotSupported())
            .Should().Throw<NotSupportedException>()
            .Which.Message.Should().Contain("not supported");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRangeException_CapacitySmall"/> contract.
    /// </summary>
    [Fact]
    public void CapacitySmall_WithArgumentName_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "capacity";

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRangeException_CapacitySmall(ArgumentName))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("Requested capacity is less than current size");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRangeException_StreamLength"/> contract.
    /// </summary>
    [Fact]
    public void StreamLength_WithArgumentName_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "value";

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRangeException_StreamLength(ArgumentName))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("Stream length must be non-negative");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowArgumentOutOfRangeException_TooLargeForArray"/> contract.
    /// </summary>
    [Fact]
    public void TooLargeForArray_WithArgumentName_ThrowsArgumentOutOfRangeException()
    {
        const string ArgumentName = "count";

        ArgumentOutOfRangeException exception = this.Invoking(_ =>
                ThrowHelper.ThrowArgumentOutOfRangeException_TooLargeForArray(ArgumentName))
            .Should().Throw<ArgumentOutOfRangeException>().Which;

        exception.ParamName.Should().Be(ArgumentName);
        exception.Message.Should().Contain("too large for an array");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowIOException_StreamTooLong"/> contract.
    /// </summary>
    [Fact]
    public void StreamTooLong_OnInvoke_ThrowsIOException()
    {
        this.Invoking(_ => ThrowHelper.ThrowIOException_StreamTooLong())
            .Should().Throw<IOException>()
            .Which.Message.Should().Contain("too long");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowIOException_SeekBeforeBegin"/> contract.
    /// </summary>
    [Fact]
    public void SeekBeforeBegin_OnInvoke_ThrowsIOException()
    {
        this.Invoking(_ => ThrowHelper.ThrowIOException_SeekBeforeBegin())
            .Should().Throw<IOException>()
            .Which.Message.Should().Contain("before the beginning");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowInvalidOperationException_IntOverflowCapacity"/> contract.
    /// </summary>
    [Fact]
    public void IntOverflowCapacity_OnInvoke_ThrowsInvalidOperationException()
    {
        this.Invoking(_ => ThrowHelper.ThrowInvalidOperationException_IntOverflowCapacity())
            .Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Contain("int.MaxValue");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowInvalidOperationException_TooLargeToCopyToArray"/> contract.
    /// </summary>
    [Fact]
    public void TooLargeToCopyToArray_OnInvoke_ThrowsInvalidOperationException()
    {
        this.Invoking(_ => ThrowHelper.ThrowInvalidOperationException_TooLargeToCopyToArray())
            .Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Contain("too long to copy into an array");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowInvalidOperation_GlobalSettingsCantBeUpdated"/> contract.
    /// </summary>
    [Fact]
    public void GlobalSettingsCantBeUpdated_WithTypeName_ThrowsInvalidOperationException()
    {
        const string classTypeName = "MemoryStreamSlim";

        InvalidOperationException exception = this.Invoking(_ =>
                ThrowHelper.ThrowInvalidOperation_GlobalSettingsCantBeUpdated(classTypeName))
            .Should().Throw<InvalidOperationException>().Which;

        exception.Message.Should().Contain("global default settings").And.Contain(classTypeName);
        exception.Message.Split(classTypeName, StringSplitOptions.None).Length.Should().Be(3);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies <see cref="ThrowHelper.ThrowObjectDisposedException"/> contract.
    /// </summary>
    [Fact]
    public void ObjectDisposed_WithObjectName_ThrowsObjectDisposedException()
    {
        const string ObjectName = "owner";

        ObjectDisposedException exception = this.Invoking(_ => ThrowHelper.ThrowObjectDisposedException(ObjectName))
            .Should().Throw<ObjectDisposedException>().Which;

        exception.ObjectName.Should().Be(ObjectName);
        exception.Message.Should().Contain("disposed");
    }
    //--------------------------------------------------------------------------------

    //================================================================================

    #endregion Test Methods
}
//################################################################################
