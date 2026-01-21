// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for the <see cref="MemoryStreamSlim"/> class focusing on disposed behavior
/// compatibility with the BCL <see cref="MemoryStream"/> class.
/// </summary>
public partial class UsingMemoryStreamSlim
{
    #region Disposed Behavior Tests - ToArray

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.ToArray()"/> on a disposed
    /// dynamic mode stream throws an <see cref="ObjectDisposedException"/>.
    /// </summary>
    /// <remarks>
    /// Unlike BCL <see cref="MemoryStream"/>, MemoryStreamSlim in dynamic mode releases
    /// buffers back to the pool during disposal for memory efficiency. Therefore,
    /// ToArray() cannot work after disposal for dynamic mode streams, as the buffers
    /// are no longer available. This is an intentional design difference to maintain
    /// memory efficiency.
    /// </remarks>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_ToArray_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);

        stream.Dispose();

        // Dynamic mode releases buffers on disposal, so ToArray() cannot work
        // This is different from BCL MemoryStream but necessary for memory efficiency
        stream.Invoking(s => s.ToArray())
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.ToArray()"/> on a disposed
    /// fixed mode stream (wrapping a buffer) works correctly, matching BCL
    /// <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedMode_ToArray_ReturnsArray ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);
        long expectedLength = stream.Length;

        stream.Dispose();

