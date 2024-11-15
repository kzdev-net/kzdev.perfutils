# CopyToAsync Throughput Benchmark

In-memory-based streams have no asynchronous needs when used in most scenarios because all operations are synchronous and in-memory. However, there are scenarios where the data in the stream needs to be copied to another stream instance with actual asynchronous behavior, such as a FileStream. While this may not be an everyday use case, it is a real-world situation. This is where the `CopyToAsync` method comes into play.
This benchmark scenario uses stream instances that are instantiated as expandable (dynamic growth) streams with an initial capacity set to the operation's data size. The stream is filled with random data, similar to the [`Bulk Fill and Read`](./dynamic-throughput-benchmarks.md#bulk-fill-and-read) scenario.

In each operation, the `CopyToAsync` method copies data from the stream to another stream designed to mock an actual file I/O-based stream with asynchronous behavior.

## Summary 

The benchmark results show that for smaller streams, all the stream classes perform similarly in terms of throughput performance. `MemoryStreamSlim` and RecyclableMemoryStream perform better than MemoryStream in terms of memory allocation performance. However, as the stream size increases, the `MemoryStreamSlim` class performance stays consistent and deterministic, even as the internal chained memory segments slow things down slightly compared to `MemoryStream` to copy the entire stream contents to the destination stream.

Once the stream size approaches 1MB, the `RecyclableMemoryStream` class starts to perform very poorly in terms of throughput performance, and the performance rapidly deteriorates as the stream size increases further. Using the `UseExponentialLargeBuffer` option did not affect the throughput performance in this scenario.

By far, the `MemoryStream` class performs the worst in terms of memory allocation performance in this scenario under all conditions.

_Given that file systems managed by the OS and related drivers employ a series of buffer and caching mechanisms, the [emulation](#asynchronous-stream-emulation) approach used in this benchmark is not a perfect representation of the actual performance of the `CopyToAsync` method in a real-world **local** file-based scenario. However, it does provide a means to compare the performance of the different stream classes in a consistent and deterministic way for asynchronous I/O operations that do incur regular asynchronous latencies such as for **network** based files on file servers, etc._

### Benchmark Operation

A single benchmark operation consists of performing a loop of steps that does the following:

1. Create a new stream instance with a capacity set to the operation data size.
1. Write the test data synchronously to the stream (either in a single write or segmented based on the [BulkInitialFill](#bulkinitialfill) parameter).
1. Call CopyToAsync() on the stream passing a mock asynchronous File I/O stream destination.
1. Dispose of the stream instance.

The [number of loops](./memorystream-benchmarks.md#loop-count-impact) in each operation is determined by the [`DataSize`](#datasize) parameter to keep each benchmark reasonably consistent in duration, but the loop count is always the same for all classes being compared for any given DataSize parameter value.

`MemoryStreamSlim` and `RecyclableMemoryStream` classes are created with the option to zero out memory buffers when they are no longer used disabled to keep the benchmark performance focused on the `CopyToAsync()` call. The `MemoryStream` class has no option to zero out memory buffers (used memory is always cleared - i.e. internal buffers are allocated with `new byte[]`), so this parameter does not apply to that class.

#### Asynchronous Stream Emulation

A note on how the destination stream is used in the `CopyToAsync` call. 

The destination stream used in the `CopyToAsync` call is a simple mock stream that emulates the behavior of an I/O based stream. This is accomplished by using a `MemoryStream` instance internally to manage the stream contents. Then each asynchronous operation on the mock stream class (ReadAsync, WriteAsync, CopyToAsync) is counted and on every 16th operation, an asynchronous delay of 10ms is introduced to simulate the latency of an actual I/O operation. For other call counts evenly divisible by 4, an await is performed on a `Task.Yield()`. 

This is all done to provide a consistent and deterministic performance comparison between the different stream classes specifically for benchmarking and accentuating the impact of the number of internal asynchronous operations performed. Results using different I/O based streams will vary based on the actual I/O performance characteristics of the underlying system.

### Benchmark Parameters

The following parameters were used in the benchmarks. These will appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./memorystream-benchmarks.md#legend).

#### DataSize

The amount of data to write to the stream in each operation loop. The data is a byte array of the specified size.

#### CapacityOnCreate

When `true`, the stream is instantiated with the current loop iteration data size as the initial capacity. When `false`, the stream is created with the default capacity (no initial capacity specified). The results show no notable difference in performance between the two options, but is included in this benchmark to clarify that fact.

#### BulkInitialFill

When `true`, the stream is initially filled with random data in a single bulk write operation. When `false`, the stream is filled with random data in a loop of write operations. The initial stream data fill operation is similar to the operations used in the [Bulk Fill and Read](./dynamic-throughput-benchmarks.md#bulk-fill-and-read) (**BulkInitialFill** is _true_) and [Segmented Fill and Read](./dynamic-throughput-benchmarks.md#segmented-fill-and-read) (**BulkInitialFill** is _false_) benchmarks. The results show no notable difference in performance between the two options, but is included in this benchmark to clarify that fact.

## Benchmark Results

The results of the benchmarks are found in the [`CopyToAsync()`](./MemoryStreamBenchmarks.CopyToAsyncThroughputBenchmarks-report-github.md) benchmark output.

### HTML Report

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in a separate HTML file. 

This can be found [here](./MemoryStreamBenchmarks.CopyToAsyncThroughputBenchmarks-report.html).