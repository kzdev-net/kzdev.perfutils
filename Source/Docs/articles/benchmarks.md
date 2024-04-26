# Benchmarks

As previously mentioned, the general goals and approach to ease GC pressure and reduce memory traffic are the same for both `MemoryStreamSlim` and `RecyclableMemoryStream`. So, for benchmarking purposes, the standard `MemoryStream` class, `RecyclableMemoryStream`, and `MemoryStreamSlim` are compared side-by-side for performance under a number of use cases.

In all cases, the benchmark values on the `MemoryStream` class are used as the baseline for comparison. The `RecyclableMemoryStream` and `MemoryStreamSlim` classes are then compared to the `MemoryStream` class to determine the performance and memory allocation differences for each set of benchmark parameters.

## Benchmark Setup

The benchmarks were run using the [BenchmarkDotNet](https://benchmarkdotnet.org/) library. The following sections describe the different types of benchmarks run, and some general information on how to read the results. Details on the different benchmark scenarios are covered on their respective pages.

### Legend

These are the standard BenchmarkDotNet columns found in the benchmark results. Each benchmark scenario will describe the parameter value meanings for those specific benchmark runs on those topic pages.

| Column | Description |
| --- | --- |
| Mean | Arithmetic mean of all measurements
| Error | Half of 99.9% confidence interval
| StdDev | Standard deviation of all measurements
| Median | Value separating the higher half of all measurements (50th percentile)
| Ratio | Mean of the ratio distribution ([Current]/[Baseline])
| RatioSD | Standard deviation of the ratio distribution ([Current]/[Baseline])
| Gen0 | GC Generation 0 collects per 1000 operations
| Gen1 | GC Generation 1 collects per 1000 operations
| Gen2 | GC Generation 2 collects per 1000 operations
| Allocated | Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
| Alloc Ratio | Allocated memory ratio distribution ([Current]/[Baseline])
| 1 μs | 1 Microsecond (0.000001 sec)
| 1 ms | 1 Millisecond (0.001 sec)

### HTML Reports

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in separate HTML files for each scenario. 

## Benchmark Scenarios

The benchmark scenarios are broken down into three main categories: 

- Dynamic Throughput
- Wrapper Throughput
- Continuous Growth Throughput

### Dynamic Throughput

The two scenarios in this category use a dynamic expandable stream that is instantiated with an initial zero length and capacity. [Read More](./dynamic-throughput-benchmarks.md)

### Wrapper Throughput

For this scenario, the streams are instantiated with an already allocated and available byte array to benchmark the 'wrapped' mode behavior. [Read More](./wrapper-throughput-benchmarks.md)

### Continuous Growth Throughput

This scenario demonstrates the performance impact of accessing the internal buffer directly then then continuaing to grow the stream after that. [Read More](./continuous-growth-throughput-benchmarks.md)

### CopyToAsync Throughput

This scenario demonstrates the performance impact of copying the contents of a stream to another stream asynchronously. [Read More](./copytoasync-throughput-benchmarks.md)

### Set Loop Count Throughput

This scenario demonstrates the performance impact of different data sizes on the streams with that same loop count for every benchmark operation. [Read More](./set-loop-count-throughput-benchmarks.md)

## Reading Results

### Parameter Effect

Not every parameter used in the benchmarks will be applicable to every class being compared. For example, the `MemoryStream` class does not have an option to zero out memory buffers, so the `ZeroBuffers` parameter is not applicable to that class, it is the default and only behavior for that class.

Two things to consider in this case. First, the `MemoryStream` class operations are run again for every parameter scenario even though the `ZeroBuffers` parameter is not applicable to that class to make the result easy to read and compare to the other classes side by side. Second, the benchmark results where 'ZeroBuffers' is set to `true` will give a more accurate comparison between the different classes, but the results where 'ZeroBuffers' is set to `false` do show the performance gains that can be had by not zeroing out memory buffers for non-sensitive data streams.

The specific parameter values that have similar caveats will be noted in the benchmark scenario descriptions.

### Loop Count Impact

In order to keep the benchmark operation times reasonable and measurable and not too fast or slow, each operation involves a number of stream instances being instantiated, written to, read from, and released in a loop. The number of loops is calculated based on the `DataSize` parameter, which is the amount of data written to the stream in each loop of the operation. The loop count is always the same for all classes being compared for any given `DataSize` parameter value, but will be different for different `DataSize` values. Keep this in mind when comparing the results for different `DataSize` values. Performance comparisons should be scoped or partitioned to a common `DataSize` value for the different classes being compared and the other parameter values. In other words, consider each `DataSize` value as a separate benchmark scenario and report.

The exception to this is the ['Set Loop Count'](./set-loop-count-throughput-benchmarks.md) benchmark scenario, where the loop count is set to a fixed value for all classes and all data sizes being compared, while the `DataSize` parameter is still varied to see the impact of the data size on the operation times. This scenario is useful for understanding the performance impact of different data sizes on the different classes being compared, given that the other benchmarks can not be read that way due to the loop count being determined by the `DataSize` parameter.

## Allocations

The loop count for each respective data size also contributes to the memory allocations per operation. As the data size increases, the loop count decreases to keep the operation times reasonable. Since each benchmark operation consists of creating a new stream instance on each loop, more instances of the stream classes are created and disposed of for smaller data sizes than the larger sizes, which results in more memory allocations attributed to the test classes themselves with the smaller data sizes. This is really unavoidable, but it is important to understand that the memory allocations are not just for the data being written to the stream, but also for the stream instances themselves.

This is another reason why the ['Set Loop Count'](./set-loop-count-throughput-benchmarks.md) benchmark scenario is useful for understanding the performance impact of different data sizes on the different classes being compared, given that the other benchmarks can not be read that way due to the loop count being determined by the `DataSize` parameter.

## Versions

The benchmarks published here used the following versions of the libraries:

- `MemoryStreamSlim` version: 1.0.0
- `RecyclableMemoryStream` version: 3.0.1
- `MemoryStream` version: .NET 8.0.10