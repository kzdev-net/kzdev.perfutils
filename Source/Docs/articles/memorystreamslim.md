# MemoryStreamSlim

The .NET class library provides a **MemoryStream** class representing a stream of bytes stored in memory. It operates in one of two implied modes: expandable (dynamic) or fixed, determined by how you instantiate the **MemoryStream** instance. In expandable mode, the **MemoryStream** class uses a single byte array to store the data, resizing the array as needed to accommodate changes in length and capacity. In fixed mode, the **MemoryStream** class uses a fixed-size byte array provided during instantiation.

The [`MemoryStream`](xref:System.IO.MemoryStream) class is optimized for fixed mode, providing `Stream` semantics to a pre-allocated byte array. However, in expandable mode, the MemoryStream class can result in poor performance and significant garbage collection (GC) pressure when working with large amounts of memory, frequently creating and disposing of MemoryStream instances, or dynamically growing streams.

Microsoft released the [`RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) package to address many of the issues associated with the expandable mode of the **MemoryStream** class. However, the **RecyclableMemoryStream** package has limitations, including the need for careful tuning of configuration options and consistent usage patterns to achieve optimal performance. In some scenarios, it can perform worse than the standard **MemoryStream** class. Specifically, **RecyclableMemoryStream** attempts to support both expandable and fixed modes, but it often exhibits worse throughput performance than **MemoryStream** in fixed-mode scenarios.

[`MemoryStreamSlim`](xref:KZDev.PerfUtils.MemoryStreamSlim) is a high-performance, memory-efficient alternative to the **MemoryStream** class. It is specifically designed for scenarios where performance and memory management are critical, particularly for expandable memory-based streams that are large, frequently used, or have varying use cases. **MemoryStreamSlim** focuses on improving performance in expandable mode use cases, while leaving fixed-mode scenarios to the standard **MemoryStream** class. By concentrating on what needs improvement, **MemoryStreamSlim** avoids unnecessary complexity and delivers targeted optimizations for expandable mode. This is discussed in more detail below.

## Memory Traffic

Even relatively small memory streams can generate significant memory traffic (churn) by frequently allocating and releasing memory buffers, leading to performance issues and GC pressure.

Larger memory streams can exacerbate these issues by creating allocations on the [Large Object Heap (LOH)](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap). The LOH is used to store large objects (85,000 bytes or larger by default) and is typically not compacted by the garbage collector, which can lead to fragmentation and performance degradation over time.

**MemoryStreamSlim** minimizes memory traffic by reusing memory buffers. It uses a specialized, internally managed pool of small, heap-allocated reusable buffers for smaller memory needs. For larger buffers, it allocates groups of segments from the LOH and reuses them as needed, avoiding LOH fragmentation. This approach reduces allocations and deallocations, prevents LOH fragmentation, and improves performance by reducing GC pressure.

When a **MemoryStreamSlim** instance is created with an existing byte array, it acts as a wrapper around the standard **MemoryStream** class, as no additional memory management benefits can be provided in this case. For scenarios requiring `Stream` semantics for pre-allocated byte arrays, the standard **MemoryStream** class is recommended, though the overhead of the **MemoryStreamSlim** wrapper is negligible.

## Usage

To create a `MemoryStreamSlim` instance, use one of the [`Create`](xref:KZDev.PerfUtils.MemoryStreamSlim.Create) static overload methods. These methods allow you to specify an initial capacity, configure options for the stream instance, or provide an existing byte array to wrap.

Once created, **MemoryStreamSlim** can be used as a drop-in replacement for the standard **MemoryStream** class in most scenarios. It implements all current methods and properties of **MemoryStream**, except for deprecated or outdated ones. For information about behavioral differences, particularly regarding disposed instances, see the [Disposed Behavior Compatibility](#disposed-behavior-compatibility) section below.

```csharp
using KZDev.PerfUtils;

// Create a new MemoryStreamSlim instance with an initial capacity of 1024 bytes, and setting the option to not clear memory buffers
using (Stream stream = MemoryStreamSlim.Create(1024, options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None))
{
    // Read and Write stream operations...
}
```

## Decoding Stream Contents

The [`Decode`](xref:KZDev.PerfUtils.MemoryStreamSlim.Decode(System.Text.Encoding)) method provides a convenient way to decode the entire stream contents to a string using a specific encoding:

```csharp
using KZDev.PerfUtils;
using System.Text;

using (MemoryStreamSlim stream = MemoryStreamSlim.Create())
{
    // Write some data to the stream
    byte[] data = Encoding.UTF8.GetBytes("Hello, World!");
    stream.Write(data, 0, data.Length);
    
    // Decode the stream contents back to a string
    stream.Position = 0;
    string result = stream.Decode(Encoding.UTF8);
    Console.WriteLine(result); // Output: "Hello, World!"
}
```

> **Note:** The `Decode` method reads the entire stream regardless of the current position and may allocate a temporary buffer for large streams.

## Key Difference: GetBuffer Method

The primary behavioral difference between **MemoryStreamSlim** and **MemoryStream** is the implementation of the [GetBuffer](xref:KZDev.PerfUtils.MemoryStreamSlim.GetBuffer*) method:

- In fixed mode, `GetBuffer` behaves like **MemoryStream**, returning the underlying byte array if the instance was created with the `publiclyVisible` parameter set to `true` (using the `Create(byte[] buffer, int index, int count, bool writable, bool publiclyVisible)` overload). Otherwise, it throws an exception.
- In expandable mode, `GetBuffer` always throws a `NotSupportedException`. This design prevents misuse in scenarios where the underlying buffer is not guaranteed to be contiguous in memory. Safer and more efficient methods are available for accessing stream data.

In contrast, **RecyclableMemoryStream** supports `GetBuffer` and recommends it over `ToArray` for performance reasons. However, its documentation warns about potential performance implications and the need for careful use. Additionally, the initial call to `GetBuffer` can significantly degrade the performance of the **RecyclableMemoryStream** instance from that point on.

## Disposed Behavior Compatibility

**MemoryStreamSlim** maintains compatibility with **MemoryStream** for most operations after disposal, with one important exception related to memory efficiency:

### Properties That Throw After Disposal

The following properties throw `ObjectDisposedException` when accessed after disposal, matching **MemoryStream** behavior:

- [`Length`](xref:System.IO.MemoryStream.Length) - Throws when accessed after disposal
- [`Position`](xref:System.IO.MemoryStream.Position) - Throws when accessed after disposal
- [`Capacity`](xref:KZDev.PerfUtils.MemoryStreamSlim.Capacity) - Throws when accessed after disposal
- [`CapacityLong`](xref:KZDev.PerfUtils.MemoryStreamSlim.CapacityLong) - Throws when accessed after disposal

### ToArray Method Behavior

The behavior of the [`ToArray`](xref:System.IO.MemoryStream.ToArray) method after disposal differs based on the stream mode:

- **Fixed Mode**: `ToArray()` works after disposal, matching **MemoryStream** behavior. This is because fixed mode streams wrap a standard **MemoryStream** instance, which keeps its buffer after disposal.

- **Dynamic Mode**: `ToArray()` throws `ObjectDisposedException` after disposal. This is an intentional design difference necessary for memory efficiency. Unlike **MemoryStream**, which keeps its buffer after disposal, **MemoryStreamSlim** releases buffers back to the pool during disposal to minimize memory usage. Once buffers are released, the data is no longer available, so `ToArray()` cannot work.

This difference ensures that **MemoryStreamSlim** can efficiently manage memory by releasing buffers when they are no longer needed, rather than keeping them allocated indefinitely like **MemoryStream** does.

### ToMemory Method

The [`ToMemory`](xref:KZDev.PerfUtils.MemoryStreamSlim.ToMemory*) overloads copy the stream’s visible content into a contiguous buffer exposed as an [`IMemoryOwner<byte>`](xref:System.Buffers.IMemoryOwner`1). They are intended for callers who want `Memory<byte>`-friendly access while preferring a **pool-backed** rental instead of a new GC heap array from [`ToArray`](xref:System.IO.MemoryStream.ToArray).

