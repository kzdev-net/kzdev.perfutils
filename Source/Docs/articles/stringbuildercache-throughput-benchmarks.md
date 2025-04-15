# Single-Threaded StringBuilder Throughput Benchmarks

This benchmark compares the throughput performance of **StringBuilder** class instances managed with the `StringBuilderCache` class (_'Cached StringBuilders'_) against operations that instantiate a new **StringBuilder** class for each use (_'Unique StringBuilders'_). The operations are run on a single thread, using one instance of the **StringBuilder** at a time.

## Summary 

The benchmark results demonstrate that utilizing the **StringBuilderCache** class consistently outperforms using unique instances of the **StringBuilder** class in all scenarios. The throughput performance and memory allocation overhead of the **StringBuilderCache** class are significantly better than instantiating a new StringBuilder instance for each use. By reusing instances of the StringBuilder class, the **StringBuilderCache** eliminates the overhead of creating and disposing of instances and their internal buffers, improving both performance and memory efficiency.

### Benchmark Operations

A single benchmark operation consists of performing 100,000 iterations of the following steps:

1. Acquire an instance of the **StringBuilder** class.
1. Append pre-allocated test string segments to the **StringBuilder** instance.
1. Retrieve the built string from the **StringBuilder** instance.
1. Release the **StringBuilder** instance back to the cache.[^1]

### HTML Report

Since the benchmark results can create large tables that are difficult to navigate due to horizontal and vertical scrolling, the results are also provided in a simplified HTML table format.

The HTML report can be found [here](./StringsBenchmarks.StringBuilderThroughputBenchmarks-report.html).
