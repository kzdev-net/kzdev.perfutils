# MemoryStreamSlim Memory Management

You can help manage the memory used by MemoryStreamSlim instances in your application in a couple of ways. They are:

- [Clearing Memory](#clearing-memory) behavior
- [Releasing Memory](#releasing-memory)
- Optionally using [Native Memory](#native-memory)


## Clearing Memory

By default, `MemoryStreamSlim` will zero out its memory buffers when they are no longer used and before the memory segments are cached for future use. This is done for security reasons to prevent sensitive data from being left in memory after it has been used but is no longer needed. However, zeroing out memory buffers can be time-consuming, especially for large buffers.

You can control how the memory buffers are cleared by setting the `MemoryStreamSlimOptions` [`ZeroBufferBehavior`](xref:KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior) property, which can be set globally as a default value or, per instance, when instantiating `MemoryStreamSlim`. The ZeroBufferBehavior property can be set to one of the following [MemoryStreamSlimZeroBufferOption](xref:KZDev.PerfUtils.MemoryStreamSlimZeroBufferOption) values:

- `None`: No clearing of memory buffers is performed. This is the fastest option, but it can leave potentially sensitive data in memory. For streams that don't contain any sensitive data, this can be a good option to improve performance.
- `OutOfBand`: **This is the default behavior.** . Memory buffers are cleared out-of-band, meaning the clearing is done on a separate thread. This can help reduce the latency impact on the thread disposing of or reducing the capacity on MemoryStreamSlim. Still, it can briefly leave information in memory before it is cleared and returned to the buffer cache. This hybrid approach keeps potentially sensitive information out of memory while providing better performance for the threads using `MemoryStreamSlim`. While this processes in near real time, it is not instantaneous.
- `OnRelease`: Memory buffers are cleared instantly when the MemoryStreamSlim instance is disposed, or buffers are released when the capacity is reduced. This is the most secure option, but it can impact latency performance when disposing of `MemoryStreamSlim` instances with large memory buffers.

```csharp
using KZDev.PerfUtils;

// Create a new MemoryStreamSlim instance with an expandable initial capacity of 0 bytes, and setting the option to not clear memory buffers
using (Stream stream = MemoryStreamSlim.Create(options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None))
{
    // Read and Write stream operations...
}
```

## Releasing Memory

One of the primary benefits of using `MemoryStreamSlim` is that it caches memory buffers for reuse by new stream instances. This can help reduce the number of allocations and deallocations performed and help prevent Large Object Heap (LOH) fragmentation. However, there are likely to be cases where you may have used a large `MemoryStreamSlim` instance that is not often used or is a one-off use. Also, there may be cases where a large amount of memory was used for many `MemoryStreamSlim` instances, but now that memory is no longer needed.

To keep from letting the reserved memory buffers sit around indefinitely, excess memory buffers can be released for garbage collection. This is handled two different ways in `MemoryStreamSlim`; one is automatic and the other is manual.

### Automatic Memory Release

On a regularly scheduled basis, `MemoryStreamSlim` will check the memory buffers that are being held for reuse by new stream instances. If there are memory buffers that have not been used for a long period of time, they will be released for garbage collection. This is done to help prevent holding on to reserved, but unused memory buffers indefinitely. Letting the system manage this process is the best way to ensure that memory is released when it is no longer needed.

Alternatively, you can manually release the memory buffers that are being held for reuse by new stream instances using the [`ReleaseMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.ReleaseMemoryBuffers) method

### Manual Memory Release - ReleaseMemoryBuffers

`MemoryStreamSlim` provides a static [`ReleaseMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.ReleaseMemoryBuffers) method that allows you to release the memory buffers being cached for use by the stream instances. This hints to the system that the memory buffers are no longer needed and can be released for garbage collection. After calling this method, the memory buffers will be released as soon as possible based on current usage and other factors. Still, the release of memory may not be immediate.

> It is important to note that calling `ReleaseMemoryBuffers` will mark all the currently allocated memory buffers as eligible for release, and new memory segments needed for new instances of `MemoryStreamSlim` will be allocated as needed. This can cause a performance hit if the memory buffers are needed again soon after being released. This is also different than the automatic memory release process, which will only release memory buffers that have not been used for a long period of time.

```csharp
using KZDev.PerfUtils;

// Release the memory buffers that are being held for reuse by new stream instances
MemoryStreamSlim.ReleaseMemoryBuffers();
```

The idea with allocating and reusing memory buffers is to reduce the number of allocations and deallocations performed and to help prevent Large Object Heap fragmentation, so you typically would not use this method and instead let the system manage the memory for you. However, in cases where you may have used an exceptionally large `MemoryStreamSlim` instance and you can determine that it was a one-off use, you can call this method to release the large set of memory buffers being cached for reuse.

After calling 'ReleaseMemoryBuffers', future instances of `MemoryStreamSlim` will allocate new memory buffers as needed, while the old memory buffers will be eligible for garbage collection when all stream instances using those old buffers are disposed.

## Native Memory

To avoid large heap fragmentation and reduce the number of allocations and deallocations, `MemoryStreamSlim` will, by default, allocate the memory buffer segments from the LOH and keep them available in a cache for future use. This does help avoid LOH fragmentation, but the memory is still allocated from the managed heap. Every full GC cycle will still have to review references to these segments in the LOH, and the memory buffers will still be subject to the same GC pressure as any other managed memory.

To avoid managed memory and GC pressure caused by these memory buffers altogether, you can configure the system to allocate the large memory buffer segments from the native memory heap. This is an AppDomain wide setting and must be done before creating any `MemoryStreamSlim` instances. You configure this by setting the [`UseNativeLargeMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.UseNativeLargeMemoryBuffers) static property to true before creating any `MemoryStreamSlim` instances.

The behavior of the `MemoryStreamSlim` instances will not differ when using native memory. The buffers remain internal to the `MemoryStreamSlim` and are never exposed publicly. This can be useful to avoid LOH fragmentation resulting from other large objects in your application being intermixed with the memory buffers used by MemoryStreamSlim. By putting the memory buffers in native memory, these fragmentation issues can be avoided.

```csharp
using KZDev.PerfUtils;

// Configure the system to allocate large memory buffers from native memory
MemoryStreamSlim.UseNativeLargeMemoryBuffers = true;
```

The number of 64K buffer segments that are allocated from native memory can be monitored using the [Native Segments Allocated](./memory-monitoring.md#segmentmemorynativeallocated-counter) counter.
