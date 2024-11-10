# Dynamic Growth Throughput Benchmarks

The two scenarios in this category use a dynamic expandable stream that is instantiated with an initial zero length and capacity. The stream is then filled with data in a loop, and the data is read back in a loop to examing the throughput performance of reading and writing as well as the memory allocation and garbage collection impact of the different stream classes.

## Summary 

The benchmark results show that `MemoryStreamSlim` consistently allocates less memory than the other classes, and the throughput performance is on par with, or better than, `RecyclableMemoryStream`. There are some use cases where `MemoryStreamSlim` performs dramatically better. While those are generally edge cases, it does show that `MemoryStreamSlim` performance is more consistent and deterministic across a wide range of scenarios.

For security senstive applications, `MemoryStreamSlim` performs better in most cases when the option to zero out unused memory buffers is enabled ([`ZeroBuffers`](#zerobuffers) benchmark parameter). In these benchmarks, when zeroing out memory buffers is enabled, the `MemoryStreamSlim` option to clear memory buffers 'on release' was used as a fair comparison to the other classes. However, by default, a more efficient option to clear buffers out-of-band is used, and would further improve throughput performance by not incuring the cost of clearing memory buffers at the time of release, but instead doing so in a background thread.

The results for the segmented operations also show that `RecyclableMemoryStream` has a pretty high memory allocation rate and incurs a large number of garbage collections when the stream sizes get large, and the initial capacity is not provided on instantiation ([`CapacityOnCreate`](#capacityoncreate) benchmark parameter is `false`).

The following sections describe the different types of benchmarks run, and some general information on how to read the results, the benchmark operations, parameters and scenarios used.

### Benchmark Operations

A single benchmark operation consists of performing a loop of steps that does the following:

1. Create a new stream instance.
1. Write test data to the stream.
1. Read data back from the stream.
1. Dispose of the stream instance.

The [number of loops](./memorystream-benchmarks.md#loop-count-impact) in each operation is determined by the [`DataSize`](#datasize) parameter to keep each benchmark reasonably consistent in duration, but the loop count is always the same for all classes being compared for any given DataSize parameter value.

### Benchmark Parameters

The following parameters were used in the benchmarks. These will appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./memorystream-benchmarks.md#legend).

#### DataSize

The amount of data to write to the stream in each loop of the operation. The data is a byte array of the specified size. When the [GrowEachLoop](#groweachloop) parameter is set to `true`, the data size is increased by 256 bytes for each loop iteration, otherwise the data size is fixed for all loop iterations.

#### CapacityOnCreate

When `true`, the stream is instantiated with the current loop iteration data size as the initial capacity. When `false`, the stream is created with the default capacity (no initial capacity specified).

#### ZeroBuffers

When `true`, the stream is created with the option to zero out memory buffers when they are no longer used. When `false`, the stream is created with the option to not zero out memory buffers specified. For the `MemoryStreamSlim` class, the [`ZeroBufferBehavior`](xref:KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior) option is set to `OnRelease` to provide a fair comparison to the other classes.

The `MemoryStream` class has no option to zero out memory buffers (used memory is always cleared), so this parameter does not apply to that class.

#### GrowEachLoop

When `true`, the data size is increased by 256 bytes for each loop iteration within a benchmark operation. When `false`, the data size is fixed for all loop iterations.

## Benchmark Scenarios

The following scenarios were used for the benchmarks.

### Bulk Fill and Read

For this scenario, the write and read operations are done in bulk, with the entire data size written in a single operation and read back in a single operation. The results of the benchmarks are found in the [`Bulk Fill And Read`](./MemoryStreamBenchmarks.BulkFillAndReadThroughputBenchmarks-report-github.md) benchmark output.

### Segmented Fill and Read

For this scenario, the write step is done by writing a successive series of 4 kilobyte segments until [`DataSize`](#datasize) bytes have been written to the stream. The same approach is then used to read the data back in 4K segments.
The results of the benchmarks are found in the [`Segmented Fill And Read`](./MemoryStreamBenchmarks.SegmentedFillAndReadThroughputBenchmarks-report-github.md) benchmark output.

### HTML Reports

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in separate HTML files for each scenario. 


They can be found [here](./MemoryStreamBenchmarks.BulkFillAndReadThroughputBenchmarks-report.html) for the `Bulk Fill And Read` scenario and [here](./MemoryStreamBenchmarks.SegmentedFillAndReadThroughputBenchmarks-report.html) for the `Segmented Fill And Read` scenario.