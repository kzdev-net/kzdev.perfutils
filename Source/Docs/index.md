---
_layout: landing
---

# PerfUtils

The [`KZDev.PerfUtils`](https://www.nuget.org/packages/KZDev.PerfUtils) package contains the following performance utility classes:

- [`MemoryStreamSlim`](./articles/memorystreamslim.md): A high-performance, memory-efficient, and easy-to-use replacement for the `MemoryStream` class, offering significant performance benefits for large or frequently used streams.
  - Compression helper classes that provide easy compression operations using `MemoryStreamSlim`:
    - [`MemoryStreamBrotli`](xref:KZDev.PerfUtils.MemoryStreamBrotli): Compresses source data using the Brotli algorithm into a **MemoryStreamSlim** instance.
    - [`MemoryStreamDeflate`](xref:KZDev.PerfUtils.MemoryStreamDeflate): Compresses source data using the Deflate algorithm into a **MemoryStreamSlim** instance.
    - [`MemoryStreamGZip`](xref:KZDev.PerfUtils.MemoryStreamGZip): Compresses source data using the GZip algorithm into a **MemoryStreamSlim** instance.
    - [`MemoryStreamZLib`](xref:KZDev.PerfUtils.MemoryStreamZLib): Compresses source data using the ZLib algorithm into a **MemoryStreamSlim** instance.
- [`StringBuilderCache`](./articles/stringbuildercache.md): A thread-safe cache of `StringBuilder` instances to improve speed and reduce the overhead of memory allocations associated with using the **StringBuilder** class.
- [`InterlockedOps`](./articles/interlockedops.md): Provides additional atomic thread-safe operations to extend the functionality of the `Interlocked` class in the .NET Class Library.

See the individual [documentation pages](./articles/getting-started.md) and the [API Reference](xref:KZDev.PerfUtils) for more detailed information.

## Features

### MemoryStreamSlim

`MemoryStreamSlim` is a drop-in replacement for the **MemoryStream** class used for dynamically sized streams, offering the following benefits:

- Better throughput performance compared to the standard **MemoryStream** and the **RecyclableMemoryStream**.
- Significantly lower memory traffic and fewer garbage collections.
- Eliminates Large Object Heap (LOH) fragmentation caused by frequent use and release of various-sized byte arrays.
- Simple replacement for **MemoryStream** with the same API, except for the constructor.
- Optionally uses native memory for storage, minimizing GC pressure.
- Supports monitoring with **.NET Metrics and Events**.
- More efficient compression when used as a destination for compressed data.

### StringBuilderCache

`StringBuilderCache` is a static class that provides a thread-safe cache of **StringBuilder** instances, reducing the number of allocations and deallocations in high-throughput scenarios. Key features include:

- `Acquire`: Retrieves a **StringBuilder** instance from the cache.
- `Release`: Returns a **StringBuilder** instance to the cache.
- `GetStringAndRelease`: Retrieves a string from a **StringBuilder** instance and returns the builder to the cache.
- `GetScope`: Provides a `using` scoped **StringBuilder** instance, automatically returning it to the cache when the scope is exited.
- Supports monitoring with **.NET Events** for detailed cache management.

### InterlockedOps

`InterlockedOps` is a static class providing thread-safe atomic operations, including:

- `Xor`: Performs an exclusive OR operation on any integer type.
- `ClearBits`: Clears bits on any integer type.
- `SetBits`: Sets bits on any integer type.
- `ConditionAnd`: Conditionally updates bits using an AND operation.
- `ConditionOr`: Conditionally updates bits using an OR operation.
- `ConditionXor`: Conditionally updates bits using an XOR operation.
- `ConditionClearBits`: Conditionally clears bits.
- `ConditionSetBits`: Conditionally sets bits.

## Examples

### MemoryStreamSlim Example

Below is an example of how to use the `MemoryStreamSlim` class. Other than instantiation using the `Create` method, the API is identical to the standard **MemoryStream** class:

```csharp
using KZDev.PerfUtils;

// Create a new MemoryStreamSlim instance
// For the best management of the memory buffers, it is very important to
// dispose of the MemoryStreamSlim instance when it is no longer needed.
using (MemoryStreamSlim stream = MemoryStreamSlim.Create())
{
    // Write some data to the stream
    stream.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);

    // Read the data back from the stream
    stream.Position = 0;
    byte[] buffer = new byte[5];
    stream.Read(buffer, 0, 5);
}
```

### Compression Example

Below is an example of using the `MemoryStreamDeflate` class for efficient compression:

```csharp
using KZDev.PerfUtils;

public MemoryStreamSlim CompressData(byte[] data)
{
    return MemoryStreamDeflate.Compress(data);
}
```

### StringBuilderCache Example

Below is an example of using the `StringBuilderCache` class to reduce allocations and deallocations of **StringBuilder** objects in high-throughput scenarios:

```csharp
using KZDev.PerfUtils;

public class StringBuilderExample
{
    public static void GetStringAndRelease ()
    {
        StringBuilder stringBuilder = StringBuilderCache.Acquire();
        stringBuilder.Append("Hello, ");
        stringBuilder.Append("World!");
        Console.WriteLine(StringBuilderCache.GetStringAndRelease(stringBuilder));
    }
}
```


### InterlockedOps Example

This example demonstrates using the `InterlockedOps` class to perform an atomic XOR operation on an integer variable:

```csharp
using KZDev.PerfUtils;

public class XorExample
{
    private int _flag;

    public bool ToggleFlag ()
    {
        int originalValue = InterlockedOps.Xor(ref _flag, 1);
        return originalValue == 0;
    }
}
```

## Compare to RecyclableMemoryStream

The `MemoryStreamSlim` class is similar in concept to the [`RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) class but offers several advantages:

- Easier to use without requiring parameter tuning.
- More flexible across a broad range of use cases.
- Fewer memory allocations and garbage collections.
- Consistent performance across workloads.
- Security-first design with opt-out zeroing of unused memory.
- Automatic memory trimming and release.
- Supports **.NET Metrics and Events** for monitoring.
- Optionally uses native memory to avoid GC pressure.

Unlike **RecyclableMemoryStream**, **MemoryStreamSlim** is specifically designed for dynamically sized memory streams and not as a `Stream` wrapper for existing byte arrays, although that use case is supported. This is covered more in the full [documentation](articles/getting-started.md). Performance comparisons are available in the [Benchmarks](./articles/memorystream-benchmarks.md) section.

## Documentation

Full documentation for the package is available on the [PerfUtils Documentation](articles/getting-started.md) page.

## Future Features

The roadmap for this package includes additional performance-focused utilities, which will be added as time permits.