- **What you get:** A snapshot from the start of the stream through [`Length`](xref:System.IO.Stream.Length), independent of [`Position`](xref:System.IO.Stream.Position), with the same observable size limits as `ToArray`.
- **Empty streams:** When `Length` is zero, the returned owner is a **shared singleton** with zero-length [`Memory`](xref:System.Buffers.IMemoryOwner`1.Memory). Its [`Dispose`](xref:System.IDisposable.Dispose) is an idempotent no-op and **does not** rent from any [`MemoryPool<byte>`](xref:System.Buffers.MemoryPool`1).
- **Non-empty streams:** The implementation rents from [`MemoryPool<byte>.Shared`](xref:System.Buffers.MemoryPool`1.Shared) (parameterless overload) or from the [`MemoryPool<byte>`](xref:System.Buffers.MemoryPool`1) you pass in. You **must** dispose the owner when finished so the buffer returns to **that** pool. The owner’s `Memory` span is exactly `Length` bytes—the pool may have allocated a larger backing array, but that trailing space is **not** visible through `Memory`.
- **Disposed streams:** Behavior matches `ToArray` by mode: **fixed mode** can still succeed after disposal when the wrapped **MemoryStream** still exposes the buffer; **dynamic mode** throws `ObjectDisposedException` because buffers were released at dispose.

