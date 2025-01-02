# StringBuilder Throughput Benchmarks

This benchmark compares the throughput performance of `StringBuilder` class instances managed with the `StringBuilderCache` class (_'Cached StringBuilders'_) with operations that instantiate a new `StringBuilder` class for each use (_'Unique StringBuilders'_). The operations are run on a single thread using a single instance of the `StringBuilder` at a time.

## Summary 

The benchmark results show that utilizing the `StringBuilderCache` class consistently outperforms using unique instances of the `StringBuilder` class in all scenarios. The throughput performance and the memory allocation overhead of using the `StringBuilderCache` class is significantly better than instantiating a new `StringBuilder` instance for each use. The `StringBuilderCache` class is designed to reuse instances of the `StringBuilder` class, which eliminates the overhead of creating and disposing of instances and internal buffers used during the lifetime of an application.

### Benchmark Operations

A single benchmark operation consists of performing a loop of steps that does the following:

1. Acquire an instance of the `StringBuilder` class.
1. Append pre-allocated test string segments to the `StringBuilder` instance.
1. Get the built string from the `StringBuilder` instance.
1. Release the `StringBuilder` instance back to the cache.[^1]

[^1]: This step only applies in the `StringBuilderCache` case; for the unique `StringBuilder` case, the `StringBuilder` instance is simply allowed to be garbage collected.

### HTML Report

Since the benchmark results can create rather large tables, and the Markdown tables can be hard to absorb with the horizontal and vertical table scrolling, the results are also provided in a separate HTML file. 

This can be found [here](./StringsBenchmarks.StringBuilderThroughputBenchmarks-report.html).
