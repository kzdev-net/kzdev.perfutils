# PerfUtils Documentation

This documentation is for the `KZDev.PerfUtils` package, which contains the `MemoryStreamSlim` class. The following sections provide insight and information on usage, how the class operates, and information on performance benchmarks. For a full API reference, see the [API Reference](xref:KZDev.PerfUtils).

## MemoryStreamSlim

The standard `MemoryStream` is a class in the .NET Class Library that represents a stream of bytes stored in memory. It is a very useful class for working with in-memory data, but it has some limitations. One of the main limitations is that it uses a single byte array to store the data, which can result in a lot of garbage collection pressure when working with large amounts of memory or cases where many `MemoryStream` instances are created and disposed of frequently.

The [`MemoryStreamSlim`](xref:KZDev.PerfUtils.MemoryStreamSlim) class is specifically tailored to improve performance in cases where using `MemoryStream` can result in a lot of GC pressure but it also has better overall throughput performance than the standard MemoryStream in most use cases.

This documentation provides information on how to use the `MemoryStreamSlim` class, how it works, and how it can help improve performance in your applications.

The [`Memory Stream`](./memorystreamslim.md) topic discusses high-level how to use the `MemoryStreamSlim` class, its features, and how it works internally.

The [`Memory Management`](./memory-management.md) topic provides some insight into how the memory is managed and the options you have to control that management.

The [`Memory Monitoring`](./memory-monitoring.md) topic discusses how you can monitor memory usage and allocations with `MemoryStreamSlim` using the `Metrics` and `Events` features of the .NET runtime.

The [`Benchmarks`](./memorystream-benchmarks.md) topic covers the benchmarks used to examine the performance benefits provided by the `MemoryStreamSlim` class.

## Interlocked Operations

The [`InterlockedOps`](xref:KZDev.PerfUtils.InterlockedOps) class provides a set of static helper methods that provide additional functionality that is not currently available in the `Interlocked` class.

These include operations such as:

* Xor : Exclusive OR operation on any integer types.
* ClearBits : Clear bits on any integer types.
* SetBits : Set bits on any integer types.
* ConditionAnd : Conditionally update bits using an AND operation on any integer types.
* ConditionOr : Conditionally update bits using an OR operation on any integer types.
* ConditionXor : Conditionally update bits using an XOR operation on any integer types.
* ConditionClearBits : Conditionally clear bits on any integer types.
* ConditionSetBits : Conditionally set bits on any integer types.

The [`Interlocked Operations`](./interlockedops.md) topic discusses these operations in more detail.

## Future Features

The roadmap plan for this package is to add several additional helpful performance-focused utilities. These will be forthcoming as time permits.
