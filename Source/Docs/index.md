---
_layout: landing
---

# PerfUtils

The `KZDev.PerfUtils` package contains the [`MemoryStreamSlim`](./articles/memorystreamslim.md) class; a high-performance, memory-efficient, easy-to-use replacement for the `MemoryStream` class that provides particular performance benefits for large or frequently used streams. The package also contains the [`InterlockedOps`](./articles/interlockedops.md) class, which provides additional atomic thread-safe operations to extend the functionality of the `Interlocked` class in the .NET Class Library.

See the individual [documentation pages](./articles/getting-started.md) and the [API Reference](xref:KZDev.PerfUtils) for more detailed information.

## Features

`MemoryStreamSlim` is a drop-in replacement for the `MemoryStream` class used for dynamically sized streams that provides the following benefits:

* Throughput performance is better than the standard `MemoryStream`.
* Much lower memory traffic and far fewer garbage collections than the standard `MemoryStream`.
* Eliminates Large Object Heap (LOH) fragmentation caused by frequent use and release of single-byte arrays used by the standard `MemoryStream`.
* Simple replacement for `MemoryStream` with the same API, other than the constructor.
* Optionally allows using native memory for storage, which allows even more flexibility to minimize GC pressure.

`InterlockedOps` is a static class providing the following thread-safe atomic operations:

* Xor : Exclusive OR operation on any integer types.
* ClearBits : Clear bits on any integer types.
* SetBits : Set bits on any integer types.
* ConditionAnd : Conditionally update bits using an AND operation on any integer types.
* ConditionOr : Conditionally update bits using an OR operation on any integer types.
* ConditionXor : Conditionally update bits using an XOR operation on any integer types.
* ConditionClearBits : Conditionally clear bits on any integer types.
* ConditionSetBits : Conditionally set bits on any integer types.

## InterlockedOps Example

Below is an example of how to use the `InterlockedOps` class to perform an atomic XOR operation on an integer variable. The `Xor` method is used to toggle a bit flag between `1` and `0` in a thread-safe manner and returns a boolean value indicating if the bit flag was set to 1.

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

## MemoryStreamSlim Example

Below is an example of how to use the `MemoryStreamSlim` class. Other than instantiation using the `Create` method, the API is identical to the standard `MemoryStream` class. It is always a best practice to dispose of the `MemoryStreamSlim` instance when it is no longer needed.

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

## Compare to RecyclableMemoryStream

The `MemoryStreamSlim` class is similar in concept and purpose to the [`RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) class from Microsoft however the internal implementation of buffer management is quite different. Also, compared to `RecyclableMemoryStream`, the `MemoryStreamSlim` class is designed to:

* 'Just work' and be easier to use without tuning parameters.
* Be more flexible in most use cases.
* Perform fewer memory allocations.
* Incur fewer garbage collections.
* Perform on par or better in terms of throughput performance.
* Provide more consistent performance across different workloads.
* Treat security as a priority and opt-out rather than opt-in zero'ing unused memory.
* Optionally allow using native memory for storage to avoid GC pressure altogether.

One other important difference is that `MemoryStreamSlim` is specifically designed to be used for dynamically sized memory streams and not as a `Stream` wrapper around existing in-memory byte arrays. `RecyclableMemoryStream` is designed to be used in both scenarios, but that approach can lead to some significant performance issues and non-deterministic behaviors. This is covered more in the full documentation. Performance comparisons are also available in the [Benchmarks](./articles/benchmarks.md) section.

## Documentation

Full documentation for the package is available on the [PerfUtils Documentation](articles/getting-started.md) page.

## Future Features

The roadmap plan for this package is to add a number of additional helpful performance focused utilities. These will be forthcoming as time permits.
