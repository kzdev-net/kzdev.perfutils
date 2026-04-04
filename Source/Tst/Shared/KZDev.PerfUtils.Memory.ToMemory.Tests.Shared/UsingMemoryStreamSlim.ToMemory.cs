// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Buffers;

using FluentAssertions;

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
        public override int MaxBufferSize => MemoryPool<byte>.Shared.MaxBufferSize;

        /// <inheritdoc />
        public override IMemoryOwner<byte> Rent (int minBufferSize = -1)
        {
            Interlocked.Increment(ref _rentCount);
            IMemoryOwner<byte> inner = MemoryPool<byte>.Shared.Rent(minBufferSize);
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
                    return inner?.Memory ?? throw new ObjectDisposedException (nameof(CountingDisposeOwner));
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

    #endregion ToMemory - parity and content shape

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
