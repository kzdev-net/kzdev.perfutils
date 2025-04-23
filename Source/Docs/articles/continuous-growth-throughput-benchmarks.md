# Continuous Growth Throughput Benchmark

This benchmark scenario evaluates stream instances that are instantiated as expandable (dynamically growing) streams. The stream is created with an initial zero length and capacity, and then filled with data in a loop. The data is subsequently read back in a loop to examine the throughput performance of reading and writing, as well as the memory allocation and garbage collection impact of the different stream classes. The data is written and read in 4-kilobyte segments to simulate a real-world scenario where data is processed in chunks.

A unique aspect of this scenario is that during the write operations, once the size of the stream reaches 256KB, the buffer from the stream is retrieved as an array of bytes. For the **MemoryStream** and `MemoryStreamSlim` classes, the **ToArray()** method is used to retrieve the buffer. For the **RecyclableMemoryStream** class, the **GetBuffer()** method is used instead, as **ToArray()** is marked as obsolete in that class, and **GetBuffer()** is the recommended alternative. This buffer retrieval is a one-time operation performed at that point in time. Afterward, the operation continues to write data to the stream and read it back once the stream reaches the specified size.

It is important to note that there are safer, more efficient, and performant ways to retrieve data from a stream than directly accessing its buffer. This scenario is designed to demonstrate the performance impact of performing such an operation and continuing to grow the stream afterward.

## Summary 

The benchmark results show that **RecyclableMemoryStream** requires the **UseExponentialLargeBuffer** option to be set to true to avoid significant throughput performance degradation. It performs the best in terms of memory allocation, regardless of whether the **UseExponentialLargeBuffer** option is enabled.

The **MemoryStreamSlim** class provides the best throughput performance in all cases and exhibits consistent and deterministic performance and memory allocation behavior across a wide range of scenarios. However, a new array is allocated on the heap for each call to **ToArray()**, which results in higher memory allocation per operation. As noted earlier, this method is provided for compatibility only, and its performance impact is well-known, making it unsuitable for high-performance scenarios.

The **MemoryStream** class performs the worst in terms of memory allocation performance under all conditions.

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
