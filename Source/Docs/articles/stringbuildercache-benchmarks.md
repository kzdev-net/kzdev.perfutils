﻿# StringBuilderCache Benchmarks

The benchmark tests for the `StringBuilderCache` are simple comparisons of the throughput time and memory consumption differences between using the `StringBuilderCache` and the standard `StringBuilder` class (instantiating a new instance for each use). There are just two benchmark scenarios, one for a single thread that repeatedly uses the `StringBuilder` class, and one for a multi-threaded scenario where multiple threads are using `StringBuilder` class instances simultaneously. The results are presented in the following sections.

## Benchmark Setup

The benchmarks were run using the [BenchmarkDotNet](https://benchmarkdotnet.org/) library. The following sections describe the different types of benchmarks run, and some general information on how to read the results.

### Legend

These are the standard BenchmarkDotNet columns found in the benchmark results. Each benchmark scenario will describe the parameter value meanings for those specific benchmark runs on those topic pages.

| Column      | Description                                                                  |
| ----------- | ---------------------------------------------------------------------------- |
| Mean        | Arithmetic mean of all measurements                                          |
| Error       | Half of 99.9% confidence interval                                            |
| StdDev      | Standard deviation of all measurements                                       |
| Median      | Value separating the higher half of all measurements (50th percentile)       |
| Ratio       | Mean of the ratio distribution ([Current]/[Baseline])                        |
| RatioSD     | Standard deviation of the ratio distribution ([Current]/[Baseline])          |
| Gen0        | GC Generation 0 collects per 1000 operations                                 |
| Gen1        | GC Generation 1 collects per 1000 operations                                 |
| Gen2        | GC Generation 2 collects per 1000 operations                                 |
| Allocated   | Allocated memory per single operation (managed only, inclusive, 1KB = 1024B) |
| Alloc Ratio | Allocated memory ratio distribution ([Current]/[Baseline])                   |
| 1 μs        | 1 Microsecond (0.000001 sec)                                                 |
| 1 ms        | 1 Millisecond (0.001 sec)                                                    |

### HTML Reports

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in separate HTML files for each scenario. 

## Benchmark Scenarios

The benchmark scenarios are broken down into two categories; Single Thread & Multiple Threads.

### Single Thread

The Single Thread benchmark scenario uses a single thread that repeatedly uses the `StringBuilder` class to build a string from a series of string segments and then retrieve the built string. [Read More](./stringbuildercache-throughput-benchmarks.md)

### Multiple Threads

The Multiple Thread benchmark scenario uses the exact same operation flow as the Single Thread scenario, however multiple threads are simultaneously running the operations. [Read More](./stringbuildercache-multithread-throughput-benchmarks.md)

## Reading Results

### Allocations

The very nature of building a string in memory requires memory allocations. So, the allocation values in the benchmarks are substantially large. The meaning of this information as standalone values don't provide much insight, but like most benchmarking, the important thing to note is the comparison and _**difference**_ in allocations between the `StringBuilder` and `StringBuilderCache` use cases. It is easier to use the ` Alloc Ratio` column for this comparison. The strings built for the benchmarks are identical for both classes, so the allocations are directly comparable.

The string segments used to build the strings are pre-allocated (outside of the scope of the benchmarks) and are the same for each benchmark operation. The allocations are the result of the `StringBuilder` or `StringBuilderCache` class operations.

## Versions

The benchmarks published here used the following versions of the libraries:

- `BenchmarkDotNet` version: 0.14.0
- `StringBuilderCache` version: 1.2.0
- `StringBuilder` version: .NET 8.0.11