# Release Notes

## Version 3.0

### Breaking Changes

#### MemoryStreamSlim - Disposed Stream Behavior (dynamic mode)

**Changed:** **Dynamic-mode** `MemoryStreamSlim` (streams created without a caller-supplied buffer, i.e. segment-backed / pool-backed instances) now throws `ObjectDisposedException` from `Length`, `Position`, and capacity-related APIs after disposal, **where that behavior aligns with the BCL `MemoryStream` contract** for the same members.

The following members now throw `ObjectDisposedException` when used after the stream has been disposed (previously, dynamic mode often returned the last known values instead of throwing):

- **`Length` property getter**
- **`Position` property getter**
- **`Position` property setter**
- **`Capacity` property getter and setter**
- **`CapacityLong` property getter and setter**

**Impact:** If any consumer relied on **non-throwing** access to `Length`, `Position`, or capacity after `Dispose()` on **dynamic-mode** streams, that code will now fail with `ObjectDisposedException`. Fixed-mode streams (created with a provided buffer) continue to delegate these members to the wrapped BCL `MemoryStream`, which already follows BCL disposed semantics.

**Migration:** Do not read or set `Length`, `Position`, `Capacity`, or `CapacityLong` on a disposed dynamic-mode stream. Capture any values you need before disposal. This aligns dynamic-mode observability with BCL `MemoryStream` and surfaces incorrect lifetime use earlier.

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

These changes improve compatibility with the BCL `MemoryStream` class behavior when streams are disposed. Dynamic-mode `MemoryStreamSlim` now matches BCL disposed semantics for `Length`, `Position`, and capacity surface area as described above, with the documented exception of dynamic mode `ToArray()` after disposal, which is a necessary limitation for memory efficiency.

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
