# KZDev.PerfUtils

This repository contains the  ['KZDev.PerfUtils'](https://www.nuget.org/packages/KZDev.PerfUtils) NuGet package, which provides the following high-performance utilities:

- **MemoryStreamSlim**: A memory-efficient, high-performance replacement for the MemoryStream class, offering significant benefits for large or frequently used streams.
- **StringBuilderCache**: A utility for caching StringBuilder instances to improve performance in high-throughput scenarios.
- **InterlockedOps**: A set of additional atomic thread-safe operations that extend the functionality of the Interlocked class in the .NET Class Library.

## Performance Highlights

This sampling of performance benchmarks clearly demonstrates the advantages of using the KZDev.PerfUtils package:

![StringBuilderCache Performance Sample](https://raw.githubusercontent.com/kzdev-net/kzdev.perfutils/refs/heads/main/Source/Docs/images/stringbuilder_sample.jpg)

![MemoryStreamSlim Performance Sample](https://raw.githubusercontent.com/kzdev-net/kzdev.perfutils/refs/heads/main/Source/Docs/images/memorystreamslim_sample.jpg)

For more details, refer to the benchmark related pages in the [documentation](https://kzdev-net.github.io/kzdev.perfutils/).

## Features

### MemoryStreamSlim

`MemoryStreamSlim` is a drop-in replacement for the **MemoryStream** class, offering the following advantages:

- **Improved Throughput**: Outperforms the standard MemoryStream in terms of throughput.
- **Reduced Memory Traffic**: Significantly lowers memory traffic and garbage collection compared to the standard MemoryStream.
- **Eliminates LOH Fragmentation**: Prevents Large Object Heap (LOH) fragmentation caused by frequent use and release of single-byte arrays.
- **API Compatibility**: Provides the same API as MemoryStream, with minor differences in the constructor.
- **Optional Native Memory Storage**: Allows the use of native memory for storage, further reducing GC pressure and increasing flexibility.

### StringBuilderCache

`StringBuilderCache` is a static class that provides a thread-safe cache of StringBuilder instances, reducing allocations and deallocations in high-throughput scenarios. Key features include:

- **Acquire**: Retrieve a StringBuilder instance from the cache.
- **Release**: Return a StringBuilder instance to the cache.
- **GetStringAndRelease**: Retrieve the string from a StringBuilder instance and return it to the cache.
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

## Documentation

Comprehensive documentation for the package is available on the [PerfUtils Documentation](https://kzdev-net.github.io/kzdev.perfutils/) page.

## Future Features

The roadmap for this package includes plans to add additional performance-focused utilities as time allows.

## Contribution Guidelines

At this time, external pull requests are not being accepted. However, feedback and suggestions are welcome through the following channels:

- **Feature Requests**: Use GitHub Discussions to propose new features or enhancements. This ensures alignment with the project's goals and vision before opening a feature request.
- **Bug Reports**: If you encounter any issues, please open an issue on GitHub to report them.

Your understanding is appreciated, and I look forward to collaborating with you through discussions and issue tracking.
