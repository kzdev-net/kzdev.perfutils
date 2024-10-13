# MemoryStream

The .NET class library provides a `MemoryStream` class representing a stream of bytes stored in memory. It operates two implied modes: expandable (dynamic) or fixed, determined by how you instantiate the `MemoryStream` instance. In expandable mode, the `MemoryStream` class uses a single-byte array to store the data, and the array is resized as needed to accommodate the data as the length and capacity change. In fixed mode, the `MemoryStream` class uses a fixed-size byte array to store the data provided to the constructor during instantiation.

The [`MemoryStream`](xref:System.IO.MemoryStream) class is optimized for and works well in fixed mode, providing `Stream` semantics to a byte array you have already allocated. However, in expandable mode, the `MemoryStream` class can result in poor performance and a lot of garbage collection pressure when working with large amounts of memory, many `MemoryStream` instances being created and disposed of frequently, or when the stream instances grow dynamically.

Microsoft released a separate [`RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) package that addresses many issues with the expandable mode of the `MemoryStream` class. Unfortunately, the `RecyclableMemoryStream` package is somewhat cumbersome to use. It also has several limitations, including requiring an application to have consistent usage and behavior patterns and then tune configuration options to those patterns. In some scenarios, this class can also perform much worse than the standard `MemoryStream` class. One mistake with the `RecyclableMemoryStream` class is that it doesn't focus strictly on fixing the expandable mode of the `MemoryStream` class but instead tries to provide a more general-purpose memory stream class that can be used for a fixed mode scenario as well. The issue is that `RecyclableMemoryStream` typically has worse throughput performance than the standard `MemoryStream` class in most fixed-mode scenarios.

[`MemoryStreamSlim`](xref:KZDev.PerfUtils.MemoryStreamSlim) is a memory stream that is optimized for performance. It is a drop-in replacement for the `MemoryStream` class and is designed to be used in scenarios where performance and memory management are crucial. This class is instrumental in scenarios where you need expandable memory-based stream instances that are substantially large, the stream size needs or the general use cases vary a lot, or you use memory-based streams frequently. Since the standard `MemoryStream` works so well as a `Stream` semantic API wrapper around existing byte arrays, `MemoryStreamSlim` does not attempt to fix what isn't broken and focuses on the expandable mode use cases. This is discussed more below.

## Memory Traffic

Even relatively small memory streams can generate a lot of memory traffic by allocating and releasing memory buffers. This can lead to performance issues by putting pressure on the garbage collector.

Larger memory streams can cause issues because they can create allocations on the [Large Object Heap (LOH)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap). The LOH is a particular heap in the .NET runtime that is used to store large objects (objects that are 85,000 bytes or larger by default). The LOH is not usually compacted by the garbage collector, which means that if you have a lot of large objects on the LOH, it can become fragmented over time, leading to performance issues.



`MemoryStreamSlim` is designed to minimize memory traffic by reusing memory buffers. It uses a specialized, internally managed, and optimized set of 'small' heap-allocated re-usable buffers for buffers smaller than a certain threshold.

`MemoryStreamSlim` allocates groups of buffer segments from the LOH by default for larger buffers and reuses them as needed. This helps reduce the number of allocations and deallocations performed and helps prevent LOH fragmentation, which can help improve performance by reducing GC pressure.

This memory management is only utilized when the `MemoryStreamSlim` instance is created without wrapping an existing byte array. If you create a `MemoryStreamSlim` instance with an existing byte array, the `MemoryStreamSlim` instance will become a wrapper around the standard `MemoryStream` class because there is no beneficial memory management that can be provided in this case. Suppose you need stream semantic access to a byte array you have already allocated. In that case, it might be best to use the standard `MemoryStream` class directly, though the overhead of the `MemoryStreamSlim` wrapper is not measurable and can be used in cases where you just want to use the same class type for all stream instances.

## Usage

To create a `MemoryStreamSlim` instance, use one of the [`Create`](xref:KZDev.PerfUtils.MemoryStreamSlim.Create) static overload methods on the MemoryStreamSlim class. These methods allow you to create a `MemoryStreamSlim` instance with an optional specified initial capacity, select options for the stream instance, or provide an existing byte array to wrap.

Besides creation, the `MemoryStreamSlim` class can be used exactly like the standard `MemoryStream` class. It implements all the same current methods and properties as `MemoryStream` so that you can use it as a drop-in replacement in your code. Deprecated and outdated methods and properties are not implemented in `MemoryStreamSlim`.

```csharp
using KZDev.PerfUtils;

// Create a new MemoryStreamSlim instance with an initial capacity of 1024 bytes, and setting the option to not clear memory buffers
using (Stream stream = MemoryStreamSlim.Create(1024, options => options.ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None))
{
    // Read and Write stream operations...
}
```

The one notable difference between `MemoryStreamSlim` and `MemoryStream` behavior is the implementation of the [GetBuffer](xref:KZDev.PerfUtils.MemoryStreamSlim.GetBuffer*) method. For fixed mode, the `GetBuffer` method works exactly as the `MemoryStream` class does, returning the underlying byte array that the stream is using if the instance was created with the 'publiclyVisible' parameter set to `true`, otherwise throwing an exception. For expandable mode `MemoryStreamSlim` instances, the `GetBuffer` method will strictly throw a `NotSupportedException`. This is a purposeful design decision to prevent misuse of the `MemoryStreamSlim` class in scenarios where the underlying buffer is not guaranteed to be contiguous in memory, and there are better and safer ways to access the data in the stream than getting direct access to the buffer. The `MemoryStreamSlim` class is designed to be a high-performance memory stream, and the `GetBuffer` method is not a high-performance method in the general case, and results of direct buffer access can be unpredictable and unsafe.

`RecyclableMemoryStream` on the other hand, supports calling `GetBuffer` and suggests using it over `ToArray` for performance reasons to get a byte array representation of the stream contents. The documentation has many warnings about the potential performance implications of using `GetBuffer` however and the need to be careful with its use. In addition, the first call to `GetBuffer` will cause the `RecyclableMemoryStream` class's internal workings to change so that performance could degrade significantly for that instance from that point on.

## How MemoryStreamSlim Works

`MemoryStreamSlim` is designed to be highly efficient regarding memory usage and throughput performance. It combines techniques to achieve this, but the primary technique is to reuse memory buffers as much as possible. . First, this is done by breaking memory needs into small and large (standard) buffers. These buffers are internally chained to create a logically contiguous memory stream of nearly any size.

`MemoryStreamSlim` stores the byte stream in an internally optimized pool of small reusable buffers when the stream's memory capacity is relatively small. Given their size, these buffers are GC-allocated the same as any byte array, but they are cached for reuse when no longer needed, so there is a performance gain there.

When the memory capacity of the stream is larger than the small buffer threshold, `MemoryStreamSlim` allocates large buffers from the Large Object Heap (LOH) 
divided into 64K logical segments. The segments are internally managed and linked together to create highly efficient memory streams of any size within the limits of the memory limitations placed on the process. These large buffers are cached for future use, which avoids memory traffic as well as helps to prevent LOH fragmentation, which could otherwise lead to performance issues by putting pressure on the GC.

## Features

One of `MemoryStreamSlim`'s primary design goals is to provide a high-performance memory stream that is easy to use and minimizes memory traffic. It 'just works' without testing and tweaking different usage scenarios and option settings. However, a few options are available that can be used to control some of the behavior of MemoryStreamSlim instances. Those are discussed below.

### Clearing Memory

By default, `MemoryStreamSlim` will zero out the memory buffers that are used when they are released. This is done for security reasons, to prevent potentially sensitive data from being left in memory after use. There are options that allow you to control this behavior. [Read More](./memory-management.md#clearing-memory)

### Releasing Memory

`MemoryStreamSlim` provides a [`ReleaseMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.ReleaseMemoryBuffers) method that that allows you to release the memory buffers cached for use by new stream instances. Releasing this memory can be useful when you want to free up cached memory that is no longer needed. [Read More](./memory-management.md#releasing-memory)

### Native Memory

`MemoryStreamSlim` can use native memory internally instead of the GC heap to further avoid the GC Large Object Heap fragmentation. [Read More](./memory-management.md#native-memory)

## Monitoring

`MemoryStreamSlim` provides a couple of ways to monitor usage using the `Metrics` and `Events` features of the .NET runtime. [Read More](./memory-monitoring.md)
