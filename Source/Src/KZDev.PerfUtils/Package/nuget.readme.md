# KZDev.PerfUtils

The KZDev.PerfUtils package provides the following high-performance utility classes:

- `MemoryStreamSlim`: A high-performance, memory-efficient, and easy-to-use replacement for the **MemoryStream** class, offering significant performance benefits for large or frequently used streams.
- `StringBuilderCache`: A thread-safe cache of **StringBuilder** instances to improve speed and reduce the overhead of memory allocations associated with the **StringBuilder** class.
- `InterlockedOps`: A utility that extends the functionality of the **Interlocked** class in the .NET Class Library by providing additional atomic thread-safe operations.

## Performance Highlights

This sampling of performance benchmarks clearly demonstrates the advantages of using the KZDev.PerfUtils package:

![StringBuilderCache Performance Sample](https://raw.githubusercontent.com/kzdev-net/kzdev.perfutils/refs/heads/main/Source/Src/KZDev.PerfUtils/Package/v2.0/images/stringbuilder_sample.jpg)

![MemoryStreamSlim Performance Sample](https://raw.githubusercontent.com/kzdev-net/kzdev.perfutils/refs/heads/main/Source/Src/KZDev.PerfUtils/Package/v2.0/images/memorystreamslim_sample.jpg)

For more details, refer to the benchmark related pages in the [documentation](https://kzdev-net.github.io/kzdev.perfutils/articles/getting-started.html).

## Features

### MemoryStreamSlim

`MemoryStreamSlim` is a drop-in replacement for the **MemoryStream** class, offering the following benefits:

- **Improved Throughput**: Outperforms the standard **MemoryStream**.
- **Reduced Memory Traffic**: Significantly lowers memory traffic and garbage collection compared to the standard **MemoryStream**.
- **Eliminates LOH Fragmentation**: Prevents Large Object Heap (LOH) fragmentation caused by frequent use and release of single-byte arrays.
- **API Compatibility**: Provides the same API as **MemoryStream**, with minor differences in the constructor.
- **Optional Native Memory Storage**: Allows the use of native memory for storage, further reducing GC pressure and increasing flexibility.
- **Outperforms Similar Libraries**: Compared to other libraries like **RecyclableMemoryStream**, it is designed to be easier to use, more flexible, and incurs fewer memory allocations and garbage collections.

### StringBuilderCache

`StringBuilderCache` is a static class that provides a thread-safe cache of **StringBuilder** instances, reducing allocations and deallocations in high-throughput scenarios. Key features include:
- **Acquire**: Retrieve a **StringBuilder** instance from the cache.
- **Release**: Return a **StringBuilder** instance to the cache.
- **GetStringAndRelease**: Retrieve the string from a **StringBuilder** instance and return it to the cache.
- **GetScope**: Use a using-scoped StringBuilder instance, which is automatically returned to the cache when the scope is exited.
- **Monitoring**: Leverages the .NET runtime's Events feature for detailed cache management and monitoring.

### InterlockedOps

`InterlockedOps` is a static class that provides additional thread-safe atomic operations for integer types, including:

- **Xor**: Perform an exclusive OR operation.
- **ClearBits**: Clear specific bits.
- **SetBits**: Set specific bits.
- **ConditionAnd**: Conditionally update bits using an AND operation.
- **ConditionOr**: Conditionally update bits using an OR operation.
- **ConditionXor**: Conditionally update bits using an XOR operation.
- **ConditionClearBits**: Conditionally clear specific bits.
- **ConditionSetBits**: Conditionally set specific bits.

## Examples

### MemoryStreamSlim Example

Below is an example of how to use the `MemoryStreamSlim` class. Other than instantiation using the **Create** method, the API is identical to the standard **MemoryStream** class. Note that it is always a best practice to dispose of the **MemoryStreamSlim** instance when it is no longer needed.

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

### StringBuilderCache Example

Below is an example of how to use the `StringBuilderCache` class to acquire a **StringBuilder** instance, append strings to it, and then release it back to the cache. The **GetStringAndRelease** method is used to retrieve the string and release the **StringBuilder** instance back to the cache.

```csharp
using KZDev.PerfUtils;

class Program
{
    static void Main()
    {
        StringBuilder stringBuilder = StringBuilderCache.Acquire();
        stringBuilder.Append("Hello, ");
        stringBuilder.Append("World!");
        Console.WriteLine(StringBuilderCache.GetStringAndRelease(stringBuilder));
    }
}
```

### InterlockedOps Example

Below is an example of how to use the `InterlockedOps` class to perform an atomic **XOR** operation on an integer variable. The **Xor** method toggles a bit flag between 1 and 0 in a thread-safe manner and returns a boolean value indicating whether the bit flag was set to 1.

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

The `MemoryStreamSlim` class is conceptually similar to the [`RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) class from Microsoft. However, the internal implementation of buffer management is quite different. Compared to **RecyclableMemoryStream**, the **MemoryStreamSlim** class is designed to:

- Easier to use without requiring parameter tuning.
- More flexible across a broad range of use cases.
- Fewer memory allocations and garbage collections.
- Consistent performance across workloads.
- Security-first design with opt-out zeroing of unused memory.
- Automatic memory trimming and release.
- Supports **.NET Metrics and Events** for monitoring.
- Optionally uses native memory to avoid GC pressure.

Performance comparisons are also available in the [benchmarks](https://kzdev-net.github.io/kzdev.perfutils/articles/memorystream-benchmarks.html) documentation section.

## Documentation

Comprehensive documentation for the package is available on the [PerfUtils Documentation](https://kzdev-net.github.io/kzdev.perfutils/) page.

## Future Features

The roadmap for this package includes plans to add additional performance-focused utilities as time permits.
