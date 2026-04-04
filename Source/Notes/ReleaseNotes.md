# Release Notes

## Version 3.0

### Breaking Changes

#### MemoryStreamSlim - Disposed Stream Behavior

**Changed:** Improved compatibility with BCL `MemoryStream` behavior when accessing properties on disposed streams.

The following property accessors now throw `ObjectDisposedException` when accessed after the stream has been disposed, matching the behavior of the BCL `MemoryStream` class:

- **`Length` property getter** - Previously returned the last known length value; now throws `ObjectDisposedException`
- **`Position` property getter** - Previously returned the last known position value; now throws `ObjectDisposedException`
- **`Capacity` property getter** - Previously returned the last known capacity value; now throws `ObjectDisposedException`
- **`Capacity` property setter** - Now explicitly throws `ObjectDisposedException` (previously may have thrown through other validation)
- **`CapacityLong` property getter** - Previously returned the last known capacity value; now throws `ObjectDisposedException`
- **`CapacityLong` property setter** - Now explicitly throws `ObjectDisposedException` (previously may have thrown through other validation)

**Impact:** Code that accesses these properties on disposed `MemoryStreamSlim` instances will now throw `ObjectDisposedException` instead of returning values. This change improves compatibility with BCL `MemoryStream` and helps catch programming errors where disposed streams are being accessed.

**Migration:** Review code that accesses `Length`, `Position`, or `Capacity` properties on `MemoryStreamSlim` instances and ensure the stream is not disposed before accessing these properties. If you need to access these values after disposal, store them in local variables before disposing the stream.

### Improvements

#### MemoryStreamSlim - Fixed Mode ToArray() After Disposal

**Changed:** Fixed mode `MemoryStreamSlim` instances (created with a provided buffer) now allow `ToArray()` to be called after disposal, matching BCL `MemoryStream` behavior.

**Note:** Dynamic mode `MemoryStreamSlim` instances (created without a provided buffer) will continue to throw `ObjectDisposedException` when `ToArray()` is called after disposal. This is an intentional design difference to maintain memory efficiency - dynamic mode streams release their buffers back to the pool during disposal, making the data unavailable. This trade-off is necessary to achieve the memory efficiency goals of `MemoryStreamSlim`.

#### MemoryStreamSlim - ToMemory (pooled contiguous export)

**Added:** `MemoryStreamSlim` now exposes `ToMemory()` and `ToMemory(MemoryPool<byte>)`, which copy the stream’s visible bytes into a contiguous buffer wrapped in an `IMemoryOwner<byte>`. The copy spans the stream from the beginning through `Length`, independent of `Position`, with the same observable limits as `ToArray()`.

**Why use `ToMemory` instead of `ToArray`?**

- **`ToArray()`** allocates a new `byte[]` on the GC heap every time. The array is reclaimed only when the garbage collector runs, which can add GC pressure when you materialize large payloads often.
- **`ToMemory()`** rents backing storage from a `MemoryPool<byte>` (shared pool by default, or a pool you supply). When you call `Dispose()` on the returned owner, the buffer is returned to that pool for reuse, which can reduce allocations and steady-state heap growth compared with repeated `ToArray()` calls.
- **`ToMemory()`** fits APIs that already work with `Memory<byte>` or `ReadOnlyMemory<byte>` without an extra array wrapper, and a custom pool lets you attribute or cap rentals for specific subsystems.

**Other details:** When the stream length is zero, the API returns a shared singleton owner whose `Dispose()` is a no-op and does not rent from any pool. For non-empty results, callers must dispose the owner so the rented buffer is returned. After disposal of the stream, behavior matches `ToArray()` by mode (fixed mode may still succeed when the underlying buffer remains available; dynamic mode throws `ObjectDisposedException`). Monitoring includes a dedicated `MemoryStreamSlimToMemory` event (separate from `MemoryStreamSlimToArray`) when a non-zero payload is materialized.

### Compatibility

These changes improve compatibility with the BCL `MemoryStream` class behavior when streams are disposed. The changes ensure that `MemoryStreamSlim` behaves consistently with `MemoryStream` in most scenarios, with the documented exception of dynamic mode `ToArray()` after disposal, which is a necessary limitation for memory efficiency.

## Version 2.0.1

This release of the 'KZDev.PerfUtils' package fixes a potential internal race condition that could lead to an index out of range exception.

## Version 2.0

This release of the 'KZDev.PerfUtils' package:

- Introduces several compression helper classes that use MemoryStreamSlim as the compression destination for improved performance.
- Adds targeted support for .NET 9
- MemoryStreamSlim supports int64 (long) sized streams.

## Version 1.2

This release of the 'KZDev.PerfUtils' package introduces the StringBuilderCache class which provides performance enhancements for frequent use of StringBuilders.
In addition, the MemoryStreamSlim class now provides internal memory trimming for memory segments that have not been used for an extended period.

## Version 1.1

This release of the 'KZDev.PerfUtils' package contains some additional performance enhancements to the MemoryStreamSlim class and adds the InterlockedOps class which provides additional thread-safe atomic operations beyond those available in the Interlocked class.

## Version 1.0

This is the first release of the KZDev.PerfUtils package containing the MemoryStreamSlim class; a high-performance, memory-efficient, and easy-to-use replacement for the MemoryStream class that provides benefits for large or frequently used streams.
