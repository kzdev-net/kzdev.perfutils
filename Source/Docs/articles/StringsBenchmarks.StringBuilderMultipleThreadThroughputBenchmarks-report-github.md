```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]                         : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Multithread StringBuilderCache : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Multithread StringBuilderCache  InvocationCount=1  MaxIterationCount=30  
MaxWarmupIterationCount=20  UnrollFactor=1  

```
| Method                               | ThreadCount | Mean       | Error     | StdDev   | Median     | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------------------- |------------ |-----------:|----------:|---------:|-----------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **4**           |  **12.364 ms** | **0.6927 ms** | **1.037 ms** |  **11.890 ms** |  **1.01** |    **0.11** |  **20000.0000** |   **1000.0000** |  **307.74 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 4           |   8.319 ms | 1.2482 ms | 1.868 ms |   9.334 ms |  0.68 |    0.16 |   8000.0000 |           - |  110.59 MB |        0.36 |
|                                      |             |            |           |          |            |       |         |             |             |            |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **16**          |  **86.179 ms** | **1.5601 ms** | **1.459 ms** |  **86.150 ms** |  **1.00** |    **0.02** |  **79000.0000** |  **19000.0000** |  **1197.1 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 16          |  29.027 ms | 0.9068 ms | 1.357 ms |  29.455 ms |  0.34 |    0.02 |  34000.0000 |           - |  461.52 MB |        0.39 |
|                                      |             |            |           |          |            |       |         |             |             |            |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **32**          | **171.260 ms** | **2.7276 ms** | **2.418 ms** | **170.266 ms** |  **1.00** |    **0.02** | **148000.0000** |  **68000.0000** | **2304.97 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 32          |  82.538 ms | 1.8530 ms | 2.773 ms |  81.769 ms |  0.48 |    0.02 |  69000.0000 |           - |  895.66 MB |        0.39 |
|                                      |             |            |           |          |            |       |         |             |             |            |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **64**          | **636.923 ms** | **6.2141 ms** | **5.813 ms** | **635.695 ms** |  **1.00** |    **0.01** | **324000.0000** | **234000.0000** | **4964.49 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 64          | 169.063 ms | 2.5609 ms | 2.395 ms | 168.463 ms |  0.27 |    0.00 | 139000.0000 |           - |  1852.9 MB |        0.37 |
