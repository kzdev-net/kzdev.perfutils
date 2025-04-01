# Continuous Growth Throughput Benchmark

This benchmark scenario uses stream instances that are instantiated as expandable (dynamic growth) streams. The stream is created with an initial zero length and capacity, and then filled with data in a loop. The data is then read back in a loop to examine the throughput performance of reading and writing, as well as the memory allocation and garbage collection impact of the different stream classes. The data is written and read in 4 kilobyte segments to simulate a real-world scenario where data is written and read in chunks.

The twist to this scenario is that during the write operations, once the size of the stream reaches 256KB, the buffer from the stream is retrieved as an array of bytes from the stream. In the `MemoryStream` and `MemoryStreamSlim` classes, the `ToArray()` method is used to get the buffer. In the `RecyclableMemoryStream` class, the `GetBuffer()` method is used to get the buffer, since `ToArray()` is marked as obsolete in that class and it is suggested that `GetBuffer()` be used instead. This is a one time operation and is only done at that point in time, otherwise the operation simply continues to write data to the stream and then read it back once the stream gets to the specified size.

It is important to note that there are much better, safer, and performant ways to get the data in to and out of the stream than getting a buffer and using it directly. This is just a scenario to show the performance impact of performing such an operation and continuing to grow the stream after that.

## Summary 

The benchmark results show that `RecyclableMemoryStream` requires the `UseExponentialLargeBuffer` option to be set to `true` to avoid serious throughput performance degradation. It does perform the best in terms of memory allocation with or without the `UseExponentialLargeBuffer` option set to `true`.

The `MemoryStreamSlim` provides the best throughput performance in all cases, and has a consistent and deterministic performance and memory allocation impact across a wide range of scenarios. However, a new array is GC allocated for each call to `ToArray()` which is the reason for the larger memory allocation per operation. As noted earlier, this method is provided for compatibility only and is a known performance hit and should be avoided.

By far, the `MemoryStream` class performs the worst in terms of memory allocation performance in this scenario under all conditions.

### Benchmark Operation

A single benchmark operation consists of performing a loop of steps that does the following:

1. Create a new stream instance.
1. Write test data to the stream.
1. Read data back from the stream.
1. Dispose of the stream instance.

The first loop uses a data size of 0x8_0000 for the writing and reading steps. The data size is then increased by 0x40_0000 for each subsequent loop iteration. The loops continue until the data size reaches at least 0x600_0000, at which point the benchmark operation is complete.

During the write operation, the data is written in 4KB chunks, and once the stream size reaches 256KB, the buffer is retrieved from the stream and stored in a local variable. This is done to simulate a real-world scenario where the buffer is needed for some operation. This capture of the buffer is only done once, and otherwise the operation continues to write data to the stream and then read it back once the stream reaches the specified size.

### Benchmark Parameters

The following parameters were used in the benchmarks. These will appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./memorystream-benchmarks.md#legend).

#### ZeroBuffers

When `true`, the stream is created with the option to zero out memory buffers when they are no longer used. When `false`, the stream is created with the option to not zero out memory buffers specified. For the `MemoryStreamSlim` class, the [`ZeroBufferBehavior`](xref:KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior) option is set to `OnRelease` to provide a fair comparison to the other classes.

The `MemoryStream` class has no option to zero out memory buffers (used memory is always cleared), so this parameter does not apply to that class.

#### ExponentialBufferGrowth

This parameter is only applicable to the `RecyclableMemoryStream` class. When `true`, the `UseExponentialLargeBuffer` option is set to `true` when creating the stream. When `false`, the `UseExponentialLargeBuffer` option is set to `false`. This parameter has no effect on the other stream classes.

## Benchmark Results

The results of the benchmarks are found in the [`Continuous Growth Fill And Read`](./MemoryStreamBenchmarks.ContinuousGrowFillAndReadThroughputBenchmarks-report-github.md) benchmark output.

### HTML Report

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in a separate HTML file. 

This can be found [here](./MemoryStreamBenchmarks.ContinuousGrowFillAndReadThroughputBenchmarks-report.html).