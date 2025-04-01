# Compression Benchmarks

The benchmarks for the different compression classes are simple comparisons of the throughput time and memory consumption differences between using the `MemoryStream` `RecyclableMemoryStream` and `MemoryStreamSlim` classes to perform compression of data of various sizes. The results are presented in the following sections.

## Benchmark Setup

The benchmarks were run using the [BenchmarkDotNet](https://benchmarkdotnet.org/) library. The following sections describe the different types of benchmarks run, and some general information on how to read the results. This article covers basic information for the result using the different compression classes.

### Legend

These are the standard BenchmarkDotNet columns found in the benchmark results. Each benchmark scenario will describe the parameter value meanings for those specific benchmark runs on those topic pages.

| Column | Description |
| --- | --- |
| Mean | Arithmetic mean of all measurements
| Error | Half of 99.9% confidence interval
| StdDev | Standard deviation of all measurements
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

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in separate HTML files for each compression class. 

## Benchmark Scenarios

The compression benchmarks are run with source byte arrays of various sizes. The compression classes are compared to the `MemoryStream` class as the baseline for comparison. The `RecyclableMemoryStream` and `MemoryStreamSlim` classes are then compared to the `MemoryStream` class to determine the performance and memory allocation differences for each set of benchmark parameters.

The benchmark being run uses the different MemoryStream types as the destination stream for the compression operations.

There is a separate benchmark scenario for each type of compression technology:

- Brotli Compression
- Deflate Compression
- GZip Compression
- ZLib Compression

## Summary 

The benchmark results show that the `RecyclableMemoryStream` and `MemoryStreamSlim` classes perform comparably for throughput time with both of them far outperforming the standard `MemoryStream` class. However, `MemoryStreamSlim` clearly requires less memory allocations than the `RecyclableMemoryStream`.

### Benchmark Operation

A single benchmark operation consists of performing five loops of steps that do the following:

1. Create a new stream instance to receive the result of the compression operation.
1. Create an instance of the compression class to perform the compression.
1. Send the test data through the compression class to the memory stream instance receiving the compressed result.
1. Dispose of the compression class instance.
1. Dispose of the destintation memory stream instance.

`MemoryStreamSlim` and `RecyclableMemoryStream` classes are created with the option to zero out memory buffers when they are no longer used disabled to keep the benchmark performance focused on the compression operation. The `MemoryStream` class has no option to zero out memory buffers (used memory is always cleared - i.e. internal buffers are allocated with `new byte[]`), so this option does not apply to that class.

### Benchmark Parameters

The following parameters were used in the benchmarks. These will appear as columns in the benchmark results along with the [standard BenchmarkDotNet columns](./memorystream-benchmarks.md#legend).

#### DataSize

The size of the source data that is compressed into the destination stream in each operation loop. The source data is a byte array of the specified size.

#### CapacityOnCreate

When `true`, the destination stream is instantiated with the data size as the initial capacity. 

When `false`, the stream is created with the default capacity (no initial capacity specified). The results show no notable difference in throughput time performance between the two options, but there is a noticable impact in memory allocations for `MemoryStream` and a sizeable impact for `RecyclableMemoryStream`.

## Benchmark Results

The results of the benchmarks are found in the following benchmark outputs:

- [Brotli Compression](./MemoryStreamBenchmarks.BrotliCompressionThroughputBenchmarks-report-github.md)
- [Deflate Compression](./MemoryStreamBenchmarks.DeflateCompressionThroughputBenchmarks-report-github.md)
- [GZip Compression](./MemoryStreamBenchmarks.GZipCompressionThroughputBenchmarks-report-github.md)
- [ZLib Compression](./MemoryStreamBenchmarks.ZLibCompressionThroughputBenchmarks-report-github.md)

### HTML Reports

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in separate HTML files. 

These can be found here:

- [Brotli Compression](./MemoryStreamBenchmarks.BrotliCompressionThroughputBenchmarks-report.html)
- [Deflate Compression](./MemoryStreamBenchmarks.DeflateCompressionThroughputBenchmarks-report.html)
- [GZip Compression](./MemoryStreamBenchmarks.GZipCompressionThroughputBenchmarks-report.html)
- [ZLib Compression](./MemoryStreamBenchmarks.ZLibCompressionThroughputBenchmarks-report.html)

## Versions

The benchmarks published here used the following versions of the libraries:

- `BenchmarkDotNet` version: 0.14.0
- `KZDev.PerfUtils` version: 2.0.0
- `RecyclableMemoryStream` version: 3.0.1
- `MemoryStream` and Compression Streams version: .NET 9.0.3
