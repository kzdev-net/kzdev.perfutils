

using System.Buffers;
using System.Diagnostics;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// A class that mimics the <see cref="System.IO.FileStream"/> class to provide a
    /// predictable and controlled environment for the benchmarks with async file I/O
    /// </summary>
    internal class FileStreamMock : Stream
    {
        /// <summary>
        /// Represents a scope that will suspend the async delay so that we can get
        /// predictable results for some operations that should not be delayed such
        /// as the verification of the stream contents
        /// </summary>
        public readonly struct SuspendFileStreamAsyncDelay : IDisposable
        {
            private readonly FileStreamMock _stream;

            /// <summary>
            /// Initializes a new instance of the <see cref="SuspendFileStreamAsyncDelay"/> class.
            /// </summary>
            /// <param name="stream">
            /// The stream instance that will have the async delay suspended
            /// </param>
            public SuspendFileStreamAsyncDelay (FileStreamMock stream)
            {
                _stream = stream;
                _stream.AdjustSuspendAsyncDelay(true);
            }
            /// <inheritdoc />
            public void Dispose ()
            {
                _stream.AdjustSuspendAsyncDelay(false);
            }
        }

        /// <summary>
        /// Represents the type of operations that will emulate an async delay
        /// </summary>
        [Flags]
        public enum AsyncDelayType
        {
            /// <summary>
            /// No delay
            /// </summary>
            None = 0,
            /// <summary>
            /// Delay the async read operations
            /// </summary>
            Read = 1,
            /// <summary>
            /// Delay the async write operations
            /// </summary>
            Write = 2,
            /// <summary>
            /// Delay the async copy operations
            /// </summary>
            CopyTo = 4,
            /// <summary>
            /// Delay all async operations
            /// </summary>
            All = Read | Write | CopyTo
        }

        /// <summary>
        /// The number of async calls that have been made to the stream
        /// </summary>
        private int _asyncCallCount;

        /// <summary>
        /// The number of active suspensions of the async delay scopes that exist
        /// </summary>
        private int _asyncSuspendCount;

        /// <summary>
        /// The internal stream that is used to store the data
        /// </summary>
        private readonly MemoryStream _internalStream;

        /// <summary>
        /// The type of async operations that will be delayed
        /// </summary>
        private readonly AsyncDelayType _delayAsyncOperationsType;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper to adjust a suspension of the async delay
        /// </summary>
        /// <param name="increment">
        /// If true, the suspension count will be incremented, otherwise decremented
        /// </param>
        private void AdjustSuspendAsyncDelay (bool increment)
        {
            if (increment)
            {
                Interlocked.Increment(ref _asyncSuspendCount);
                return;
            }

            // Use conditional compilation to check for negative async suspension count
            // instead of Debug.Assert to avoid creating a local variable in release builds
#if DEBUG
            int newCount = Interlocked.Decrement(ref _asyncSuspendCount);
            if (newCount >= 0)
                return;
            Debug.Fail("The async suspension count has gone negative");
#else
            Interlocked.Decrement(ref _asyncSuspendCount);
#endif
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a task to emulate an I/O delay
        /// </summary>
        /// <returns></returns>
        private async Task GetAsyncDelay (AsyncDelayType operationType)
        {
            if ((AsyncDelayType.None == _delayAsyncOperationsType) ||
                (0 == (_delayAsyncOperationsType & operationType)))
            {
                return;
            }
            if (Volatile.Read(ref _asyncSuspendCount) > 0)
            {
                return;
            }

            // For every one in 16 calls, we will delay the async operation
            int callCount = Interlocked.Increment(ref _asyncCallCount);
            if (0 == (15 & callCount))
            {
                await Task.Delay(10).ConfigureAwait(false);
                return;
            }
            // Otherwise, for every fourth call, we simulate a near immediate async operation with a yield
            if (0 == (3 & callCount))
            {
                await Task.Yield();
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper that performs the read operation for the CopyToAsync method
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<int> ReadForCopyToAsync (byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await GetAsyncDelay(AsyncDelayType.CopyTo).ConfigureAwait(false);
            return await _internalStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamMock"/> class.
        /// </summary>
        public FileStreamMock ()
        {
            _internalStream = new MemoryStream();
            _delayAsyncOperationsType = AsyncDelayType.All;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamMock"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of the internal stream
        /// </param>
        public FileStreamMock (int capacity)
        {
            _internalStream = new MemoryStream(capacity);
            _delayAsyncOperationsType = AsyncDelayType.All;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamMock"/> class.
        /// </summary>
        /// <param name="delayAsyncOperationsType">
        /// Specifies the type of async operations that will be delayed
        /// </param>
        public FileStreamMock (AsyncDelayType delayAsyncOperationsType)
        {
            _internalStream = new MemoryStream();
            _delayAsyncOperationsType = delayAsyncOperationsType;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamMock"/> class.
        /// </summary>
        /// <param name="delayAsyncOperationsType">
        /// Specifies the type of async operations that will be delayed
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the internal stream
        /// </param>
        public FileStreamMock (AsyncDelayType delayAsyncOperationsType, int capacity)
        {
            _internalStream = new MemoryStream(capacity);
            _delayAsyncOperationsType = delayAsyncOperationsType;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a disposable scope that will suspend the async delay
        /// </summary>
        /// <returns></returns>
        public SuspendFileStreamAsyncDelay SuspendAsyncDelay () => new(this);
        //--------------------------------------------------------------------------------

        #region Overrides of Stream

        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void Flush ()
        {
            _internalStream.Flush();
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override int Read (byte[] buffer, int offset, int count) =>
            _internalStream.Read(buffer, offset, count);
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override long Seek (long offset, SeekOrigin origin) =>
            _internalStream.Seek(offset, origin);
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void SetLength (long value)
        {
            _internalStream.SetLength(value);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void Write (byte[] buffer, int offset, int count)
        {
            _internalStream.Write(buffer, offset, count);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override bool CanTimeout => _internalStream.CanTimeout;
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override bool CanRead => _internalStream.CanRead;
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override bool CanSeek => _internalStream.CanSeek;
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override bool CanWrite => _internalStream.CanWrite;
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override long Length => _internalStream.Length;
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void CopyTo (Stream destination, int bufferSize)
        {
            _internalStream.CopyTo(destination, bufferSize);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override async Task CopyToAsync (Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                int bytesRead;
                while ((bytesRead = await ReadForCopyToAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
                {
                    await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override async Task<int> ReadAsync (byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await GetAsyncDelay(AsyncDelayType.Read).ConfigureAwait(false);
            return await _internalStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override async ValueTask<int> ReadAsync (Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await GetAsyncDelay(AsyncDelayType.Read).ConfigureAwait(false);
            return await _internalStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override async Task WriteAsync (byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await GetAsyncDelay(AsyncDelayType.Write).ConfigureAwait(false);
            await _internalStream.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override async ValueTask WriteAsync (ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await GetAsyncDelay(AsyncDelayType.Write).ConfigureAwait(false);
            await _internalStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override long Position
        {
            get => _internalStream.Position;
            set => _internalStream.Position = value;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        protected override void Dispose (bool disposing)
        {
            _internalStream.Dispose();
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override ValueTask DisposeAsync ()
        {
            return _internalStream.DisposeAsync();
        }
        //--------------------------------------------------------------------------------

        #endregion
    }
    //################################################################################
}
