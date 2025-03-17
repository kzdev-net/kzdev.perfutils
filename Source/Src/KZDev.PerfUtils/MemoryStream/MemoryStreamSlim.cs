// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;

using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Internals;
using KZDev.PerfUtils.Observability;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// <para>
/// MemoryStreamSlim is a memory stream that is designed to be more memory efficient,
/// and often times faster than the standard MemoryStream class. For relatively small
/// size streams, this utilizes a specialized pool of small buffers for the backing
/// memory, which can reduce the number of allocations and the amount of memory used; 
/// and for larger streams, it uses a list of re-usable buffers to store the contents 
/// of the stream that are allocated by an internally managed buffer pool that is 
/// designed to minimize GC memory traffic pressure, by allocating directly from the
/// large object heap, and re-using buffers as much as possible.
/// </para>
/// <para>
/// This is only designed to replace <see cref="MemoryStream"/> use cases where the 
/// stream does not wrap an existing specific byte array. If an existing byte array buffer
/// is provided on creation, that array will be used as the backing store, and the 
/// MemoryStreamSlim will simply wrap a standard MemoryStream to provide the same 
/// functionality as the standard MemoryStream.
/// </para>
/// </summary>
/// <remarks>
/// Since this operates on in-memory buffers, the asynchronous methods are not truly
/// asynchronous, but they are provided for compatibility with the <see cref="Stream"/> class.
/// The exception to this is the <see cref="MemoryStream.CopyToAsync(Stream)"/> method,
/// which does in fact copy the contents of the stream asynchronously assuming the 
/// destination stream is a true asynchronous stream.
/// </remarks>
public abstract class MemoryStreamSlim : MemoryStream
{
    /// <summary>
    /// The maximum size of an individual standard buffer we will use.
    /// </summary>
    /// <exclude />
    protected const int StandardBufferSegmentSize = MemorySegmentedBufferGroup.StandardBufferSegmentSize;

    /// <summary>
    /// The size of the buffer used for asynchronous copying. This is just actually the size we will 
    /// use for each partial async write operation as we move through the list of internal buffers.
    /// </summary>
    protected const int StandardAsyncCopyBufferSize =
        (MemorySegmentedBufferGroup.StandardBufferSegmentSize * MemorySegmentedGroupGenerationArray.MaxAllowedGroupSegmentCount) >> 4;

    /// <summary>
    /// The maximum length allowed for a <see cref="MemoryStreamSlim"/> instance.
    /// </summary>
    /// <exclude />
    protected internal static readonly long MaxMemoryStreamLength = 
        Environment.Is64BitProcess ? Math.Max(Math.Min(0x8_0000_0000, GC.GetGCMemoryInfo().TotalAvailableMemoryBytes), int.MaxValue) : int.MaxValue;

    /// <summary>
    /// The absolute maximum capacity we will allow a stream instance to get to.
    /// </summary>
    /// <exclude />
    protected internal static readonly long AbsoluteMaxCapacity = MaxMemoryStreamLength;

    /// <summary>
    /// The maximum capacity that can be set for any instance of MemoryStreamSlim.
    /// </summary>
    private static long _globalMaximumCapacity = AbsoluteMaxCapacity;

    /// <summary>
    /// Indicates whether to use native memory buffers for large memory segments.
    /// </summary>
    private static volatile bool _useNativeMemoryBuffers;

    /// <summary>
    /// The number of active MemoryStreamSlim instances.
    /// </summary>
    private static int _activeStreamCount;

    /// <summary>
    /// Indicates whether the global settings for all instances of MemoryStreamSlim have been locked.
    /// That is, this indicates whether the global settings can no longer be updated because an
    /// instance of MemoryStreamSlim has already been created.
    /// </summary>
    /// <remarks>
    /// The /* volatile */ comment is here to remind us that all accesses to this field
    /// should be done using the Volatile class, but we want to be explicit about it in the
    /// code, so we don't actually use the volatile keyword here.
    /// </remarks>
    private static /* volatile */ bool _globalSettingsLocked;

    /// <summary>
    /// Internal helper to get the value of the global setting for whether to use native 
    /// memory buffers for large memory, this also shortcuts/overrides the check for the global settings
    /// by checking if this is running in a browser environment.
    /// </summary>
    protected static bool InternalUseNativeLargeMemoryBuffers => !OperatingSystem.IsBrowser() && _useNativeMemoryBuffers;

    /// <summary>
    /// Gets the default settings for <see cref="MemoryStreamSlim"/> instances.
    /// </summary>
    /// <remarks>
    /// These are the settings that will be applied to newly created <see cref="MemoryStreamSlim"/>
    /// instances if no other settings are provided. Specific settings can be changed on a per-instance
    /// basis by using the options parameter when creating a new instance.
    /// </remarks>
    public static MemoryStreamSlimSettings GlobalDefaultSettings { get; private set; } = new();

