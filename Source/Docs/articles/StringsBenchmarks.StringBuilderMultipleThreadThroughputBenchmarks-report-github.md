```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.101
  [Host]                         : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Multithread StringBuilderCache : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2

Job=Multithread StringBuilderCache  InvocationCount=1  MaxIterationCount=30  
MaxWarmupIterationCount=20  UnrollFactor=1  

```
| Method                               | ThreadCount | Mean        | Error    | StdDev   | Ratio | Gen0         | Gen1         | Allocated   | Alloc Ratio |
|------------------------------------- |------------ |------------:|---------:|---------:|------:|-------------:|-------------:|------------:|------------:|
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **4**           |    **47.09 ms** | **0.305 ms** | **0.255 ms** |  **1.00** |   **87000.0000** |    **4000.0000** |  **1364.03 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 4           |    28.17 ms | 0.480 ms | 0.449 ms |  0.60 |   42000.0000 |            - |   551.86 MB |        0.40 |
|                                      |             |             |          |          |       |              |              |             |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **16**          |   **431.10 ms** | **1.943 ms** | **1.817 ms** |  **1.00** |  **391000.0000** |   **94000.0000** |  **6150.17 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 16          |   146.82 ms | 0.817 ms | 0.724 ms |  0.34 |  174000.0000 |            - |   2287.2 MB |        0.37 |
|                                      |             |             |          |          |       |              |              |             |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **32**          | **1,112.96 ms** | **3.728 ms** | **3.304 ms** |  **1.00** |  **773000.0000** |  **378000.0000** | **12231.45 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 32          |   429.55 ms | 8.221 ms | 8.074 ms |  0.39 |  347000.0000 |            - |  4698.66 MB |        0.38 |
|                                      |             |             |          |          |       |              |              |             |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **64**          | **2,103.34 ms** | **5.419 ms** | **4.804 ms** |  **1.00** | **1475000.0000** | **1153000.0000** | **23502.95 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 64          |   869.08 ms | 2.541 ms | 2.122 ms |  0.41 |  696000.0000 |            - |  9075.77 MB |        0.39 |
