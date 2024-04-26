# Set Loop Count Throughput Benchmark

The benchmarks provided on this site loop through a set of steps for each benchmark `operation`. The number of times these steps are repeated is called the `loop count`. For all the other benchmarks, this loop count is determined by the current `DataSize` parameter value. This means the loop count is the same for all benchmark runs for the same `DataSize` value, and is different for different `DataSize` parameter values.

This is meant to keep the run time of each operation within a reasonable range, regardless of the size of the data being processed. When reviewing the results, the benchmarks should then be partitioned into groups of operations that are run with different `DataSize` values when comparing the performance of the different stream classes.

This makes it difficult, though, to compare the performance of the different stream classes give different data size values with a consistent `operation`. This benchmark is meant to provide a consistent `operation` that can be used to compare the performance of the different stream classes with different data size values.

For this benchmark, the loop count is set to a fixed value of 50 for each operation. This provides a means to compare the classes' relative performance given the exact same operations but with different data sizes.

## Summary 

The benchmark results show that the `MemoryStreamSlim` and `RecyclableMemoryStream` classes perform similarly in terms of throughput performance and better than the `MemoryStream` class in terms of throughput and memory allocation performance. Given how the memory is managed internally for both classes, it is unsurprising that the throughput performance gains vs. `MemoryStream` are not as significant for very large streams as for small or moderately sized streams. However, they both still perform far better than `MemoryStream`.

This also shows that the memory allocations with the `MemoryStreamSlim` class remain almost constant and minimal while GCs are virtually avoided for all data sizes. In constrast the memory allocations and GCs for the `RecyclableMemoryStream` class increase substantially as the data size increases, and `MemoryStream` memory allocations increase at a much higher rate still than the other two classes. Using the `UseExponentialLargeBuffer` option for `RecyclableMemoryStream` had no effect on these results.

### Benchmark Operation

A single benchmark operation consists of performing a loop of steps that does the following:

1. Create a new stream instance.
1. Write test data to the stream.
1. Read data back from the stream.
1. Dispose of the stream instance.

Other than a set loop count and the parameters listed below, the operation is identical to the [`Segmented Fill and Read`](./dynamic-throughput-benchmarks.md#segmented-fill-and-read) with `GrowEachLoop` and `CapacityOnCreate` both set to `false`.

### Benchmark Parameters

The following parameters were used in the benchmarks. These will appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./benchmarks.md#legend).

#### DataSize

The amount of data to write to the stream in each loop of the operation. The data is a byte array of the specified size.

#### ZeroBuffers

When `true`, the stream is created with the option to zero out memory buffers when they are no longer used. When `false`, the stream is created with the option to not zero out memory buffers specified. For the `MemoryStreamSlim` class, the [`ZeroBufferBehavior`](/api/KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior.html) option is set to `OnRelease` to provide a fair comparison to the other classes.

The `MemoryStream` class has no option to zero out memory buffers (used memory is always cleared), so this parameter does not apply to that class.

## Benchmark Results

The results of the benchmarks are found in the [`Set Loop Count`](./MemoryStreamBenchmarks.SetLoopCountFillAndReadThroughputBenchmarks-report-github.md) benchmark output.

### HTML Report

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in a separate HTML file. 

This can be found [here](./MemoryStreamBenchmarks.SetLoopCountFillAndReadThroughputBenchmarks-report.html).