    /// <summary>
    /// Gets or sets the global setting for whether to use native memory buffers for large memory 
    /// segments.
    /// </summary>
    /// <remarks>
    /// This value can only be set before any instances of MemoryStreamSlim are created, and once set,
    /// it cannot be changed. If this value is set to <c>true</c>, then all large memory segments will
    /// be allocated using native memory buffers, which can be more efficient in many scenarios.
    /// If this value is set to <c>false</c>, then all memory segments will be allocated using managed
    /// heap memory, but allocated in a way that minimizes GC pressure.
    /// </remarks>
    [UnsupportedOSPlatform("browser")]
    public static bool UseNativeLargeMemoryBuffers
    {
        get => _useNativeMemoryBuffers;
        set
        {
            if (value == _useNativeMemoryBuffers)
                return;
            CheckGlobalSettings();
            _useNativeMemoryBuffers = value;
        }
    }

    /// <summary>
    /// Gets or sets the global maximum capacity for all instances of MemoryStreamSlim. This value affects newly
    /// created instances of MemoryStreamSlim only, and any existing instances will use the value
    /// that was set at the time they were created.
    /// </summary>
    /// <remarks>
    /// Generally, this never needs to be set, as the default value is <see cref="int"/>.<see cref="int.MaxValue"/>,
    /// but if for some reason there is a benefit to keep any single instance from growing
    /// beyond a certain size, this can be set to enforce that limit.
    /// </remarks>
    public static long GlobalMaximumCapacity
    {
        get
        {
            // Lock needed to ensure that the value is read atomically (it is a long)
            lock (SettingsLock)
            {
                return _globalMaximumCapacity;
            }
        }
        set
        {
            if (value < 1 || value > AbsoluteMaxCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(value), 1, AbsoluteMaxCapacity);
            // Lock needed to ensure that the value is set atomically (it is a long)
            lock (SettingsLock)
            {
                _globalMaximumCapacity = value;
            }
        }
    }

    /// <summary>
    /// Requests that all large memory buffers managed by MemoryStreamSlim instances should be released 
    /// back to the system as soon as possible.
    /// </summary>
    /// <remarks>
    /// This is a hint to the system that the caller requests that large memory segments be released, but it 
    /// is not guaranteed that the memory will be released immediately. As soon as all 
    /// currently in-use MemoryStreamSlim instances are disposed or finalized, the currently 
    /// allocated memory buffers will be released.
    /// </remarks>
    public static void ReleaseMemoryBuffers ()
    {
        SegmentMemoryStreamSlim.ReleaseMemoryBufferPool();
    }

    /// <summary>
    /// The internal capacity of this stream. This is the reported capacity, not the actual
    /// allocated capacity, which can be larger. This is also the capacity that can be
    /// explicitly set by the user. So, we will always have at least this much space.
    /// </summary>
    private long _internalCapacity;

    /// <summary>
    /// The settings for this stream instance.
    /// </summary>
    public MemoryStreamSlimSettings Settings { [DebuggerStepThrough] get; }

    /// <summary>
    /// Gets the operating mode of this stream instance.
    /// </summary>
    /// <remarks>
    /// This is a readonly property that represents how the stream was instantiated. If 
    /// the stream was created with a specific existing buffer to provide a <see cref="Stream"/>
    /// semantic API around that fixed buffer, then this will return 
    /// <see cref="MemoryStreamSlimMode.Fixed"/>, otherwise, it will return
    /// <see cref="MemoryStreamSlimMode.Dynamic"/>.
    /// </remarks>
    public MemoryStreamSlimMode Mode { [DebuggerStepThrough] get; }

    /// <summary>
    /// The maximum capacity that can be set for this instance of MemoryStreamSlim.
    /// </summary>
#if NOT_PACKAGING
    internal
#else
        protected
#endif
        long MaximumCapacity
    { [DebuggerStepThrough] get; }

