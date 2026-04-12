# KZDev.PerfUtils

[![Build / Test](https://github.com/kzdev-net/kzdev.perfutils/actions/workflows/ci.yml/badge.svg)](https://github.com/kzdev-net/kzdev.perfutils/actions/workflows/ci.yml)

This repository contains the  ['KZDev.PerfUtils'](https://www.nuget.org/packages/KZDev.PerfUtils) NuGet package, which provides the following high-performance utilities:

- **MemoryStreamSlim**: A memory-efficient, high-performance stream for large or frequently resized in-memory buffers, modeled on **MemoryStream** with a largely compatible API.
- **StringBuilderCache**: A utility for caching StringBuilder instances to improve performance in high-throughput scenarios.
- **InterlockedOps**: A set of additional atomic thread-safe operations that extend the functionality of the Interlocked class in the .NET Class Library.

## Supported target frameworks

This package targets **.NET 8** (`net8.0`), **.NET 9** (`net9.0`), and **.NET 10** (`net10.0`). The 3.x line does not ship builds for older TFMs (including **.NET 6**); upgrade consuming projects to a supported target framework before referencing the latest package versions.

## Performance Highlights

This sampling of performance benchmarks clearly demonstrates the advantages of using the KZDev.PerfUtils package:

![StringBuilderCache Performance Sample](https://raw.githubusercontent.com/kzdev-net/kzdev.perfutils/main/Source/Src/KZDev.PerfUtils/Package/v2.0/images/stringbuilder_sample.jpg)

![MemoryStreamSlim Performance Sample](https://raw.githubusercontent.com/kzdev-net/kzdev.perfutils/main/Source/Src/KZDev.PerfUtils/Package/v2.0/images/memorystreamslim_sample.jpg)

For more details, refer to the benchmark related pages in the [documentation](https://kzdev-net.github.io/kzdev.perfutils/).

## Features

### MemoryStreamSlim

`MemoryStreamSlim` targets workloads where **MemoryStream** limits throughput or drives high GC cost. It is **largely API-compatible** with **MemoryStream**, so many call sites can switch the concrete type with small edits, but it is **not** guaranteed to match **MemoryStream** in every behavioral edge case. Construct instances with **`MemoryStreamSlim.Create`** (and related overloads) instead of **`new MemoryStream`**. After **Dispose**, **Length**, **Position**, and capacity-related members follow the same **ObjectDisposedException** rules as **MemoryStream**; do not rely on reading those values from a disposed stream.

- **Improved Throughput**: Outperforms the standard MemoryStream in terms of throughput.
- **Reduced Memory Traffic**: Significantly lowers memory traffic and garbage collection compared to the standard MemoryStream.
- **Eliminates LOH Fragmentation**: Prevents Large Object Heap (LOH) fragmentation caused by frequent use and release of single-byte arrays.
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
