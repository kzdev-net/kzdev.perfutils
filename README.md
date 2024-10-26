# KZDev.PerfUtils

This is the repository for the ['KZDev.PerfUtils'](https://www.nuget.org/packages/KZDev.PerfUtils) nuget package that contains the `MemoryStreamSlim` class; a high-performance, memory-efficient, and easy-to-use replacement for the `MemoryStream` class that provides particular benefits for large or frequently used streams. As well as the `InterlockedOps` class, which provides additional atomic thread-safe operations to extend the functionality of the `Interlocked` class in the .NET Class Library.

## Features

`MemoryStreamSlim` is a drop-in replacement for the `MemoryStream` class that provides the following benefits:

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

## Documentation

Full documentation for the package is available on the [PerfUtils Documentation](https://kzdev-net.github.io/kzdev.perfutils/) page.

## Future Features

The roadmap plan for this package is to add several additional helpful performance focused utilities as time allows.

## Contribution Guidelines

At this time, I am not accepting external pull requests. However, any feedback or suggestions are welcome and can be provided through the following channels:

- **Feature Requests:** Please use GitHub Discussions to discuss new features or enhancements before opening a feature request. This will help ensure that your request is in line with the project's goals and vision.
- **Bug Reports:** If you encounter any issues, feel free to open an issue so it can be addressed promptly.

I appreciate your understanding and look forward to collaborating with you through discussions and issue tracking.