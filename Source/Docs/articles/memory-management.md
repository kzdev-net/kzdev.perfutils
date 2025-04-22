# MemoryStreamSlim Memory Management

You can help manage the memory used by MemoryStreamSlim instances in your application in several ways:

- [Clearing Memory](#clearing-memory) behavior
- [Releasing Memory](#releasing-memory)
- Optionally using [Native Memory](#native-memory)

## Clearing Memory

By default, **MemoryStreamSlim** zeroes out its memory buffers when they are no longer used and before the memory segments are cached for future use. This is done for security reasons to prevent sensitive data from remaining in memory after it is no longer needed. However, zeroing out memory buffers can be time-consuming, especially for large buffers.

You can control how memory buffers are cleared by setting the `MemoryStreamSlimOptions` [`ZeroBufferBehavior`](xref:KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior) property. This property can be set globally as a default value or per instance when instantiating **MemoryStreamSlim**. The ZeroBufferBehavior property can be set to one of the following [MemoryStreamSlimZeroBufferOption](xref:KZDev.PerfUtils.MemoryStreamSlimZeroBufferOption) values:

- **None**: No clearing of memory buffers is performed. This is the fastest option but may leave potentially sensitive data in memory. For streams that do not contain sensitive data, this can improve performance.
- **OutOfBand**: This is the default behavior. Memory buffers are cleared out-of-band, meaning the clearing is done on a separate thread. This reduces the latency impact on the thread disposing of or reducing the capacity of **MemoryStreamSlim**. However, it may briefly leave information in memory before it is cleared and returned to the buffer cache. This hybrid approach balances security and performance.
- **OnRelease**: Memory buffers are cleared immediately when the **MemoryStreamSlim** instance is disposed or when buffers are released due to capacity reduction. This is the most secure option but can impact latency performance when disposing of **MemoryStreamSlim** instances with large memory buffers.

```csharp
using KZDev.PerfUtils;

// Create a new MemoryStreamSlim instance with an expandable initial capacity of 0 bytes, and setting the option to not clear memory buffers
using (Stream stream = MemoryStreamSlim.Create(options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None))
{
    // Read and Write stream operations...
}
```

## Releasing Memory

One of the primary benefits of using **MemoryStreamSlim** is its ability to cache memory buffers for reuse by new stream instances. This reduces the number of allocations and deallocations performed and helps prevent Large Object Heap (LOH) fragmentation. However, there may be cases where you have used a large **MemoryStreamSlim** instance that is infrequently used or a one-off. Additionally, a large amount of memory may have been used for many **MemoryStreamSlim** instances, but that memory is no longer needed.

To prevent reserved memory buffers from sitting idle indefinitely, **MemoryStreamSlim** provides two ways to release excess memory buffers: **automatic** and **manual**.

### Automatic Memory Release

**MemoryStreamSlim** automatically checks its cached memory buffers on a regular schedule. Buffers that have not been used for an extended period are released for garbage collection. This ensures that unused memory is not held indefinitely. Allowing the system to manage this process is the recommended approach for most scenarios.

### Manual Memory Release - ReleaseMemoryBuffers

**MemoryStreamSlim** provides a static [`ReleaseMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.ReleaseMemoryBuffers) method that allows you to manually release cached memory buffers. This method signals the system that the buffers are no longer needed and can be released for garbage collection. However, the release may not be immediate and depends on current usage and other factors.

> **Note:** Calling `ReleaseMemoryBuffers` marks all currently allocated memory buffers as eligible for release. If new memory buffers are needed shortly after calling this method, they will be reallocated, which can cause a performance hit. This behavior differs from the automatic memory release process, which only releases buffers that have been unused for an extended period.

```csharp
using KZDev.PerfUtils;

// Release the memory buffers that are being held for reuse by new stream instances
MemoryStreamSlim.ReleaseMemoryBuffers();
```

The primary goal of caching and reusing memory buffers is to reduce allocations and deallocations and prevent LOH fragmentation. Therefore, it is generally best to let the library manage memory automatically. However, in cases where you have used an exceptionally large **MemoryStreamSlim** instance and know it was a one-off use, you can call this method to quickly release the excess cached memory buffers.

After calling `ReleaseMemoryBuffers`, future **MemoryStreamSlim** instances will allocate new memory buffers and rebuild the cache as needed. Old memory buffers will become eligible for garbage collection once all stream instances using them are disposed.

## Native Memory

By default, **MemoryStreamSlim** allocates memory buffer segments from the LOH and keeps them in a cache for future use. While this approach helps avoid LOH fragmentation, the memory is still allocated from the managed heap. As a result, every full GC cycle must review references to these segments, and the buffers remain subject to GC pressure.

To completely avoid managed memory and GC pressure caused by these buffers, you can configure the system to allocate large memory buffer segments from the native memory heap. This is an AppDomain-wide setting and must be configured before creating any **MemoryStreamSlim** instances. To enable this behavior, set the [`UseNativeLargeMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.UseNativeLargeMemoryBuffers) static property to true.

The behavior of **MemoryStreamSlim** instances remains unchanged when using native memory. The buffers are internal to MemoryStreamSlim and are never exposed publicly. Using native memory can help avoid LOH fragmentation caused by other large objects in your application.

```csharp
using KZDev.PerfUtils;

// Configure the system to allocate large memory buffers from native memory
MemoryStreamSlim.UseNativeLargeMemoryBuffers = true;
```

The number of 64KB buffer segments allocated from native memory can be monitored using the [Native Segments Allocated](./memory-monitoring.md#segmentmemorynativeallocated-counter) counter.
