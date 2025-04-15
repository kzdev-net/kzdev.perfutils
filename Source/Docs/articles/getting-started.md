# PerfUtils Documentation

This article introduces the `KZDev.PerfUtils` library package, which includes **MemoryStreamSlim**, **StringBuilderCache**, **InterlockedOps**, and a series of compression helper classes. The following sections provide insights into usage, class operations, and performance benchmarks. For a full API reference, see the [API Reference](xref:KZDev.PerfUtils).

## MemoryStreamSlim

The standard `MemoryStream` class in the .NET Class Library represents a stream of bytes stored in memory. While it is convenient for working with in-memory data, it has limitations. One major limitation is its reliance on a single byte array to store data, which can lead to significant garbage collection (GC) pressure when handling large amounts of memory or frequently creating and disposing of `MemoryStream` instances.

The [`MemoryStreamSlim`](xref:KZDev.PerfUtils.MemoryStreamSlim) class is specifically designed to address these limitations. It improves performance in scenarios where MemoryStream would cause high GC pressure and also provides better overall throughput.

Key topics for **MemoryStreamSlim** include:

- The [`Memory Stream`](./memorystreamslim.md) topic, which explains how to use the MemoryStreamSlim class, its features, and its internal workings.
- The [`Memory Management`](./memory-management.md) topic, which provides insights into memory management and options for controlling it.
- The [`Memory Monitoring`](./memory-monitoring.md) topic, which discusses how to monitor memory usage and allocations with MemoryStreamSlim using the .NET Metrics and Events features.
- The [`Benchmarks`](./memorystream-benchmarks.md) topic, which covers performance benchmarks demonstrating the benefits of **MemoryStreamSlim**.

### Compression Helpers

**MemoryStreamSlim** is complemented by compression helper classes that compress data into **MemoryStreamSlim** instances, reducing memory allocation for compressed data. These classes provide a simple interface for compressing and decompressing data.

For more details, see the [Compression](./memorystream-compression.md) topic, which explains how to use these helper classes and their performance benefits.

## StringBuilderCache

The **StringBuilder** class is a mutable string class that is more memory-efficient than repeated string concatenation. However, frequent allocation and deallocation of StringBuilder instances and their internal buffers can cause memory pressure in high-throughput scenarios.

The [`StringBuilderCache`](xref:KZDev.PerfUtils.StringBuilderCache) class is a static, thread-safe cache of StringBuilder instances. It reduces the number of allocations and deallocations, improving performance in scenarios with frequent string manipulations.

Key topics for `StringBuilderCache` include:

- The [`StringBuilderCache`](./stringbuildercache.md) topic, which explains how to use the class and its benefits.
- The [`Benchmarks`](./stringbuildercache-benchmarks.md) topic, which provides performance benchmarks for StringBuilderCache.

## Interlocked Operations

The **Interlocked** class in the .NET Class Library provides atomic operations for thread-safe updates to shared variables. However, its functionality is limited to basic operations.

The [`InterlockedOps`](xref:KZDev.PerfUtils.InterlockedOps) class extends the functionality of **Interlocked** by providing additional atomic operations, including:

- Xor: Performs an exclusive OR operation on any integer type.
- ClearBits: Clears bits on any integer type.
- SetBits: Sets bits on any integer type.
- ConditionAnd: Conditionally updates bits using an AND operation.
- ConditionOr: Conditionally updates bits using an OR operation.
- ConditionXor: Conditionally updates bits using an XOR operation.
- ConditionClearBits: Conditionally clears bits.
- ConditionSetBits: Conditionally sets bits.

For more details, see the [`Interlocked Operations`](./interlockedops.md) topic.

## Future Features

The roadmap for this package includes additional performance-focused utilities, which will be added as time permits.
