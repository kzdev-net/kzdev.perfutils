// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Observability;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// A memory stream slim implementation that simply wraps a <see cref="MemoryStream"/>
/// for cases where the stream is created with a buffer that is already allocated.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
internal sealed class MemoryStreamWrapper : MemoryStreamSlim
{
    /// <summary>
    /// The wrapped memory stream instance.
    /// </summary>
    private readonly MemoryStream _wrappedStream;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the (debug) display value.
    /// </summary>
    /// <value>
    /// The (debug) display value.
    /// </value>
    [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
    private string DebugDisplayValue => $@"{nameof(MemoryStreamSlim)}: Length = {Length}, Position = {Position}, Mode = {nameof(MemoryStreamSlimMode.Fixed)}";
#pragma warning restore HAA0601
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Finalizes an instance of the <see cref="MemoryStreamWrapper"/> class.
    /// </summary>
    ~MemoryStreamWrapper ()
    {
        UtilsEventSource.Log.MemoryStreamSlimFinalized(Id);
        Dispose(false);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Validates a logical byte length for fixed-mode <see cref="ToMemoryInternal"/> materialization,
    /// matching observable limits used for <see cref="SegmentMemoryStreamSlim"/> ToArray/ToMemory paths.
    /// </summary>
    /// <param name="byteLength">
    /// The number of bytes to materialize.
    /// </param>
    /// <returns>
    /// Zero when <paramref name="byteLength"/> is zero; otherwise the validated count.
    /// </returns>
    private static int GetFixedModeMaterializationByteCountOrThrow (long byteLength)
    {
        switch (byteLength)
        {
            case 0:
                return 0;
            case < 0:
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(byteLength));
                break;
            case > int.MaxValue:
                ThrowHelper.ThrowInvalidOperationException_TooLargeToCopyToArray();
                break;
        }

        int bytesToCopy = (int)byteLength;
        if (bytesToCopy > Array.MaxLength)
        {
            ThrowHelper.ThrowInvalidOperationException_TooLargeToCopyToArray();
        }

        return bytesToCopy;
    }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new non-resizable instance of the <see cref="MemoryStreamWrapper"/> 
    /// class based on the specified byte array.
    /// </summary>
    /// <param name="stream">
    /// The stream instance to wrap.
    /// </param>
    public MemoryStreamWrapper (MemoryStream stream) : base(stream.Capacity, MemoryStreamSlimMode.Fixed,
        new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.OnRelease })
    {
        _wrappedStream = stream;
        IsOpen = true;
        UtilsEventSource.Log.MemoryStreamSlimCreated(Id, stream.Capacity, MemoryStreamSlimMode.Fixed, Settings);
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------

    #region Overrides of MemoryStreamSlim

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override long CapacityLong
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.Capacity;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            switch (value)
            {
                case < 0:
                    ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(Capacity));
                    break;

                case > int.MaxValue:
                    ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(Capacity), 0, int.MaxValue);
                    break;
            }
            _wrappedStream.Capacity = (int)value;
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.Capacity;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (value < 0) 
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(Capacity));
            _wrappedStream.Capacity = value;
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool CanSeek
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.CanSeek;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string Decode (Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        return encoding.GetString(_wrappedStream.GetBuffer(), 0, (int)_wrappedStream.Length);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool CanRead
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.CanRead;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool CanWrite
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.CanWrite;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Flush ()
    {
        _wrappedStream.Flush();
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Task FlushAsync (CancellationToken cancellationToken) => _wrappedStream.FlushAsync(cancellationToken);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override byte[] GetBuffer () => _wrappedStream.GetBuffer();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool TryGetBuffer (out ArraySegment<byte> buffer) => _wrappedStream.TryGetBuffer(out buffer);
    //--------------------------------------------------------------------------------

    #region ToMemory fixed-mode paths

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Rents a buffer for <paramref name="bytesToCopy"/> payload bytes (must be &gt; 0), copies
    /// <paramref name="source"/> into the visible prefix, emits ToMemory telemetry, and returns a wrapping owner.
    /// </summary>
    /// <param name="memoryPool">
    /// The pool to rent from.
    /// </param>
    /// <param name="bytesToCopy">
    /// The logical stream length; must be positive.
    /// </param>
    /// <param name="source">
    /// Exactly <paramref name="bytesToCopy"/> bytes to copy.
    /// </param>
    /// <returns>
    /// A <see cref="PooledMemoryStreamSlimMemoryOwner"/> over the rental.
    /// </returns>
    private IMemoryOwner<byte> CreatePooledCopyFromSpan (
        MemoryPool<byte> memoryPool,
        int bytesToCopy,
        ReadOnlySpan<byte> source)
    {
        IMemoryOwner<byte> rental = memoryPool.Rent(bytesToCopy);
        try
        {
            UtilsEventSource.Log.MemoryStreamSlimToMemory(Id, bytesToCopy);
            source.CopyTo(rental.Memory.Span[..bytesToCopy]);
        }
        catch
        {
            rental.Dispose();
            throw;
        }

        return new PooledMemoryStreamSlimMemoryOwner(rental, bytesToCopy);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Materializes via <see cref="MemoryStream.TryGetBuffer"/> when the BCL exposes the backing store.
    /// This path works for many fixed buffers even after the stream is disposed (matching
    /// <see cref="ToArray"/> when the buffer stays visible).
    /// </summary>
    /// <param name="memoryPool">
    /// The pool to rent from for non-empty payloads.
    /// </param>
    /// <param name="bufferSegment">
    /// The segment from <see cref="MemoryStream.TryGetBuffer(out ArraySegment{byte})"/>.
    /// </param>
    /// <returns>
    /// The shared empty owner or a pooled copy of the exposed bytes.
    /// </returns>
    private IMemoryOwner<byte> ToMemoryFromExposedBuffer (
        MemoryPool<byte> memoryPool,
        ArraySegment<byte> bufferSegment)
    {
        // Use bufferSegment.Count (not MemoryStream.Length): for MemoryStream, TryGetBuffer's segment
        // length is the logical stream payload—the same range ToArray materializes. After dispose,
        // Length throws ObjectDisposedException while TryGetBuffer can still succeed, so Count is
        // the only safe length source on that path.
        int bytesToCopy = GetFixedModeMaterializationByteCountOrThrow(bufferSegment.Count);
        if (bytesToCopy == 0)
        {
            return EmptyMemoryStreamSlimMemoryOwner.Instance;
        }

        return CreatePooledCopyFromSpan(memoryPool, bytesToCopy, bufferSegment.AsSpan());
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Materializes when the buffer is not exposed: uses <see cref="Stream.Length"/>, seeks to the start,
    /// reads into a rented buffer, then restores <see cref="Stream.Position"/>. Only valid while this wrapper
    /// is not disposed (so BCL length/seek/read behave).
    /// </summary>
    /// <param name="memoryPool">
    /// The pool to rent from for non-empty payloads.
    /// </param>
    /// <returns>
    /// The shared empty owner or a pooled copy of stream bytes from index zero through length.
    /// </returns>
    private IMemoryOwner<byte> ToMemoryByReadingFromStreamStart (MemoryPool<byte> memoryPool)
    {
        int bytesToCopy = GetFixedModeMaterializationByteCountOrThrow(_wrappedStream.Length);
        if (bytesToCopy == 0)
        {
            return EmptyMemoryStreamSlimMemoryOwner.Instance;
        }

        IMemoryOwner<byte> rental = memoryPool.Rent(bytesToCopy);
        try
        {
            UtilsEventSource.Log.MemoryStreamSlimToMemory(Id, bytesToCopy);
            long previousPosition = _wrappedStream.Position;
            _wrappedStream.Seek(0, SeekOrigin.Begin);
            try
            {
                _wrappedStream.ReadExactly(rental.Memory.Span[..bytesToCopy]);
            }
            finally
            {
                _wrappedStream.Position = previousPosition;
            }
        }
        catch
        {
            rental.Dispose();
            throw;
        }

        return new PooledMemoryStreamSlimMemoryOwner(rental, bytesToCopy);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Last resort after disposal when <see cref="TryGetBuffer"/> did not expose data: BCL
    /// <see cref="MemoryStream.ToArray()"/> can still succeed while <see cref="Stream.Length"/> / seek would throw.
    /// </summary>
    /// <remarks>
    ///   This path allocates a heap array via BCL <see cref="MemoryStream.ToArray()"/> and then copies into
    ///   memory rented from <paramref name="memoryPool"/> (see <see cref="CreatePooledCopyFromSpan"/>). It is
    ///   not allocation-free; it exists only for the double-disposal case where safer materialization is
    ///   unavailable. Operators and telemetry consumers should treat this as an exceptional fallback, not the
    ///   steady-state <see cref="MemoryStreamSlim.ToMemory()"/> cost profile.
    /// </remarks>
    /// <param name="memoryPool">
    /// The pool to rent from for non-empty payloads.
    /// </param>
    /// <returns>
    /// The shared empty owner or a pooled copy of the array returned by <see cref="MemoryStream.ToArray"/>.
    /// </returns>
    private IMemoryOwner<byte> ToMemoryUsingBclToArrayFallback (MemoryPool<byte> memoryPool)
    {
        byte[] fallbackArray = _wrappedStream.ToArray();
        int fallbackLength = GetFixedModeMaterializationByteCountOrThrow(fallbackArray.Length);
        if (fallbackLength == 0)
        {
            return EmptyMemoryStreamSlimMemoryOwner.Instance;
        }

        return CreatePooledCopyFromSpan(memoryPool, fallbackLength, fallbackArray.AsSpan(0, fallbackLength));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Materializes the fixed-mode wrapped stream into a contiguous <see cref="IMemoryOwner{T}"/> backed by
    /// <paramref name="memoryPool"/> when non-empty, matching <see cref="ToArray"/> content and disposal
    /// semantics (including BCL <see cref="MemoryStream.ToArray()"/> after wrapper disposal when the
    /// underlying buffer is still accessible).
    /// </summary>
    /// <param name="memoryPool">
    /// The pool to rent from for non-empty streams; never <c>null</c> (validated by the public
    /// <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/> entry point).
    /// </param>
    /// <returns>
    /// The shared empty owner or a pooled copy of the stream bytes from the start through
    /// <see cref="Stream.Length"/>, independent of <see cref="Stream.Position"/>.
    /// </returns>
    protected override IMemoryOwner<byte> ToMemoryInternal (MemoryPool<byte> memoryPool)
    {
        // When TryGetBuffer succeeds, copy directly from the backing array (no ToArray, no seek/read).
        // This remains usable after dispose for publicly visible buffers, same family of cases as BCL GetBuffer.
        if (_wrappedStream.TryGetBuffer(out ArraySegment<byte> bufferSegment) && bufferSegment.Array is not null)
        {
            return ToMemoryFromExposedBuffer(memoryPool, bufferSegment);
        }

        // While the slim wrapper is still alive, Length and Position are defined; read from zero without
        // leaving the caller's position changed.
        if (!IsDisposed)
        {
            return ToMemoryByReadingFromStreamStart(memoryPool);
        }

        // Both wrappers disposed: often Length/Seek throw, but ToArray can still materialize like ToArray().
        return ToMemoryUsingBclToArrayFallback(memoryPool);
    }
    //--------------------------------------------------------------------------------

    #endregion ToMemory fixed-mode paths

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void SetCapacity (long capacityValue)
    {
        if (capacityValue > int.MaxValue)
            ThrowHelper.ThrowInvalidOperationException_IntOverflowCapacity();
        _wrappedStream.Capacity = (int)capacityValue;
    }
    //--------------------------------------------------------------------------------

    #endregion Overrides of MemoryStreamSlim

    #region Overrides of MemoryStream

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override long Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.Length;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override long Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _wrappedStream.Position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _wrappedStream.Position = value;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Read (byte[] buffer, int offset, int count) => _wrappedStream.Read(buffer, offset, count);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Read (Span<byte> destinationBuffer) => _wrappedStream.Read(destinationBuffer);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Task<int> ReadAsync (byte[] buffer, int offset, int count,
        CancellationToken cancellationToken) => _wrappedStream.ReadAsync(buffer, offset, count, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override ValueTask<int> ReadAsync (Memory<byte> buffer, 
        CancellationToken cancellationToken = default) => _wrappedStream.ReadAsync(buffer, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int ReadByte () => _wrappedStream.ReadByte();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void CopyTo (Stream destination, int bufferSize)
    {
        _wrappedStream.CopyTo(destination, bufferSize);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Seek (long offset, SeekOrigin origin) =>
        _wrappedStream.Seek(offset, origin);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void SetLength (long value)
    {
        _wrappedStream.SetLength(value);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override byte[] ToArray ()
    {
        // BCL MemoryStream allows ToArray() after disposal, so we don't check IsOpen here
        byte[] returnArray = _wrappedStream.ToArray();
        if (0 == returnArray.Length) return returnArray;
        // Report the ToArray operation
        UtilsEventSource.Log.MemoryStreamSlimToArray(Id, returnArray.Length, false);
        return returnArray;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write (byte[] buffer, int offset, int count)
    {
        _wrappedStream.Write(buffer, offset, count);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write (ReadOnlySpan<byte> buffer)
    {
        _wrappedStream.Write(buffer);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Task WriteAsync (byte[] buffer, int offset, int count,
        CancellationToken cancellationToken) => 
        _wrappedStream.WriteAsync(buffer, offset, count, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override ValueTask WriteAsync (ReadOnlyMemory<byte> buffer, 
        CancellationToken cancellationToken = default) => _wrappedStream.WriteAsync(buffer, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteByte (byte value) => _wrappedStream.WriteByte(value);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteTo (Stream stream)
    {
        _wrappedStream.WriteTo(stream);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    /// <remarks>
    ///   Repeated calls after the stream is disposed have no further effect, consistent with
    ///   typical <see cref="Stream"/> disposal semantics.
    /// </remarks>
    protected override void Dispose (bool disposing)
    {
        if (IsDisposed)
            return;

        if (!disposing) return;

        UtilsEventSource.Log.MemoryStreamSlimDisposed(Id);
        GC.SuppressFinalize(this);
        IsDisposed = true;
        IsOpen = false;
        _wrappedStream.Dispose();
    }
    //--------------------------------------------------------------------------------

    #endregion Overrides of MemoryStream
}
//################################################################################
