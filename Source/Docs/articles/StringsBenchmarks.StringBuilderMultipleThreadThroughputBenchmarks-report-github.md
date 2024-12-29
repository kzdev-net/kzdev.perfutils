```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.404
  [Host]                         : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Multithread StringBuilderCache : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2

Job=Multithread StringBuilderCache  InvocationCount=1  MaxIterationCount=30  
MaxWarmupIterationCount=20  UnrollFactor=1  

```
| Method                               | ThreadCount | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0         | Gen1         | Allocated   | Alloc Ratio |
|------------------------------------- |------------ |------------:|----------:|----------:|------:|--------:|-------------:|-------------:|------------:|------------:|
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **4**           |    **59.01 ms** |  **0.894 ms** |  **0.836 ms** |  **1.00** |    **0.02** |   **98000.0000** |    **6000.0000** |  **1517.47 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 4           |    26.72 ms |  0.528 ms |  0.468 ms |  0.45 |    0.01 |   43000.0000 |            - |   585.06 MB |        0.39 |
|                                      |             |             |           |           |       |         |              |              |             |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **16**          |   **504.76 ms** |  **2.535 ms** |  **2.247 ms** |  **1.00** |    **0.01** |  **413000.0000** |  **102000.0000** |   **6379.7 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 16          |   153.75 ms |  0.686 ms |  0.642 ms |  0.30 |    0.00 |  173000.0000 |            - |  2304.96 MB |        0.36 |
|                                      |             |             |           |           |       |         |              |              |             |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **32**          |   **909.87 ms** |  **3.466 ms** |  **2.895 ms** |  **1.00** |    **0.00** |  **734000.0000** |  **347000.0000** | **11718.35 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 32          |   357.86 ms |  2.668 ms |  2.495 ms |  0.39 |    0.00 |  348000.0000 |            - |  4581.92 MB |        0.39 |
|                                      |             |             |           |           |       |         |              |              |             |             |
| **&#39;Multi-Thread Unique StringBuilders&#39;** | **64**          | **3,144.81 ms** |  **5.737 ms** |  **5.367 ms** |  **1.00** |    **0.00** | **1669000.0000** | **1460000.0000** | **25332.15 MB** |        **1.00** |
| &#39;Multi-Thread Cached StringBuilders&#39; | 64          | 1,033.32 ms | 28.191 ms | 42.195 ms |  0.33 |    0.01 |  697000.0000 |            - |  9138.28 MB |        0.36 |