#### When to prefer ToMemory vs ToArray

| | **ToArray()** | **ToMemory()** |
| --- | --- | --- |
| Allocation | New `byte[]` on the GC heap | Rented buffer from a `MemoryPool<byte>` |
| Lifetime | Array becomes eligible for GC when unreferenced | Return memory with `Dispose()` on the owner |
| API shape | `byte[]` | `IMemoryOwner<byte>` → `Memory<byte>` |

Use `ToArray` when a simple heap array is enough. Use `ToMemory` when you want to pair contiguous bytes with pool discipline, custom pools, or `Memory<byte>`-based APIs—always dispose non-empty owners.

```csharp
using KZDev.PerfUtils;
using System;
using System.Buffers;

using (MemoryStreamSlim stream = MemoryStreamSlim.Create())
{
    stream.WriteByte(1);
    stream.WriteByte(2);
    using IMemoryOwner<byte> owner = stream.ToMemory();

    ReadOnlySpan<byte> span = owner.Memory.Span;
    // Use span ...
}
```

### GetBuffer and TryGetBuffer Methods

The behavior of [`GetBuffer`](xref:KZDev.PerfUtils.MemoryStreamSlim.GetBuffer*) and [`TryGetBuffer`](xref:System.IO.MemoryStream.TryGetBuffer*) after disposal matches **MemoryStream**:

- **Fixed Mode with publiclyVisible=true**: Both methods work after disposal, returning the underlying buffer.
- **Fixed Mode with publiclyVisible=false**: `GetBuffer()` throws `UnauthorizedAccessException`, and `TryGetBuffer()` returns `false`.
- **Dynamic Mode**: `GetBuffer()` throws `NotSupportedException` (regardless of disposal state), and `TryGetBuffer()` returns `false`.

## How MemoryStreamSlim Works

`MemoryStreamSlim` achieves high efficiency in memory usage and throughput performance by reusing memory buffers. It divides memory needs into small and large buffers, which are internally chained to create a logically contiguous memory stream.

- Small Buffers: For smaller memory capacities, **MemoryStreamSlim** uses a pool of small, reusable buffers. These buffers are GC-allocated but cached for reuse, reducing allocation overhead.
- Large Buffers: For larger memory capacities, **MemoryStreamSlim** allocates multiples of 64KB segments from the LOH. These segments are linked together to create efficient memory streams of any size. By caching these buffers, **MemoryStreamSlim** avoids memory traffic and prevents LOH fragmentation.

## Features

MemoryStreamSlim is designed to "just work" without requiring extensive configuration or tuning. However, it provides a few options for controlling its behavior:

### Clearing Memory

By default, **MemoryStreamSlim** zeroes out memory buffers when they are released to prevent sensitive data from remaining in memory. This behavior can be customized. [Read More](./memory-management.md#clearing-memory)

### Releasing Memory

**MemoryStreamSlim** automatically releases memory buffers that have been unused for an extended period on a regular schedule. Additionally, you can manually release cached buffers when they are no longer needed by calling the [`ReleaseMemoryBuffers`](xref:KZDev.PerfUtils.MemoryStreamSlim.ReleaseMemoryBuffers) method. [Learn more](./memory-management.md#releasing-memory)

### Native Memory

**MemoryStreamSlim** can use native memory instead of the GC heap to further reduce LOH fragmentation. [Read More](./memory-management.md#native-memory)

## Monitoring

**MemoryStreamSlim** supports monitoring through the **.NET Metrics and Events** features, allowing you to track memory usage and performance. [Read More](./memory-monitoring.md)
