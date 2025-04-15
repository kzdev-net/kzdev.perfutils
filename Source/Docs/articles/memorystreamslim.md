# MemoryStreamSlim

The .NET class library provides a **MemoryStream** class representing a stream of bytes stored in memory. It operates in one of two implied modes: expandable (dynamic) or fixed, determined by how you instantiate the **MemoryStream** instance. In expandable mode, the **MemoryStream** class uses a single byte array to store the data, resizing the array as needed to accommodate changes in length and capacity. In fixed mode, the MemoryStream class uses a fixed-size byte array provided during instantiation.

The [`MemoryStream`](xref:System.IO.MemoryStream) class is optimized for fixed mode, providing `Stream` semantics to a pre-allocated byte array. However, in expandable mode, the MemoryStream class can result in poor performance and significant garbage collection (GC) pressure when working with large amounts of memory, frequently creating and disposing of MemoryStream instances, or dynamically growing streams.

Microsoft released the [`RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) package to address many issues with the expandable mode of the MemoryStream class. However, the **RecyclableMemoryStream** package has limitations, including requiring consistent usage patterns and tuning configuration options. In some scenarios, it can perform worse than the standard MemoryStream class. Additionally, **RecyclableMemoryStream** attempts to support both expandable and fixed modes, but it often has worse throughput performance than MemoryStream in fixed-mode scenarios.

[`MemoryStreamSlim`](xref:KZDev.PerfUtils.MemoryStreamSlim) is a high-performance, memory-efficient replacement for the **MemoryStream** class. It is designed for scenarios where performance and memory management are critical, particularly for expandable memory-based streams that are large, frequently used, or have varying use cases. Unlike **RecyclableMemoryStream**, **MemoryStreamSlim** focuses exclusively on improving expandable mode use cases, leaving fixed-mode scenarios to the standard MemoryStream class. **MemoryStreamSlim** does not attempt to fix what isn't broken and focuses on the expandable mode use cases. This is discussed more below.

## Memory Traffic

Even relatively small memory streams can generate significant memory traffic (churn) by frequently allocating and releasing memory buffers, leading to performance issues and GC pressure.

Larger memory streams can exacerbate these issues by creating allocations on the [Large Object Heap (LOH)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap). The LOH is used to store large objects (85,000 bytes or larger by default) and is typically not compacted by the garbage collector, which can lead to fragmentation and performance degradation over time.

**MemoryStreamSlim** minimizes memory traffic by reusing memory buffers. It uses a specialized, internally managed pool of small, heap-allocated reusable buffers for smaller memory needs. For larger buffers, it allocates groups of segments from the LOH and reuses them as needed. This approach reduces allocations and deallocations, prevents LOH fragmentation, and improves performance by reducing GC pressure.

When a **MemoryStreamSlim** instance is created with an existing byte array, it acts as a wrapper around the standard **MemoryStream** class, as no additional memory management benefits can be provided in this case. For scenarios requiring `Stream` semantics for pre-allocated byte arrays, the standard MemoryStream class is recommended, though the overhead of the **MemoryStreamSlim** wrapper is negligible.

## Usage

To create a `MemoryStreamSlim` instance, use one of the [`Create`](xref:KZDev.PerfUtils.MemoryStreamSlim.Create) static overload methods. These methods allow you to specify an initial capacity, configure options for the stream instance, or provide an existing byte array to wrap.

Once created, **MemoryStreamSlim** can be used as a drop-in replacement for the standard **MemoryStream** class. It implements all current methods and properties of **MemoryStream**, except for deprecated or outdated ones.

```csharp
using KZDev.PerfUtils;

// Create a new MemoryStreamSlim instance with an initial capacity of 1024 bytes, and setting the option to not clear memory buffers
using (Stream stream = MemoryStreamSlim.Create(1024, options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None))
{
    // Read and Write stream operations...
}
```

## Key Difference: GetBuffer Method

The primary behavioral difference between **MemoryStreamSlim** and **MemoryStream** is the implementation of the [GetBuffer](xref:KZDev.PerfUtils.MemoryStreamSlim.GetBuffer*) method:

- In fixed mode, `GetBuffer` behaves like **MemoryStream**, returning the underlying byte array if the instance was created with the publiclyVisible parameter set to true. Otherwise, it throws an exception.
- In expandable mode, `GetBuffer` always throws a `NotSupportedException`. This design prevents misuse in scenarios where the underlying buffer is not guaranteed to be contiguous in memory. Safer and more efficient methods are available for accessing stream data.

In contrast, **RecyclableMemoryStream** supports `GetBuffer` and recommends it over `ToArray` for performance reasons. However, its documentation warns about potential performance implications and the need for careful use. Additionally, the initial call to `GetBuffer` can significantly degrade the performance of the **RecyclableMemoryStream** instance from that point on.

## How MemoryStreamSlim Works

`MemoryStreamSlim` achieves high efficiency in memory usage and throughput performance by reusing memory buffers. It divides memory needs into small and large buffers, which are internally chained to create a logically contiguous memory stream.

- Small Buffers: For smaller memory capacities, **MemoryStreamSlim** uses a pool of small, reusable buffers. These buffers are GC-allocated but cached for reuse, reducing allocation overhead.
- Large Buffers: For larger memory capacities, **MemoryStreamSlim** allocates multiples of 64KB segments from the LOH. These segments are linked together to create efficient memory streams of any size. By caching these buffers, **MemoryStreamSlim** avoids memory traffic and prevents LOH fragmentation.

## Features

MemoryStreamSlim is designed to "just work" without requiring extensive configuration or tuning. However, it provides a few options for controlling its behavior:

### Clearing Memory

By default, **MemoryStreamSlim** zeroes out memory buffers when they are released to prevent sensitive data from remaining in memory. This behavior can be customized. [Read More](./memory-management.md#clearing-memory)

### Releasing Memory

**MemoryStreamSlim** automatically releases unused memory buffers on a regular schedule. Additionally, the [`ReleaseMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.ReleaseMemoryBuffers) method allows you to manually release cached buffers when they are no longer needed. [Read More](./memory-management.md#releasing-memory)

### Native Memory

**MemoryStreamSlim** can use native memory instead of the GC heap to further reduce LOH fragmentation. [Read More](./memory-management.md#native-memory)

## Monitoring

**MemoryStreamSlim** supports monitoring through the **.NET Metrics and Events** features, allowing you to track memory usage and performance. [Read More](./memory-monitoring.md)
