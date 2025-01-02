# Wrapped Throughput Benchmark

This benchmark scenario uses stream instances that are instantiated by providing an existing byte array, creating a `Stream` semantic API access to the provided array.

Note that the `RecyclableMemoryStream` behavior is different in that it will not use the provided byte array directly, but will instead create a new byte array and copy the provided data into it. This is because the `RecyclableMemoryStream` class is designed to be used with a pool of memory buffers that are reused, and the provided byte array is not managed by the pool.

The `MemoryStreamSlim` class will internally just wrap an instance of `MemoryStream` which in turn wraps the provided byte array. This is done to provide a consistent `Stream` semantic API to the provided byte array, but allow a common use of the `MemoryStreamSlim` class when that consistency is desired. This also allows for a consistent and deterministic behavior when using this fixed mode of operation.

## Summary 

The benchmark results show that `RecyclableMemoryStream` provides no benefit over the `MemoryStream` class when the stream is instantiated with a fixed byte array, and is in fact detrimental to performance. This is because the buffer provided in the constructor is copied to a new buffer, which is then used by the stream. This copying operation is a significant performance hit, especially when the data size is large.

The `MemoryStreamSlim` class understandably has the same throughput performance as the `MemoryStream` class, as it is just a wrapper around the `MemoryStream` class. The allocation difference in the results is due to the fact that each loop iteration creates a new `MemoryStreamSlim` instance, which in turn creates a new internal `MemoryStream` instance, which then wraps the provided byte array. As the [`DataSize`](#datasize) parameter increases, the `MemoryStreamSlim` class results show lower allocations per operation because the number of loops for each operation decreases as the data size increases, which results in fewer instances of `MemoryStreamSlim` being created. The allocations then are for the `MemoryStreamSlim` `MemoryStream` class instances and there are no additional allocations made.

### Benchmark Operation

A single benchmark operation consists of performing a loop of steps that does the following:

1. Create a new stream instance.
1. Write test data to the stream.
1. Read data back from the stream.
1. Dispose of the stream instance.

The [number of loops](./memorystream-benchmarks.md#loop-count-impact) in each operation is determined by the [`DataSize`](#datasize) parameter to keep each benchmark reasonably consistent in duration, but the loop count is always the same for all classes being compared for any given DataSize parameter value.

### Benchmark Parameters

The following parameters were used in the benchmarks. These will appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./memorystream-benchmarks.md#legend).

#### DataSize

The amount of data to write to the stream in each loop of the operation. The data is a byte array of the specified size. This data size is fixed for all loop iterations.

#### ZeroBuffers

When `true`, the stream is created with the option to zero out memory buffers when they are no longer used. When `false`, the stream is created with the option to not zero out memory buffers specified. For the `MemoryStreamSlim` class, the [`ZeroBufferBehavior`](xref:KZDev.PerfUtils.MemoryStreamSlimOptions.ZeroBufferBehavior) option is set to `OnRelease` to provide a fair comparison to the other classes.

The `MemoryStream` class has no option to zero out memory buffers (used memory is always cleared), so this parameter does not apply to that class.

## Benchmark Scenario

For this scenario, the stream is created with an initial source byte array. The write step overwrites the entire contents of the stream from the beginning done by writing a successive series of 4 kilobyte segments until [`DataSize`](#datasize) bytes have been written to the stream. The same approach is then used to read the data back in 4K segments.
The results of the benchmarks are found in the [`Wrapped Fill And Read`](./MemoryStreamBenchmarks.WrapperFillAndReadThroughputBenchmarks-report-github.md) benchmark output.

### HTML Report

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in a separate HTML file. 

This can be found [here](./MemoryStreamBenchmarks.WrapperFillAndReadThroughputBenchmarks-report.html).