    /// <summary>
    /// True when this instance has been disposed
    /// </summary>
    /// <exclude />
    protected bool IsDisposed { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

    /// <summary>
    /// <c>true</c> when the stream is open.
    /// </summary>
    /// <exclude />
    protected bool IsOpen { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

    /// <summary>
    /// <c>true</c> when the stream can be written to.
    /// </summary>
    /// <exclude />
    /// <exclude />
    protected bool CanWriteInternal { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

    /// <summary>
    /// The current length of this stream.
    /// </summary>
    /// <exclude />
    protected long LengthInternal { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = 0;

    /// <summary>
    /// The current file position in this stream.
    /// </summary>
    /// <exclude />
    protected long PositionInternal { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = 0;

    /// <summary>
    /// The current capacity of this stream. This is the reported capacity, not the actual
    /// allocated capacity, which can be larger. This is also the capacity that can be
    /// explicitly set by the user. So, we will always have at least this much space.
    /// </summary>
    /// <exclude />
    protected long CapacityInternal
    {
        [DebuggerStepThrough]
        get => _internalCapacity;
        [DebuggerStepThrough]
        set
        {
            if (value == _internalCapacity)
                return;
            if (value < _internalCapacity)
            {
                UtilsEventSource.Log.MemoryStreamSlimCapacityReduced(Id, _internalCapacity, value);
            }
            else
            {
                UtilsEventSource.Log.MemoryStreamSlimCapacityExpanded(Id, _internalCapacity, value);
            }
            _internalCapacity = value;
        }
    }

    /// <summary>
    /// The object used to lock the global settings for all instances of MemoryStreamSlim. We also
    /// use this to protect access to the MaximumCapacity value.
    /// </summary>
#if NET9_0_OR_GREATER
    private static readonly Lock SettingsLock = new();
#else
        private static readonly object SettingsLock = new();
#endif
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to track the number of active MemoryStreamSlim instances.
    /// </summary>
    /// <param name="increment">
    /// If <c>true</c>, the active stream count is incremented; otherwise, it is decremented.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void AdjustActiveStreamCount (bool increment)
    {
        if (increment)
            Interlocked.Increment(ref _activeStreamCount);
        else
            Interlocked.Decrement(ref _activeStreamCount);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the state of the global settings for all instances of MemoryStreamSlim to locked.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LockGlobalSettings ()
    {
        Volatile.Write(ref _globalSettingsLocked, true);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Checks whether the global settings for all instances of MemoryStreamSlim can still be updated
    ///  and throws an exception if they cannot.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the global settings are locked and can no longer be updated.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckGlobalSettings ()
    {
        if (Volatile.Read(ref _globalSettingsLocked))
            ThrowHelper.ThrowInvalidOperation_GlobalSettingsCantBeUpdated(nameof(MemoryStreamSlim));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Validates arguments provided to reading and writing methods on <see cref="MemoryStreamSlim"/>.
    /// </summary>
    /// <param name="buffer">
    /// The array "buffer" argument passed to the reading or writing method.
    /// </param>
    /// <param name="offset">
    /// The integer "offset" argument passed to the reading or writing method.
    /// </param>
    /// <param name="count">
    /// The integer "count" argument passed to the reading or writing method.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="buffer"/> was null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> was outside the bounds of <paramref name="buffer"/>, or
    /// <paramref name="count"/> was negative, or the range specified by the combination of
    /// <paramref name="offset"/> and <paramref name="count"/> exceed the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exclude />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected new static void ValidateBufferArguments (byte[] buffer, int offset, int count)
    {
        // Do the standard stream validations
        Stream.ValidateBufferArguments(buffer, offset, count);
        // Then other validations not done by the base class
        if (count < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));
    }

    /// <summary>
    /// Returns <c>true</c> if the specified length is valid for a <see cref="MemoryStreamSlim"/> 
    /// instance, or <c>false</c> if it is not.
    /// </summary>
    /// <param name="length">
    /// The length to validate.
    /// </param>
    /// <returns>
    /// <c>true</c> if the length is valid; otherwise, <c>false</c>.
    /// </returns>
    /// <exclude />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void ValidateLength (long length)
    {
        if ((length >= 0) && (length <= MaxMemoryStreamLength))
            return;
        ThrowHelper.ThrowIOException_StreamTooLong();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Static constructor for the <see cref="MemoryStreamSlim"/> class.
    /// </summary>
    static MemoryStreamSlim ()
    {
        // Create the counter meter for the active stream count
        MemoryMeter.Meter.CreateObservableGauge("memory_stream_slim.count", static () => Volatile.Read(ref _activeStreamCount),
            unit: "{instances}", description: $"The number of active {nameof(MemoryStreamSlim)} instances");
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStreamSlim"/> class with the specified options.
    /// </summary>
    /// <param name="maximumCapacity">
    /// The maximum capacity that was configured when this stream was created.
    /// </param>
    /// <param name="mode">
    /// The operating mode of this stream instance.
    /// </param>
    /// <param name="options">
    /// The options used for configuring the stream instance settings.
    /// </param>
    /// <exclude />
    protected MemoryStreamSlim (long maximumCapacity, MemoryStreamSlimMode mode, MemoryStreamSlimOptions? options)
    {
        MaximumCapacity = maximumCapacity;
        Mode = mode;
        Settings = new MemoryStreamSlimSettings(options);
        AdjustActiveStreamCount(true);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStreamSlim"/> class with the specified options.
    /// </summary>
    /// <param name="maximumCapacity">
    /// The maximum capacity that was configured when this stream was created.
    /// </param>
    /// <param name="mode">
    /// The operating mode of this stream instance.
    /// </param>
    /// <param name="settings">
    /// The stream instance settings.
    /// </param>
    /// <exclude />
    protected MemoryStreamSlim (long maximumCapacity, MemoryStreamSlimMode mode, in MemoryStreamSlimSettings settings)
    {
        MaximumCapacity = maximumCapacity;
        Mode = mode;
        Settings = settings;
        AdjustActiveStreamCount(true);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// For known destination classes that are in-memory, we can optimize the copy operation
    /// by copying synchronously instead of asynchronously.
    /// </summary>
    /// <param name="destination">
    /// The destination stream to copy the contents of this stream to.
    /// </param>
    /// <param name="bufferSize">
    /// The size of the buffer to use for copying the stream contents.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// </param>
    /// <returns></returns>
    protected Task CopyToSyncAsAsync (Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        // We have no async needs here
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<int>(cancellationToken);

        try
        {
            CopyTo(destination, bufferSize);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Implemented in derived classes to set the capacity of the stream.
    /// </summary>
    /// <param name="capacityValue">
    /// The new capacity value to set.
    /// </param>
    /// <exclude />
    protected abstract void SetCapacity (long capacityValue);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies that the stream is not closed and throws an exception if it is.
    /// </summary>
    /// <exclude />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void EnsureNotClosed ()
    {
        if (IsOpen) return;
        if (IsDisposed)
            ThrowHelper.ThrowObjectDisposedException_StreamClosed();
        else
            ThrowHelper.ThrowInvalidOperationException_StreamClosed();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Verifies that the stream is not closed and is writable and throws an exception if it is.
    /// </summary>
    /// <exclude />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void EnsureWriteable ()
    {
        EnsureNotClosed();
        if (!CanWrite)
            ThrowHelper.ThrowNotSupportedException_UnwritableStream();
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified.
    /// </summary>
    /// <param name="sourceString">
    /// The source string to convert to bytes and use to fill the stream.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to convert the string to bytes.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    internal static MemoryStreamSlim Create(string sourceString, Encoding encoding)
    {
        byte[] bytes = encoding.GetBytes(sourceString);
        MemoryStreamSlim returnStream = Create(bytes.Length);
        returnStream.Write(bytes, 0, bytes.Length);
        returnStream.Position = 0;
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified, and options to configure the 
    /// stream instance.
    /// </summary>
    /// <param name="sourceString">
    /// The source string to convert to bytes and use to fill the stream.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to convert the string to bytes.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    internal static MemoryStreamSlim Create(string sourceString, Encoding encoding,
        in MemoryStreamSlimOptions options)
    {
        byte[] bytes = encoding.GetBytes(sourceString);
        MemoryStreamSlim returnStream = Create(bytes.Length, options);
        returnStream.Write(bytes, 0, bytes.Length);
        returnStream.Position = 0;
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified, and a delegate to set up options for
    /// the stream instance.
    /// </summary>
    /// <param name="sourceString">
    /// The source string to convert to bytes and use to fill the stream.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to convert the string to bytes.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the stream instance.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    internal static MemoryStreamSlim Create(string sourceString, Encoding encoding,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        byte[] bytes = encoding.GetBytes(sourceString);
        MemoryStreamSlim returnStream = Create(bytes.Length, optionsSetup);
        returnStream.Write(bytes, 0, bytes.Length);
        returnStream.Position = 0;
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class filled with
    /// the bytes from the specified string using the provided encoding.
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state data to pass to the options setup delegate.
    /// </typeparam>
    /// <param name="sourceString">
    /// The source string to convert to bytes and use to fill the stream.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to convert the string to bytes.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the stream instance.
    /// </param>
    /// <param name="state">
    /// State data to pass to the options setup delegate.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    internal static MemoryStreamSlim Create<TState>(string sourceString, Encoding encoding,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup,
        TState state)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        byte[] bytes = encoding.GetBytes(sourceString);
        MemoryStreamSlim returnStream = Create(bytes.Length, optionsSetup, state);
        returnStream.Write(bytes, 0, bytes.Length);
        returnStream.Position = 0;
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized to zero.
    /// </summary>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized to zero.
    /// </returns>
    public static MemoryStreamSlim Create ()
    {
        // Grab a reference to the settings lock to check if it is still valid or needed
        LockGlobalSettings();

        MemoryStreamSlimSettings settings;
        // Avoid a torn read on GlobalDefaultSettings
        lock (SettingsLock)
        {
            settings = GlobalDefaultSettings;
        }
        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, settings);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified.
    /// </summary>
    /// <param name="capacity">
    /// The initial capacity of the stream in bytes.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    public static MemoryStreamSlim Create (long capacity)
    {
        LockGlobalSettings();

        MemoryStreamSlimSettings settings;
        // Avoid a torn read on GlobalDefaultSettings
        lock (SettingsLock)
        {
            settings = GlobalDefaultSettings;
        }
        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, capacity, settings);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized to zero, and options to configure the
    /// stream instance.
    /// </summary>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized to zero.
    /// </returns>
    public static MemoryStreamSlim Create (in MemoryStreamSlimOptions options)
    {
        LockGlobalSettings();

        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified, and options to configure the 
    /// stream instance.
    /// </summary>
    /// <param name="capacity">
    /// The initial capacity of the stream in bytes.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    public static MemoryStreamSlim Create (long capacity, in MemoryStreamSlimOptions options)
    {
        LockGlobalSettings();

        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, capacity, options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized to zero, and a delegate to set up options for
    /// the stream instance.
    /// </summary>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the stream instance.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized to zero.
    /// </returns>
    public static MemoryStreamSlim Create (Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));

        LockGlobalSettings();

        MemoryStreamSlimOptions options;
        // Avoid a torn read on GlobalDefaultSettings
        lock (SettingsLock)
        {
            options = GlobalDefaultSettings.ToOptions();
        }
        options = optionsSetup(options);
        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified, and a delegate to set up options for
    /// the stream instance.
    /// </summary>
    /// <param name="capacity">
    /// The initial capacity of the stream in bytes.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the stream instance.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    public static MemoryStreamSlim Create (long capacity, Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));

        LockGlobalSettings();

        MemoryStreamSlimOptions options;
        // Avoid a torn read on GlobalDefaultSettings
        lock (SettingsLock)
        {
            options = GlobalDefaultSettings.ToOptions();
        }
        options = optionsSetup(options);
        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, capacity, options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized to zero, and a delegate to set up options for
    /// the stream instance.
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state data to pass to the options setup delegate.
    /// </typeparam>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the stream instance.
    /// </param>
    /// <param name="state">
    /// State data to pass to the options setup delegate.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized to zero.
    /// </returns>
    public static MemoryStreamSlim Create<TState> (Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup,
        TState state)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));

        LockGlobalSettings();

        MemoryStreamSlimOptions options;
        // Avoid a torn read on GlobalDefaultSettings
        lock (SettingsLock)
        {
            options = GlobalDefaultSettings.ToOptions();
        }
        options = optionsSetup(options, state);
        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the <see cref="MemoryStreamSlim"/> class with an 
    /// expandable capacity initialized as specified, and a delegate to set up options for
    /// the stream instance.
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state data to pass to the options setup delegate.
    /// </typeparam>
    /// <param name="capacity">
    /// The initial capacity of the stream in bytes.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the stream instance.
    /// </param>
    /// <param name="state">
    /// State data to pass to the options setup delegate.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class with an expandable
    /// capacity initialized as specified.
    /// </returns>
    public static MemoryStreamSlim Create<TState> (long capacity, Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup,
        TState state)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));

        LockGlobalSettings();

        MemoryStreamSlimOptions options;
        // Avoid a torn read on GlobalDefaultSettings
        lock (SettingsLock)
        {
            options = GlobalDefaultSettings.ToOptions();
        }
        options = optionsSetup(options, state);
        return new SegmentMemoryStreamSlim(GlobalMaximumCapacity, capacity, options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new non-resizable instance of the <see cref="MemoryStreamSlim"/> 
    /// class based on the specified byte array.
    /// </summary>
    /// <param name="buffer">
    /// The array of unsigned bytes from which to create the current stream.
    /// </param>
    /// <remarks>
    /// This will return a <see cref="MemoryStreamSlimMode.Fixed"/> model <see cref="MemoryStreamSlim"/>
    /// instance that simply wraps a standard <see cref="MemoryStream"/> instance around the provided
    /// buffer, and will not use any of the specialized memory management features of the
    /// <see cref="MemoryStreamSlimMode.Dynamic"/> mode instances. In general, this should only be used
    /// to provide a consistent environment for code that expects a <see cref="MemoryStreamSlim"/> instance, 
    /// or to allow the metric counters and logging available with <see cref="MemoryStreamSlim"/>,
    /// otherwise, there is no specific benefit to using this over a standard <see cref="MemoryStream"/> instance.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class based on the 
    /// specified byte array.
    /// </returns>
    public static MemoryStreamSlim Create (byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        LockGlobalSettings();
        return new MemoryStreamWrapper(new MemoryStream(buffer));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new non-resizable instance of the <see cref="MemoryStreamSlim"/> class 
    /// based on the specified byte array with the CanWrite property set as specified.
    /// </summary>
    /// <param name="buffer">
    /// The array of unsigned bytes from which to create the current stream.
    /// </param>
    /// <param name="writable">
    /// The setting of the CanWrite property, which determines whether the stream supports writing.
    /// </param>
    /// <remarks>
    /// This will return a <see cref="MemoryStreamSlimMode.Fixed"/> model <see cref="MemoryStreamSlim"/>
    /// instance that simply wraps a standard <see cref="MemoryStream"/> instance around the provided
    /// buffer, and will not use any of the specialized memory management features of the
    /// <see cref="MemoryStreamSlimMode.Dynamic"/> mode instances. In general, this should only be used
    /// to provide a consistent environment for code that expects a <see cref="MemoryStreamSlim"/> instance, 
    /// or to allow the metric counters and logging available with <see cref="MemoryStreamSlim"/>,
    /// otherwise, there is no specific benefit to using this over a standard <see cref="MemoryStream"/> instance.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class based on the 
    /// specified byte array with the CanWrite property set as specified.
    /// </returns>
    public static MemoryStreamSlim Create (byte[] buffer, bool writable)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        LockGlobalSettings();
        return new MemoryStreamWrapper(new MemoryStream(buffer, writable));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new non-resizable instance of the <see cref="MemoryStreamSlim"/> class
    /// based on the specified region (index) of a byte array.
    /// </summary>
    /// <param name="buffer">
    /// The array of unsigned bytes from which to create the current stream.
    /// </param>
    /// <param name="index">
    /// The index in buffer at which the stream begins.
    /// </param>
    /// <param name="count">
    /// The length of the stream in bytes.
    /// </param>
    /// <remarks>
    /// This will return a <see cref="MemoryStreamSlimMode.Fixed"/> model <see cref="MemoryStreamSlim"/>
    /// instance that simply wraps a standard <see cref="MemoryStream"/> instance around the provided
    /// buffer, and will not use any of the specialized memory management features of the
    /// <see cref="MemoryStreamSlimMode.Dynamic"/> mode instances. In general, this should only be used
    /// to provide a consistent environment for code that expects a <see cref="MemoryStreamSlim"/> instance, 
    /// or to allow the metric counters and logging available with <see cref="MemoryStreamSlim"/>,
    /// otherwise, there is no specific benefit to using this over a standard <see cref="MemoryStream"/> instance.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class based on the 
    /// specified region (index) of a byte array.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index or count is outside the bounds of the buffer.
    /// </exception>
    public static MemoryStreamSlim Create (byte[] buffer, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (index < 0 || index > buffer.Length - count)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(index));
        if (count < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));
        if (index > buffer.Length - count)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(count), 0, buffer.Length - index);
        LockGlobalSettings();
        return new MemoryStreamWrapper(new MemoryStream(buffer, index, count));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new non-resizable instance of the <see cref="MemoryStreamSlim"/> class 
    /// based on the specified region of a byte array, with the CanWrite property set as specified.
    /// </summary>
    /// <param name="buffer">
    /// The array of unsigned bytes from which to create the current stream.
    /// </param>
    /// <param name="index">
    /// The index in buffer at which the stream begins.
    /// </param>
    /// <param name="count">
    /// The length of the stream in bytes.
    /// </param>
    /// <param name="writable">
    /// The setting of the CanWrite property, which determines whether the stream supports writing.
    /// </param>
    /// <remarks>
    /// This will return a <see cref="MemoryStreamSlimMode.Fixed"/> model <see cref="MemoryStreamSlim"/>
    /// instance that simply wraps a standard <see cref="MemoryStream"/> instance around the provided
    /// buffer, and will not use any of the specialized memory management features of the
    /// <see cref="MemoryStreamSlimMode.Dynamic"/> mode instances. In general, this should only be used
    /// to provide a consistent environment for code that expects a <see cref="MemoryStreamSlim"/> instance, 
    /// or to allow the metric counters and logging available with <see cref="MemoryStreamSlim"/>,
    /// otherwise, there is no specific benefit to using this over a standard <see cref="MemoryStream"/> instance.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class based on the specified 
    /// region of a byte array, with the CanWrite property set as specified.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index or count is outside the bounds of the buffer.
    /// </exception>
    public static MemoryStreamSlim Create (byte[] buffer, int index, int count, bool writable)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (index < 0 || index > buffer.Length - count)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(index));
        if (count < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));
        if (index > buffer.Length - count)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(count), 0, buffer.Length - index);
        LockGlobalSettings();
        return new MemoryStreamWrapper(new MemoryStream(buffer, index, count, writable));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new non-resizable instance of the <see cref="MemoryStreamSlim"/> class 
    /// based on the specified region of a byte array, with the CanWrite property set as 
    /// specified, and the ability to call GetBuffer() set as specified.
    /// </summary>
    /// <param name="buffer">
    /// The array of unsigned bytes from which to create the current stream.
    /// </param>
    /// <param name="index">
    /// The index in buffer at which the stream begins.
    /// </param>
    /// <param name="count">
    /// The length of the stream in bytes.
    /// </param>
    /// <param name="writable">
    /// The setting of the CanWrite property, which determines whether the stream supports writing.
    /// </param>
    /// <param name="publiclyVisible">
    /// true to enable GetBuffer(), which returns the unsigned byte array from which the 
    /// stream was created; otherwise, false.
    /// </param>
    /// <remarks>
    /// This will return a <see cref="MemoryStreamSlimMode.Fixed"/> model <see cref="MemoryStreamSlim"/>
    /// instance that simply wraps a standard <see cref="MemoryStream"/> instance around the provided
    /// buffer, and will not use any of the specialized memory management features of the
    /// <see cref="MemoryStreamSlimMode.Dynamic"/> mode instances. In general, this should only be used
    /// to provide a consistent environment for code that expects a <see cref="MemoryStreamSlim"/> instance, 
    /// or to allow the metric counters and logging available with <see cref="MemoryStreamSlim"/>,
    /// otherwise, there is no specific benefit to using this over a standard <see cref="MemoryStream"/> instance.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="MemoryStreamSlim"/> class based on the specified 
    /// region of a byte array, with the CanWrite property set as specified, and the 
    /// ability to call GetBuffer() set as specified.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index or count is outside the bounds of the buffer.
    /// </exception>
    public static MemoryStreamSlim Create (byte[] buffer, int index, int count, bool writable, bool publiclyVisible)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (index < 0 || index > buffer.Length - count)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(index));
        if (count < 0)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));
        if (index > buffer.Length - count)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(count), 0, buffer.Length - index);
        LockGlobalSettings();
        return new MemoryStreamWrapper(new MemoryStream(buffer, index, count, writable, publiclyVisible));
    }
    //--------------------------------------------------------------------------------

    #region Options And Settings

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Configures the default settings for <see cref="MemoryStreamSlim"/> instances based
    /// on the specified options.
    /// </summary>
    /// <param name="options">
    /// Options to configure the global default settings.
    /// </param>
    public static void SetGlobalDefaultSettings (in MemoryStreamSlimOptions options)
    {
        CheckGlobalSettings();
        lock (SettingsLock)
        {
            // Set the global settings based on the passed options
            GlobalDefaultSettings = new MemoryStreamSlimSettings(options);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Configures the default settings for <see cref="MemoryStreamSlim"/> instances based
    /// on the specified options configured by the setup delegate.
    /// </summary>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the global default settings.
    /// </param>
    public static void SetGlobalDefaultSettings (Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));

        CheckGlobalSettings();

        MemoryStreamSlimOptions options;
        lock (SettingsLock)
        {
            // Get the initial options from the current global settings
            options = GlobalDefaultSettings.ToOptions();
        }
        // Let the user set up the options
        options = optionsSetup(options);
        lock (SettingsLock)
        {
            // Update the global settings with the new options
            GlobalDefaultSettings = new MemoryStreamSlimSettings(options);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Configures the default settings for <see cref="MemoryStreamSlim"/> instances based
    /// on the specified options configured by the setup delegate.
    /// </summary>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the global default settings.
    /// </param>
    /// <param name="state">
    /// State data to pass to the options setup delegate.
    /// </param>
    public static void SetGlobalDefaultSettings<TState> (Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup,
        TState state)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        CheckGlobalSettings();

        MemoryStreamSlimOptions options;
        lock (SettingsLock)
        {
            // Get the initial options from the current global settings
            options = GlobalDefaultSettings.ToOptions();
        }
        // Let the user set up the options
        options = optionsSetup(options, state);
        lock (SettingsLock)
        {
            // Update the global settings with the new options
            GlobalDefaultSettings = new MemoryStreamSlimSettings(options);
        }
    }
    //--------------------------------------------------------------------------------

    #endregion Options And Settings

    //--------------------------------------------------------------------------------
    /// <summary>
    /// A unique identifier for this stream instance.
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a string that is decoded from the data contained in the current stream 
    /// using the specified encoding.
    /// </summary>
    /// <remarks>
    /// This method will read the entire stream of data bytes for the decoding process,
    /// regardless of the current position of the stream. This could also result in
    /// an internal allocation of a temporary buffer to hold the data bytes, which
    /// could be inefficient for large streams.
    /// </remarks>
    /// <param name="encoding">
    /// The encoding to use to decode the byte stream.
    /// </param>
    public abstract string Decode(Encoding encoding);
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
#pragma warning disable HAA0601
    public override string ToString () =>
        IsDisposed ?
            $@"(Disposed) ID = {Id}" :
            IsOpen ? $@"ID = {Id}, Length = {Length}, Position = {Position}, Mode = {Mode.GetString()}" :
                $@"(Closed) ID = {Id}";
#pragma warning restore HAA0601
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of Stream

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool CanRead => IsOpen;
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool CanSeek => IsOpen;
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool CanWrite => CanWriteInternal;
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override void Flush ()
    {
        // No-op - there is no concept of flushing with a memory stream.
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Task FlushAsync (CancellationToken cancellationToken)
    {
        // We have no async needs here
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        try
        {
            Flush();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Task<int> ReadAsync (byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        // We have no async needs here
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<int>(cancellationToken);

        try
        {
            return Task.FromResult(Read(buffer, offset, count));
        }
        catch (Exception ex)
        {
            return Task.FromException<int>(ex);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override ValueTask<int> ReadAsync (Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        // We have no async needs here
        if (cancellationToken.IsCancellationRequested)
            return ValueTask.FromCanceled<int>(cancellationToken);

        try
        {
            // TODO - Consider getting an array from the buffer before using span.
            return ValueTask.FromResult(Read(buffer.Span));
        }
        catch (Exception ex)
        {
            return ValueTask.FromException<int>(ex);
        }
    }
    //--------------------------------------------------------------------------------

    #endregion Overrides of Stream    

    #region Overrides of MemoryStream

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of bytes allocated for this stream.
    /// </summary>
    /// <value>
    /// The length of the usable portion of the buffer for the stream.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The capacity is set to a value less than zero or greater than <see cref="MaximumCapacity"/>.
    /// </exception>
    public override int Capacity
    {
        get
        {
            long returnValue = CapacityInternal;
            if (returnValue > int.MaxValue)
                ThrowHelper.ThrowInvalidOperationException_IntOverflowCapacity();
            return (int)returnValue;
        }
        set
        {
            if (CapacityInternal == value)
                return;
            CapacityLong = value;
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Task CopyToAsync (Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(destination, nameof(destination));
        if (bufferSize <= 0)
            ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(bufferSize));

        return destination is MemoryStreamSlim or MemoryStream ?
            // We can optimize the copy operation by copying synchronously instead of asynchronously
            CopyToSyncAsAsync(destination, bufferSize, cancellationToken) :
            // Just use the base class implementation
            base.CopyToAsync(destination, bufferSize, cancellationToken);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Task WriteAsync (byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        // We have no async needs here
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        try
        {
            Write(buffer, offset, count);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override ValueTask WriteAsync (ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        // We have no async needs here
        if (cancellationToken.IsCancellationRequested)
            return ValueTask.FromCanceled(cancellationToken);

        try
        {
            // TODO - Consider getting an array from the buffer before using span.
            Write(buffer.Span);
            return default;
        }
        catch (Exception ex)
        {
            return ValueTask.FromException(ex);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [DoesNotReturn]
    public override byte[] GetBuffer ()
    {
        ThrowHelper.ThrowNotSupportedException_FeatureNotSupported();
        return null;
    }
    //--------------------------------------------------------------------------------

    #endregion Overrides of MemoryStream

    #region MemoryStream Support

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of bytes allocated for this stream as a long integer.
    /// </summary>
    /// <value>
    /// The length of the usable portion of the buffer for the stream as a long integer.
    /// </value>
    /// <remarks>
    /// For current compatibility with <see cref="MemoryStream"/> and <see cref="Stream"/> classes,
    /// no instances of <see cref="MemoryStreamSlim"/> can have a capacity greater than 
    /// <see cref="int.MaxValue"/>. This property is simply a convenience to allow setting
    /// and getting the capacity as a long integer. The <see cref="Capacity"/> property 
    /// should be used for all normal operations.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The capacity is set to a value less than zero or greater than <see cref="MaximumCapacity"/>.
    /// </exception>
    public virtual long CapacityLong
    {
        get => CapacityInternal;
        set
        {
            if (CapacityInternal == value)
                return;
            if (value < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(Capacity));
            if (value < LengthInternal)
                ThrowHelper.ThrowArgumentOutOfRangeException_CapacitySmall(nameof(Capacity));
            if (value > MaximumCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(Capacity), 0, MaximumCapacity);
            SetCapacity(value);
        }
    }
    //--------------------------------------------------------------------------------

    #endregion MemoryStream Support
}
//################################################################################
