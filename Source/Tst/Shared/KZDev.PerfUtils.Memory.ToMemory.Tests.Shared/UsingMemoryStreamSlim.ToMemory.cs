// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Reflection;

using FluentAssertions;

using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for <see cref="MemoryStreamSlim.ToMemory()"/> and
/// <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/>.
/// </summary>
public partial class UsingMemoryStreamSlim
{
    //================================================================================
    /// <summary>
    /// A <see cref="MemoryPool{T}"/> that delegates to <see cref="MemoryPool{T}.Shared"/> while counting rent and return.
    /// </summary>
    private sealed class RentReturnCountingPool : MemoryPool<byte>
    {
        private int _rentCount;
        private int _returnCount;

        /// <summary>
        /// Number of completed <see cref="Rent"/> calls.
        /// </summary>
        public int RentCount => Volatile.Read(ref _rentCount);

        /// <summary>
        /// Number of times a rented owner was disposed (returned).
        /// </summary>
        public int ReturnCount => Volatile.Read(ref _returnCount);

        /// <inheritdoc />
        public override int MaxBufferSize => Shared.MaxBufferSize;

        /// <inheritdoc />
        public override IMemoryOwner<byte> Rent (int minBufferSize = -1)
        {
            Interlocked.Increment(ref _rentCount);
            IMemoryOwner<byte> inner = Shared.Rent(minBufferSize);
            return new CountingDisposeOwner(inner, () => Interlocked.Increment(ref _returnCount));
        }

        /// <inheritdoc />
        protected override void Dispose (bool disposing)
        {
        }

        /// <summary>
        /// Wraps an inner owner and invokes a callback when the inner rental is disposed.
        /// </summary>
        private sealed class CountingDisposeOwner : IMemoryOwner<byte>
        {
            private IMemoryOwner<byte>? _inner;
            private readonly Action _onInnerDisposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="CountingDisposeOwner"/> class.
            /// </summary>
            /// <param name="inner">
            /// The rental to wrap.
            /// </param>
            /// <param name="onInnerDisposed">
            /// Invoked exactly once when <paramref name="inner"/> is disposed.
            /// </param>
            public CountingDisposeOwner (IMemoryOwner<byte> inner, Action onInnerDisposed)
            {
                _inner = inner;
                _onInnerDisposed = onInnerDisposed;
            }

            /// <inheritdoc />
            public Memory<byte> Memory
            {
                get
                {
                    IMemoryOwner<byte>? inner = _inner;
                    return inner?.Memory ?? throw new ObjectDisposedException(nameof(CountingDisposeOwner));
                }
            }

            /// <inheritdoc />
            public void Dispose ()
            {
                IMemoryOwner<byte>? inner = Interlocked.Exchange(ref _inner, null);
                if (inner is null)
                {
                    return;
                }

                inner.Dispose();
                _onInnerDisposed();
            }
        }
    }
    //================================================================================
    /// <summary>
    /// Non-exposable <see cref="MemoryStream"/> that reports a logical <see cref="Stream.Length"/> too large
    /// for contiguous array or pooled materialization, with <see cref="ToArray"/> matching that guard.
    /// </summary>
    private sealed class OversizedNonExposableMemoryStream : MemoryStream
    {
        private readonly long _logicalLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="OversizedNonExposableMemoryStream"/> class.
        /// </summary>
        /// <param name="logicalLength">
        /// The value returned by <see cref="Stream.Length"/>.
        /// </param>
        public OversizedNonExposableMemoryStream (long logicalLength)
            : base([], 0, 0, writable: true, publiclyVisible: false)
        {
            _logicalLength = logicalLength;
        }

        /// <inheritdoc />
        public override long Length => _logicalLength;

        /// <inheritdoc />
        public override byte[] ToArray ()
        {
            if (_logicalLength == 0)
            {
                return [];
            }

            if (_logicalLength > int.MaxValue)
            {
                ThrowHelper.ThrowInvalidOperationException_TooLargeToCopyToArray();
            }

            int bytesToCopy = (int)_logicalLength;
            if (bytesToCopy > Array.MaxLength)
            {
                ThrowHelper.ThrowInvalidOperationException_TooLargeToCopyToArray();
            }

            return base.ToArray();
        }
    }
    //================================================================================
    /// <summary>
    /// Non-exposed <see cref="MemoryStream"/> whose <see cref="MemoryStream.Read(byte[], int, int)"/> and
    /// <see cref="MemoryStream.Read(Span{byte})"/> throw, to validate failure cleanup on the read fallback path.
    /// </summary>
    private sealed class NonExposedReadThrowingMemoryStream : MemoryStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonExposedReadThrowingMemoryStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The backing buffer.
        /// </param>
        /// <param name="index">
        /// The start index in <paramref name="buffer"/>.
        /// </param>
        /// <param name="count">
        /// The length of the stream view.
        /// </param>
        public NonExposedReadThrowingMemoryStream (byte[] buffer, int index, int count)
            : base(buffer, index, count, writable: true, publiclyVisible: false)
        {
        }

