# Continuous Growth Throughput Benchmark

This benchmark scenario evaluates stream instances that are instantiated as expandable (dynamically growing) streams. The stream is created with an initial zero length and capacity, and then filled with data in a loop. The data is subsequently read back in a loop to examine the throughput performance of reading and writing, as well as the memory allocation and garbage collection impact of the different stream classes. The data is written and read in 4-kilobyte segments to simulate a real-world scenario where data is processed in chunks.

A unique aspect of this scenario is that during the write operations, once the size of the stream reaches 256KB, the stream’s content is materialized into a contiguous buffer and observed so the cost of that operation is included in the measurement. **MemoryStream** uses [`ToArray()`](xref:System.IO.MemoryStream.ToArray), which allocates a new `byte[]` each time. **MemoryStreamSlim** uses [`ToMemory()`](xref:KZDev.PerfUtils.MemoryStreamSlim.ToMemory), which rents a buffer from [`MemoryPool<byte>.Shared`](xref:System.Buffers.MemoryPool`1.Shared); the returned [`IMemoryOwner<byte>`](xref:System.Buffers.IMemoryOwner`1) is disposed immediately in the same write iteration before writing continues, returning the rental to the pool. **RecyclableMemoryStream** uses **GetBuffer()** instead, because **ToArray()** is obsolete on that type and **GetBuffer()** is the documented alternative. This capture is a one-time operation at that point in the write loop; afterward, the benchmark continues to grow the stream and then reads it back.

It is important to note that there are safer, more efficient, and performant ways to retrieve data from a stream than directly accessing its buffer or exporting a snapshot copy of the contents. This scenario is designed to demonstrate the performance impact of performing such an operation and continuing to grow the stream afterward.

## Summary

With **MemoryStreamSlim** measured using [`ToMemory()`](xref:KZDev.PerfUtils.MemoryStreamSlim.ToMemory) (pool-backed export), the published results show **MemoryStreamSlim** ahead of both **MemoryStream** and **RecyclableMemoryStream** on **mean execution time** and on **reported allocation** for every parameter combination in this scenario.

**RecyclableMemoryStream** still benefits from enabling **UseExponentialLargeBuffer**: when it is **false**, throughput can degrade sharply relative to the baseline **MemoryStream** (see the published tables). With exponential large buffers enabled, **RecyclableMemoryStream** improves on throughput but remains slower than **MemoryStreamSlim** in these results and reports higher **Allocated** values than **MemoryStreamSlim**.

**MemoryStream** remains the worst for allocation in this scenario because each loop iteration uses **ToArray()**, which allocates a full copy on the GC heap at the 256KB capture point while the stream continues to grow to much larger sizes.

This scenario is intentionally stressful: it combines mid-stream materialization with continued growth. It does not replace general guidance—prefer [`TryGetBuffer`](xref:System.IO.MemoryStream.TryGetBuffer*) or span-friendly patterns when a single contiguous backing array is already available and safe to expose—but it highlights how **ToMemory** lets **MemoryStreamSlim** offer a pool-disciplined contiguous snapshot compared with **ToArray** on **MemoryStream**. For API and lifecycle details, see [Exporting contiguous bytes](./memorystreamslim.md#exporting-contiguous-bytes-toarray-and-tomemory) in the **MemoryStreamSlim** article.

## Benchmark Operation

A single benchmark operation consists of performing a loop of steps that includes the following:

1. Create a new stream instance.
1. Write test data to the stream.
1. Read data back from the stream.
1. Dispose of the stream instance.

The first loop uses a data size of 0x8_0000 (512KB) for the writing and reading steps. The data size is then increased by 0x40_0000 (4MB) for each subsequent loop iteration. The loops continue until the data size reaches at least 0x600_0000 (96MB), at which point the benchmark operation is complete.

During the write operation, the data is written in 4KB chunks. Once the stream size reaches 256KB, the buffer is retrieved from the stream and stored in a local variable. This simulates a real-world scenario where the buffer is needed for some operation. This buffer retrieval is performed only once; the operation then continues to write data to the stream and read it back once the stream reaches the specified size.

## Benchmark Parameters

The following parameters were used in the benchmarks. These appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./memorystream-benchmarks.md#legend).

### ZeroBuffers

- **true**: The stream is created with the option to zero out memory buffers when they are no longer used.
- **false**: The stream is created without zeroing out memory buffers.

For the **MemoryStreamSlim** class, when this parameter is **true** the [`ZeroBufferBehavior`](xref:KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior) option is set to **OnRelease** to provide a fair comparison to the other classes (vs. **OutOfBand**). The **MemoryStream** class does not support zeroing out memory buffers (used memory is always cleared), so this parameter does not apply to that class.

### ExponentialBufferGrowth

This parameter is applicable only to the RecyclableMemoryStream class.
- **true**: The **UseExponentialLargeBuffer** option is set to true when creating the stream.
- **false**: The **UseExponentialLargeBuffer** option is set to false.

## Benchmark Results

The results of the benchmarks are available in the [`Continuous Growth Fill And Read`](./MemoryStreamBenchmarks.ContinuousGrowFillAndReadThroughputBenchmarks-report-github.md) benchmark output.

### HTML Report

Since the benchmark results can create large tables that may be difficult to navigate due to horizontal and vertical scrolling, the results are also provided in a simpler HTML table format.

The HTML report can be found [here](./MemoryStreamBenchmarks.ContinuousGrowFillAndReadThroughputBenchmarks-report.html).
