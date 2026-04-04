// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// Shared singleton <see cref="IMemoryOwner{T}"/> for a zero-length stream payload. <see cref="Dispose"/>
/// is a no-op and is safe to call multiple times.
/// </summary>
[DebuggerDisplay("EmptyMemoryStreamSlimMemoryOwner (Length = 0)")]
internal sealed class EmptyMemoryStreamSlimMemoryOwner : IMemoryOwner<byte>
{
    /// <summary>
    /// The single shared instance; callers must not return this to any pool.
    /// </summary>
    internal static EmptyMemoryStreamSlimMemoryOwner Instance { get; } = new EmptyMemoryStreamSlimMemoryOwner();

    /// <summary>
    /// Prevents external construction.
    /// </summary>
    private EmptyMemoryStreamSlimMemoryOwner ()
    {
    }

    //--------------------------------------------------------------------------------
    #region IMemoryOwner<byte> Implementation

    /// <summary>
    /// Gets a zero-length view with no pool slack.
    /// </summary>
    public Memory<byte> Memory => Memory<byte>.Empty;

    /// <summary>
    /// No-op; safe to call multiple times.
    /// </summary>
    public void Dispose ()
    {
    }

    #endregion
    //--------------------------------------------------------------------------------
}
//################################################################################

//################################################################################
/// <summary>
/// <see cref="IMemoryOwner{T}"/> over a <see cref="MemoryPool{T}.Rent"/> result. <see cref="Memory"/>
/// exposes exactly <see cref="_length"/> bytes (a prefix of the rented buffer; no trailing slack visible).
/// <see cref="Dispose"/> forwards to the inner owner so the buffer is returned to the originating pool;
/// subsequent dispose is ignored.
/// </summary>
/// <remarks>
///   This type is not thread-safe. Callers should use it from a single thread of execution, which matches the
///   typical pattern after <see cref="MemoryStreamSlim.ToMemory()"/> (for example, a <c>using</c> on one thread).
///   Concurrent use of <see cref="Memory"/> or <see cref="Dispose"/> is unsupported and may fail unpredictably.
/// </remarks>
[DebuggerDisplay("PooledMemoryStreamSlimMemoryOwner (Length = {_length})")]
internal sealed class PooledMemoryStreamSlimMemoryOwner : IMemoryOwner<byte>
{
    /// <summary>
    /// Rented backing store from <see cref="MemoryPool{T}.Rent"/>; cleared when disposed.
    /// </summary>
    private IMemoryOwner<byte>? _inner;

    /// <summary>
    /// Valid logical length (prefix of the inner buffer).
    /// </summary>
    private readonly int _length;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws <see cref="ObjectDisposedException"/> for <see cref="Memory"/> after dispose.
    /// </summary>
    [DoesNotReturn]
    private static void ThrowObjectDisposed ()
    {
        throw new ObjectDisposedException(nameof(PooledMemoryStreamSlimMemoryOwner));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance with the specified rental from <see cref="MemoryPool{T}.Rent"/> and visible length.
    /// </summary>
    /// <param name="inner">
    /// Non-null owner from <see cref="MemoryPool{T}.Rent"/>; at least <paramref name="length"/> bytes must be valid for <see cref="Memory"/>.
    /// </param>
    /// <param name="length">
    /// The number of valid bytes at the start of the rented buffer.
    /// </param>
    public PooledMemoryStreamSlimMemoryOwner (IMemoryOwner<byte> inner, int length)
    {
        Debug.Assert(inner is not null);
        Debug.Assert((uint)length <= (uint)inner.Memory.Length);
        _inner = inner;
        _length = length;
    }

    //--------------------------------------------------------------------------------
    #region IMemoryOwner<byte> Implementation

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> view over the first <see cref="_length"/> bytes of the rented buffer.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The owner has been disposed.
    /// </exception>
    public Memory<byte> Memory
    {
        get
        {
            IMemoryOwner<byte>? inner = _inner;
            if (inner is null)
            {
                ThrowObjectDisposed();
            }

            return inner.Memory[.._length];
        }
    }

    /// <summary>
    /// Disposes the inner rental (returning storage to the pool) if not already disposed.
    /// </summary>
    public void Dispose ()
    {
        IMemoryOwner<byte>? inner = _inner;
        if (inner is null)
        {
            return;
        }

        _inner = null;
        inner.Dispose();
    }

    #endregion
    //--------------------------------------------------------------------------------
}
//################################################################################
