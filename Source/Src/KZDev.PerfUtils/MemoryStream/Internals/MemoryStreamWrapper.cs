// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Observability;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// A memory stream slim implementation that simply wraps a <see cref="MemoryStream"/>
    /// for cases where the stream is created with a buffer that is already allocated.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
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
        private string DebugDisplayValue => $@"Length = {Length}, Position = {Position}, Mode = {nameof(MemoryStreamSlimMode.Fixed)}";
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
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SetCapacity (int capacityValue)
        {
            _wrappedStream.Capacity = capacityValue;
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
        public override int Read (byte[] buffer, int offset, int count)
        {
            return _wrappedStream.Read(buffer, offset, count);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read (Span<byte> destinationBuffer)
        {
            return _wrappedStream.Read(destinationBuffer);
        }
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
            byte[] returnArray = _wrappedStream.ToArray();
            if (0 == returnArray.Length) return returnArray;
            // Report the ToArray operation
            UtilsEventSource.Log.MemoryStreamSlimToArray(Id, returnArray.Length);
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
        /// <exception cref="ObjectDisposedException">
        /// The stream has been disposed.
        /// </exception>
        protected override void Dispose (bool disposing)
        {
            if (IsDisposed)
            {
                if (!disposing) return; // Somehow we are being called from the finalizer after we've been disposed
                throw new ObjectDisposedException(nameof(MemoryStreamSlim));
            }

            if (!disposing) return;

            UtilsEventSource.Log.MemoryStreamSlimDisposed(Id);
            GC.SuppressFinalize(this);
            _wrappedStream.Dispose();
        }
        //--------------------------------------------------------------------------------

        #endregion Overrides of MemoryStream
    }
    //################################################################################
}