        /// <inheritdoc />
        public override int Read (byte[] buffer, int offset, int count) =>
            throw new IOException("Simulated read failure for ToMemory fallback cleanup test.");

        /// <inheritdoc />
        public override int Read (Span<byte> buffer) =>
            throw new IOException("Simulated read failure for ToMemory fallback cleanup test.");
    }
    //================================================================================

    /// <summary>
    /// Cached reflection handle for <see cref="MemoryStreamSlim"/>'s <c>LengthInternal</c> property (test-only).
    /// </summary>
    private static readonly PropertyInfo StreamLengthInternalProperty = GetStreamLengthInternalProperty();

    /// <summary>
    /// Resolves <see cref="MemoryStreamSlim"/>'s <c>LengthInternal</c> property for dynamic stream materialization tests.
    /// </summary>
    /// <returns>
    /// The non-public instance property.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The property could not be resolved (API rename).
    /// </exception>
    private static PropertyInfo GetStreamLengthInternalProperty () =>
        typeof(MemoryStreamSlim).GetProperty(
            "LengthInternal",
            BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException(
            "Could not resolve MemoryStreamSlim.LengthInternal for test setup.");

    /// <summary>
    /// Sets <see cref="MemoryStreamSlim"/>'s internal length without growing backing storage, to exercise
    /// contiguous materialization guards without allocating multi-gigabyte buffers.
    /// </summary>
    /// <param name="stream">
    /// The stream under test.
    /// </param>
    /// <param name="lengthInternal">
    /// The value to assign to <c>LengthInternal</c>.
    /// </param>
    private static void SetStreamLengthInternalForTest (MemoryStreamSlim stream, long lengthInternal)
    {
        StreamLengthInternalProperty.SetValue(stream, lengthInternal);
    }

    /// <summary>
    /// Creates a <see cref="MemoryStream"/> view over <paramref name="payload"/> with
    /// <c>publiclyVisible: false</c> so <see cref="MemoryStream.TryGetBuffer(out ArraySegment{byte})"/> returns
    /// <see langword="false"/> and <see cref="MemoryStreamWrapper"/> materializes via the seek/read fallback.
    /// </summary>
    /// <param name="payload">
    /// The bytes visible through the stream (from index zero through <c>payload.Length</c>).
    /// </param>
    /// <returns>
    /// The configured stream.
    /// </returns>
    private static MemoryStream CreateNonExposedBufferMemoryStream (byte[] payload)
    {
        MemoryStream stream = new(payload, 0, payload.Length, writable: true, publiclyVisible: false);
        stream.TryGetBuffer(out _).Should().BeFalse();
        return stream;
    }

    /// <summary>
    /// Asserts <see cref="MemoryStreamSlim.ToMemory()"/> and <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/>
    /// throw the same exception type as <see cref="MemoryStreamSlim.ToArray"/> for the current stream state.
    /// </summary>
    /// <param name="stream">
    /// The stream under test.
    /// </param>
    private static void AssertToMemoryThrowsSameExceptionTypeAsToArray (MemoryStreamSlim stream)
    {
        Type expectedType = stream.Invoking(s => s.ToArray())
            .Should()
            .Throw<Exception>()
            .Which
            .GetType();

        stream.Invoking(s =>
            {
                using (s.ToMemory())
                {
                }
            })
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType);

        stream.Invoking(s =>
            {
                using (s.ToMemory(MemoryPool<byte>.Shared))
                {
                }
            })
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType);
    }

    //================================================================================

    #region ToMemory - pool and empty-stream behavior

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that two empty dynamic streams return the same shared <see cref="IMemoryOwner{T}"/>
    /// instance from <see cref="MemoryStreamSlim.ToMemory()"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_EmptyDynamic_ToMemory_ReturnsSameSingletonInstance ()
    {
        using MemoryStreamSlim streamA = MemoryStreamSlim.Create();
        using MemoryStreamSlim streamB = MemoryStreamSlim.Create();

        using IMemoryOwner<byte> ownerA = streamA.ToMemory();
        using IMemoryOwner<byte> ownerB = streamB.ToMemory();

        ReferenceEquals(ownerA, ownerB).Should().BeTrue();
        ownerA.Memory.Length.Should().Be(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that two empty fixed-mode streams return the same shared <see cref="IMemoryOwner{T}"/>
    /// instance from <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_EmptyFixed_ToMemoryWithPool_ReturnsSameSingletonInstance ()
    {
        byte[] bufferA = [];
        byte[] bufferB = [];
        using MemoryStreamSlim streamA = MemoryStreamSlim.Create(bufferA);
        using MemoryStreamSlim streamB = MemoryStreamSlim.Create(bufferB);

        RentReturnCountingPool pool = new();
        using IMemoryOwner<byte> ownerA = streamA.ToMemory(pool);
        using IMemoryOwner<byte> ownerB = streamB.ToMemory(pool);

        pool.RentCount.Should().Be(0);
        ReferenceEquals(ownerA, ownerB).Should().BeTrue();
        ownerA.Memory.Length.Should().Be(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that an empty stream does not invoke <see cref="MemoryPool{T}.Rent"/> on the supplied pool.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_EmptyDynamic_ToMemoryWithRecordingPool_DoesNotRent ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        RentReturnCountingPool pool = new();

        using (stream.ToMemory(pool))
        {
        }

        pool.RentCount.Should().Be(0);
        pool.ReturnCount.Should().Be(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/> throws when the pool is null.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_ToMemory_NullPool_ThrowsArgumentNullException ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        stream.WriteByte(1);

        stream.Invoking(s => s.ToMemory(null!))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("memoryPool");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that disposing a non-empty owner returns storage to the originating pool (observed once).
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_NonEmpty_ToMemory_Dispose_ReturnsToPoolOnce ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] payload = new byte[64];
        GetRandomBytes(payload, payload.Length);
        stream.Write(payload, 0, payload.Length);

        RentReturnCountingPool pool = new();
        IMemoryOwner<byte> owner = stream.ToMemory(pool);
        pool.RentCount.Should().Be(1);
        pool.ReturnCount.Should().Be(0);

        owner.Dispose();
        pool.ReturnCount.Should().Be(1);

        owner.Dispose();
        pool.ReturnCount.Should().Be(1);
    }
    //--------------------------------------------------------------------------------

    #endregion ToMemory - pool and empty-stream behavior

    #region ToMemory - parity and content shape

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that materialized memory matches <see cref="MemoryStreamSlim.ToArray()"/> for dynamic and fixed streams.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_ToMemory_MatchesToArray_ForDynamicAndFixed ()
    {
        byte[] payload = new byte[256];
        GetRandomBytes(payload, payload.Length);

        using (MemoryStreamSlim dynamicStream = MemoryStreamSlim.Create())
        {
            dynamicStream.Write(payload, 0, payload.Length);
            AssertToMemoryMatchesToArray(dynamicStream, payload);
        }

        byte[] fixedBuffer = new byte[payload.Length];
        Array.Copy(payload, fixedBuffer, payload.Length);
        using MemoryStreamSlim fixedStream = MemoryStreamSlim.Create(fixedBuffer);
        AssertToMemoryMatchesToArray(fixedStream, payload);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests <see cref="MemoryStreamSlim.ToMemory()"/> versus
    /// <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/> with <see cref="MemoryPool{T}.Shared"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_ToMemory_ParameterlessAndSharedPool_Match ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] payload = new byte[128];
        GetRandomBytes(payload, payload.Length);
        stream.Write(payload, 0, payload.Length);

        using IMemoryOwner<byte> ownerDefault = stream.ToMemory();
        using IMemoryOwner<byte> ownerShared = stream.ToMemory(MemoryPool<byte>.Shared);

        ownerDefault.Memory.ToArray().Should().BeEquivalentTo(payload);
        ownerShared.Memory.ToArray().Should().BeEquivalentTo(payload);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that <see cref="IMemoryOwner{T}.Memory"/> length follows stream length, not pooled buffer slack.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_ToMemory_MemoryLength_EqualsStreamLength ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        stream.WriteByte(42);

        using IMemoryOwner<byte> owner = stream.ToMemory();
        owner.Memory.Length.Should().Be(1);
        stream.Length.Should().Be(1);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests that <see cref="MemoryStreamSlim.ToMemory()"/> captures logical content from the start through
    /// <see cref="Stream.Length"/> even when <see cref="Stream.Position"/> is at the end.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_PositionAtEnd_ToMemory_MatchesToArray ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] payload = new byte[96];
        GetRandomBytes(payload, payload.Length);
        stream.Write(payload, 0, payload.Length);
        stream.Position = stream.Length;

        byte[] fromArray = stream.ToArray();
        using IMemoryOwner<byte> owner = stream.ToMemory();
        owner.Memory.ToArray().Should().BeEquivalentTo(fromArray);
        fromArray.Should().BeEquivalentTo(payload);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Tests multi-segment dynamic content: <see cref="MemoryStreamSlim.ToMemory()"/> matches
    /// <see cref="MemoryStreamSlim.ToArray()"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_MultiSegmentDynamic_ToMemory_MatchesToArray ()
    {
        int byteCount = MemorySegmentedBufferGroup.StandardBufferSegmentSize * 3 + 17;
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        byte[] payload = new byte[byteCount];
        GetRandomBytes(payload, payload.Length);

        int offset = 0;
        while (offset < byteCount)
        {
            int chunk = Math.Min(MemorySegmentedBufferGroup.StandardBufferSegmentSize / 2, byteCount - offset);
            stream.Write(payload, offset, chunk);
            offset += chunk;
        }

        byte[] fromArray = stream.ToArray();
        using IMemoryOwner<byte> owner = stream.ToMemory();
        fromArray.Should().BeEquivalentTo(payload);
        owner.Memory.ToArray().Should().BeEquivalentTo(fromArray);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// When dynamic storage reports a length greater than <see cref="int.MaxValue"/>,
    /// <see cref="MemoryStreamSlim.ToMemory()"/> and <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/>
    /// throw the same exception type as <see cref="MemoryStreamSlim.ToArray"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_LengthAboveIntMax_Dynamic_ToMemory_ThrowsSameExceptionTypeAsToArray ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        SetStreamLengthInternalForTest(stream, (long)int.MaxValue + 1);
        AssertToMemoryThrowsSameExceptionTypeAsToArray(stream);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// When dynamic storage reports a length greater than <see cref="Array.MaxLength"/> but within
    /// <see cref="int.MaxValue"/>, <see cref="MemoryStreamSlim.ToMemory()"/> and
    /// <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/> throw the same exception type as
    /// <see cref="MemoryStreamSlim.ToArray"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_LengthAboveArrayMaxLength_Dynamic_ToMemory_ThrowsSameExceptionTypeAsToArray ()
    {
        using MemoryStreamSlim stream = MemoryStreamSlim.Create();
        SetStreamLengthInternalForTest(stream, Array.MaxLength + 1L);
        AssertToMemoryThrowsSameExceptionTypeAsToArray(stream);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// When fixed-mode wrapped storage reports a length greater than <see cref="int.MaxValue"/>,
    /// <see cref="MemoryStreamSlim.ToMemory()"/> and <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/>
    /// throw the same exception type as <see cref="MemoryStreamSlim.ToArray"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_LengthAboveIntMax_FixedWrapper_ToMemory_ThrowsSameExceptionTypeAsToArray ()
    {
        OversizedNonExposableMemoryStream inner = new((long)int.MaxValue + 1);
        using MemoryStreamSlim stream = new MemoryStreamWrapper(inner);
        AssertToMemoryThrowsSameExceptionTypeAsToArray(stream);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// When fixed-mode wrapped storage reports a length greater than <see cref="Array.MaxLength"/> but within
    /// <see cref="int.MaxValue"/>, <see cref="MemoryStreamSlim.ToMemory()"/> and
    /// <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/> throw the same exception type as
    /// <see cref="MemoryStreamSlim.ToArray"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_LengthAboveArrayMaxLength_FixedWrapper_ToMemory_ThrowsSameExceptionTypeAsToArray ()
    {
        OversizedNonExposableMemoryStream inner = new(Array.MaxLength + 1L);
        using MemoryStreamSlim stream = new MemoryStreamWrapper(inner);
        AssertToMemoryThrowsSameExceptionTypeAsToArray(stream);
    }
    //--------------------------------------------------------------------------------

    #endregion ToMemory - parity and content shape

    #region ToMemory - fixed wrapper read fallback (non-exposed buffer)

    //================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// When the wrapped BCL stream does not expose its buffer, <see cref="MemoryStreamSlim.ToMemory()"/> and
    /// <see cref="MemoryStreamSlim.ToMemory(MemoryPool{byte})"/> copy the full logical content matching
    /// <see cref="MemoryStreamSlim.ToArray"/>.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_NonExposedFixedWrapper_ToMemory_MatchesToArray ()
    {
        byte[] payload = new byte[96];
        GetRandomBytes(payload, payload.Length);
        MemoryStream inner = CreateNonExposedBufferMemoryStream(payload);
        using MemoryStreamSlim stream = new MemoryStreamWrapper(inner);

        AssertToMemoryMatchesToArray(stream, payload);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// The seek/read fallback restores <see cref="Stream.Position"/> after materializing when the buffer is not exposed.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_NonExposedFixedWrapper_PrePositioned_ToMemory_RestoresPosition ()
    {
        byte[] payload = new byte[80];
        GetRandomBytes(payload, payload.Length);
        MemoryStream inner = CreateNonExposedBufferMemoryStream(payload);
        long positionBefore = 37;
        inner.Position = positionBefore;

        using MemoryStreamSlim stream = new MemoryStreamWrapper(inner);
        byte[] fromArray = stream.ToArray();
        using (IMemoryOwner<byte> owner = stream.ToMemory())
        {
            owner.Memory.ToArray().Should().BeEquivalentTo(fromArray);
        }

        using (RentReturnCountingPool pool = new())
        {
            using IMemoryOwner<byte> owner = stream.ToMemory(pool);
            owner.Memory.ToArray().Should().BeEquivalentTo(fromArray);
            pool.RentCount.Should().BeGreaterThan(0);
        }

        stream.Position.Should().Be(positionBefore);
        inner.Position.Should().Be(positionBefore);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Zero-length non-exposed fixed wrapper streams yield an empty owner without renting from a supplied pool.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_NonExposedFixedWrapper_Empty_ToMemory_DoesNotRentFromPool ()
    {
        MemoryStream inner = CreateNonExposedBufferMemoryStream([]);
        using MemoryStreamSlim stream = new MemoryStreamWrapper(inner);
        RentReturnCountingPool pool = new();

        using (stream.ToMemory(pool))
        {
        }

        pool.RentCount.Should().Be(0);
        pool.ReturnCount.Should().Be(0);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// When the read fallback throws, the rented buffer is returned to the pool and the stream position is restored.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_NonExposedFixedWrapper_ReadFails_ToMemory_DisposesRentalAndRestoresPosition ()
    {
        byte[] payload = new byte[16];
        GetRandomBytes(payload, payload.Length);
        NonExposedReadThrowingMemoryStream inner = new(payload, 0, payload.Length);
        inner.TryGetBuffer(out _).Should().BeFalse();
        long positionBefore = 11;
        inner.Position = positionBefore;

        using MemoryStreamSlim stream = new MemoryStreamWrapper(inner);
        RentReturnCountingPool pool = new();

        stream.Invoking(s => s.ToMemory(pool))
            .Should()
            .Throw<IOException>();

        pool.RentCount.Should().Be(1);
        pool.ReturnCount.Should().Be(1);
        stream.Position.Should().Be(positionBefore);
        inner.Position.Should().Be(positionBefore);
    }
    //--------------------------------------------------------------------------------

    #endregion ToMemory - fixed wrapper read fallback (non-exposed buffer)

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asserts <see cref="MemoryStreamSlim.ToMemory()"/> and pooled overload match <see cref="MemoryStreamSlim.ToArray()"/>.
    /// </summary>
    /// <param name="stream">
    /// The non-disposed stream under test.
    /// </param>
    /// <param name="expectedPayload">
    /// Expected bytes from the start through <see cref="Stream.Length"/>.
    /// </param>
    private static void AssertToMemoryMatchesToArray (MemoryStreamSlim stream, byte[] expectedPayload)
    {
        byte[] fromArray = stream.ToArray();
        fromArray.Should().BeEquivalentTo(expectedPayload);

        using IMemoryOwner<byte> ownerDefault = stream.ToMemory();
        ownerDefault.Memory.ToArray().Should().BeEquivalentTo(fromArray);

        RentReturnCountingPool pool = new();
        using IMemoryOwner<byte> ownerPooled = stream.ToMemory(pool);
        ownerPooled.Memory.ToArray().Should().BeEquivalentTo(fromArray);
        if (fromArray.Length > 0)
        {
            pool.RentCount.Should().BeGreaterThan(0);
        }
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