        // BCL MemoryStream allows ToArray() after disposal
        byte[] result = stream.ToArray();
        result.Should().NotBeNull();
        result.Length.Should().Be((int)expectedLength);
        result.Should().BeEquivalentTo(sourceBuffer);
    }
    //--------------------------------------------------------------------------------

    #endregion Disposed Behavior Tests - ToArray

    #region Disposed Behavior Tests - Length Property

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.Length"/> property on a
    /// disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_Length_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);
        long expectedLength = stream.Length;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Length after disposal
        stream.Invoking(s => _ = s.Length)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.Length"/> property on a
    /// disposed fixed mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedMode_Length_ThrowsObjectDisposedException ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);
        long expectedLength = stream.Length;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Length after disposal
        stream.Invoking(s => _ = s.Length)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------

    #endregion Disposed Behavior Tests - Length Property

    #region Disposed Behavior Tests - Position Property

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.Position"/> property getter
    /// on a disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_PositionGetter_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);
        long expectedPosition = stream.Position;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Position getter after disposal
        stream.Invoking(s => _ = s.Position)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.Position"/> property getter
    /// on a disposed fixed mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedMode_PositionGetter_ThrowsObjectDisposedException ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);
        long expectedPosition = stream.Position;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Position getter after disposal
        stream.Invoking(s => _ = s.Position)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that setting the <see cref="MemoryStreamSlim.Position"/> property on a
    /// disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_PositionSetter_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when setting Position after disposal
        stream.Invoking(s => s.Position = 50)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that setting the <see cref="MemoryStreamSlim.Position"/> property on a
    /// disposed fixed mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedMode_PositionSetter_ThrowsObjectDisposedException ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when setting Position after disposal
        stream.Invoking(s => s.Position = 50)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------

    #endregion Disposed Behavior Tests - Position Property

    #region Disposed Behavior Tests - Capacity Property

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.Capacity"/> property getter
    /// on a disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_CapacityGetter_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        stream.Capacity = 100;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Capacity getter after disposal
        stream.Invoking(s => _ = s.Capacity)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.Capacity"/> property getter
    /// on a disposed fixed mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedMode_CapacityGetter_ThrowsObjectDisposedException ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);
        int expectedCapacity = stream.Capacity;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Capacity getter after disposal
        stream.Invoking(s => _ = s.Capacity)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that accessing the <see cref="MemoryStreamSlim.CapacityLong"/> property getter
    /// on a disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_CapacityLongGetter_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        stream.CapacityLong = 100;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Capacity getter after disposal
        stream.Invoking(s => _ = s.CapacityLong)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that setting the <see cref="MemoryStreamSlim.Capacity"/> property on a
    /// disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_CapacitySetter_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        stream.Capacity = 100;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when setting Capacity after disposal
        stream.Invoking(s => s.Capacity = 200)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that setting the <see cref="MemoryStreamSlim.CapacityLong"/> property on a
    /// disposed dynamic mode stream throws an <see cref="ObjectDisposedException"/>,
    /// matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_CapacityLongSetter_ThrowsObjectDisposedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        stream.CapacityLong = 100;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when setting Capacity after disposal
        stream.Invoking(s => s.CapacityLong = 200)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------

    #endregion Disposed Behavior Tests - Capacity Property

    #region Disposed Behavior Tests - GetBuffer and TryGetBuffer

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.GetBuffer()"/> on a disposed
    /// dynamic mode stream throws a <see cref="NotSupportedException"/> (not an
    /// ObjectDisposedException), as dynamic mode streams don't support GetBuffer()
    /// regardless of disposal state.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_GetBuffer_ThrowsNotSupportedException ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);

        stream.Dispose();

        // Dynamic mode doesn't support GetBuffer() regardless of disposal state
        stream.Invoking(s => s.GetBuffer())
            .Should()
            .Throw<NotSupportedException>()
            .WithMessage($"The operation is not supported with {nameof(MemoryStreamSlimMode.Dynamic)} mode MemoryStreamSlim instances.");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.GetBuffer()"/> on a disposed
    /// fixed mode stream with publiclyVisible=true works correctly, matching BCL
    /// <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedModePubliclyVisible_GetBuffer_ReturnsBuffer ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true, true);

        stream.Dispose();

        // BCL MemoryStream allows GetBuffer() after disposal if publiclyVisible was true
        byte[] result = stream.GetBuffer();
        result.Should().NotBeNull();
        result.Should().BeSameAs(sourceBuffer);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.GetBuffer()"/> on a disposed
    /// fixed mode stream with publiclyVisible=false throws an
    /// <see cref="UnauthorizedAccessException"/>, matching BCL <see cref="MemoryStream"/>
    /// behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedModeNotPubliclyVisible_GetBuffer_ThrowsUnauthorizedAccessException ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true, false);

        stream.Dispose();

        // BCL MemoryStream throws UnauthorizedAccessException if publiclyVisible was false
        stream.Invoking(s => s.GetBuffer())
            .Should()
            .Throw<UnauthorizedAccessException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.TryGetBuffer(out ArraySegment{byte})"/>
    /// on a disposed dynamic mode stream returns false, as dynamic mode streams don't
    /// support TryGetBuffer() regardless of disposal state.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedDynamicMode_TryGetBuffer_ReturnsFalse ()
    {
        MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);

        stream.Dispose();

        // Dynamic mode doesn't support TryGetBuffer() regardless of disposal state
        bool result = stream.TryGetBuffer(out ArraySegment<byte> buffer);
        result.Should().BeFalse();
        buffer.Array.Should().BeNull();
        buffer.Count.Should().Be(0);
        buffer.Offset.Should().Be(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.TryGetBuffer(out ArraySegment{byte})"/>
    /// on a disposed fixed mode stream with publiclyVisible=true returns true and the
    /// buffer, matching BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedModePubliclyVisible_TryGetBuffer_ReturnsTrue ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true, true);
        long expectedLength = stream.Length;

        stream.Dispose();

        // BCL MemoryStream allows TryGetBuffer() after disposal if publiclyVisible was true
        bool result = stream.TryGetBuffer(out ArraySegment<byte> buffer);
        result.Should().BeTrue();
        buffer.Array.Should().NotBeNull();
        buffer.Array.Should().BeSameAs(sourceBuffer);
        buffer.Count.Should().Be((int)expectedLength);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that calling <see cref="MemoryStreamSlim.TryGetBuffer(out ArraySegment{byte})"/>
    /// on a disposed fixed mode stream with publiclyVisible=false returns false, matching
    /// BCL <see cref="MemoryStream"/> behavior.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_DisposedFixedModeNotPubliclyVisible_TryGetBuffer_ReturnsFalse ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true, false);

        stream.Dispose();

        // BCL MemoryStream returns false if publiclyVisible was false
        bool result = stream.TryGetBuffer(out ArraySegment<byte> buffer);
        result.Should().BeFalse();
        buffer.Array.Should().BeNull();
        buffer.Count.Should().Be(0);
        buffer.Offset.Should().Be(0);
    }
    //--------------------------------------------------------------------------------

    #endregion Disposed Behavior Tests - GetBuffer and TryGetBuffer

    #region Disposed Behavior Tests - Comparison with BCL MemoryStream

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that BCL <see cref="MemoryStream.ToArray()"/> works after disposal to
    /// establish the baseline behavior. Note that MemoryStreamSlim in dynamic mode
    /// cannot match this behavior due to memory efficiency requirements (buffers are
    /// released on disposal).
    /// </summary>
    [Fact]
    public void UsingMemoryStream_Disposed_ToArray_ReturnsArray ()
    {
        MemoryStream stream = new();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);
        long expectedLength = stream.Length;

        stream.Dispose();

        // BCL MemoryStream allows ToArray() after disposal because it keeps the buffer
        // MemoryStreamSlim in dynamic mode cannot do this as it releases buffers for efficiency
        byte[] result = stream.ToArray();
        result.Should().NotBeNull();
        result.Length.Should().Be((int)expectedLength);
        result.Should().BeEquivalentTo(testData);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that BCL <see cref="MemoryStream.Length"/> throws after disposal to
    /// establish the baseline behavior we need to match.
    /// </summary>
    [Fact]
    public void UsingMemoryStream_Disposed_Length_ThrowsObjectDisposedException ()
    {
        MemoryStream stream = new();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Length after disposal
        stream.Invoking(s => _ = s.Length)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that BCL <see cref="MemoryStream.Position"/> throws after disposal to
    /// establish the baseline behavior we need to match.
    /// </summary>
    [Fact]
    public void UsingMemoryStream_Disposed_Position_ThrowsObjectDisposedException ()
    {
        MemoryStream stream = new();
        byte[] testData = new byte[100];
        GetRandomBytes(testData, testData.Length);
        stream.Write(testData, 0, testData.Length);

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Position after disposal
        stream.Invoking(s => _ = s.Position)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that BCL <see cref="MemoryStream.Capacity"/> throws after disposal to
    /// establish the baseline behavior we need to match.
    /// </summary>
    [Fact]
    public void UsingMemoryStream_Disposed_Capacity_ThrowsObjectDisposedException ()
    {
        MemoryStream stream = new();
        stream.Capacity = 100;

        stream.Dispose();

        // BCL MemoryStream throws ObjectDisposedException when accessing Capacity after disposal
        stream.Invoking(s => _ = s.Capacity)
            .Should()
            .Throw<ObjectDisposedException>();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that BCL <see cref="MemoryStream.GetBuffer()"/> works after disposal
    /// when publiclyVisible=true to establish the baseline behavior we need to match.
    /// </summary>
    [Fact]
    public void UsingMemoryStream_DisposedPubliclyVisible_GetBuffer_ReturnsBuffer ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStream stream = new(sourceBuffer, 0, sourceBuffer.Length, true, true);

        stream.Dispose();

        // BCL MemoryStream allows GetBuffer() after disposal if publiclyVisible was true
        byte[] result = stream.GetBuffer();
        result.Should().NotBeNull();
        result.Should().BeSameAs(sourceBuffer);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that BCL <see cref="MemoryStream.TryGetBuffer(out ArraySegment{byte})"/>
    /// works after disposal when publiclyVisible=true to establish the baseline behavior
    /// we need to match.
    /// </summary>
    [Fact]
    public void UsingMemoryStream_DisposedPubliclyVisible_TryGetBuffer_ReturnsTrue ()
    {
        byte[] sourceBuffer = new byte[100];
        GetRandomBytes(sourceBuffer, sourceBuffer.Length);
        MemoryStream stream = new(sourceBuffer, 0, sourceBuffer.Length, true, true);
        long expectedLength = stream.Length;

        stream.Dispose();

        // BCL MemoryStream allows TryGetBuffer() after disposal if publiclyVisible was true
        bool result = stream.TryGetBuffer(out ArraySegment<byte> buffer);
        result.Should().BeTrue();
        buffer.Array.Should().NotBeNull();
        buffer.Array.Should().BeSameAs(sourceBuffer);
        buffer.Count.Should().Be((int)expectedLength);
    }
    //--------------------------------------------------------------------------------

    #endregion Disposed Behavior Tests - Comparison with BCL MemoryStream
}
//################################################################################
