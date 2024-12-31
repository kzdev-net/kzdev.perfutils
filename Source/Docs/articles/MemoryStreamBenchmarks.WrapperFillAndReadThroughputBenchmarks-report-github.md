```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2


```
| Method                                               | DataSize  | ZeroBuffers | Mean      | Error    | StdDev    | Ratio | RatioSD | Gen0     | Allocated  | Alloc Ratio |
|----------------------------------------------------- |---------- |------------ |----------:|---------:|----------:|------:|--------:|---------:|-----------:|------------:|
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    | **False**       |  **60.18 ms** | **0.494 ms** |  **0.462 ms** |  **1.00** |    **0.01** |        **-** | **1443.67 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    | False       | 106.64 ms | 2.035 ms |  2.646 ms |  1.77 |    0.05 | 200.0000 | 6315.94 KB |        4.37 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    | False       |  59.62 ms | 0.445 ms |  0.416 ms |  0.99 |    0.01 | 250.0000 | 6676.81 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    | **True**        |  **61.56 ms** | **0.560 ms** |  **0.524 ms** |  **1.00** |    **0.01** |        **-** | **1443.67 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    | True        | 138.19 ms | 2.747 ms |  2.570 ms |  2.25 |    0.04 | 250.0000 | 6315.96 KB |        4.37 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    | True        |  59.30 ms | 0.471 ms |  0.417 ms |  0.96 |    0.01 | 250.0000 | 6676.81 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    | **False**       |  **64.83 ms** | **0.587 ms** |  **0.549 ms** |  **1.00** |    **0.01** |        **-** |  **204.17 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    | False       | 111.34 ms | 1.044 ms |  0.976 ms |  1.72 |    0.02 |        - | 1760.66 KB |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    | False       |  59.05 ms | 0.387 ms |  0.362 ms |  0.91 |    0.01 |        - |  944.12 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    | **True**        |  **64.20 ms** | **0.349 ms** |  **0.326 ms** |  **1.00** |    **0.01** |        **-** |  **204.17 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    | True        | 148.16 ms | 1.083 ms |  1.013 ms |  2.31 |    0.02 |        - | 1760.68 KB |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    | True        |  65.49 ms | 0.452 ms |  0.423 ms |  1.02 |    0.01 |        - |  944.13 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  | **False**       | **113.46 ms** | **2.198 ms** |  **2.443 ms** |  **1.00** |    **0.03** |        **-** |   **13.02 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  | False       | 255.43 ms | 3.012 ms |  2.818 ms |  2.25 |    0.05 |        - | 1083.71 KB |       83.26 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  | False       | 112.59 ms | 2.206 ms |  2.540 ms |  0.99 |    0.03 |        - |   59.91 KB |        4.60 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  | **True**        | **114.89 ms** | **2.268 ms** |  **2.520 ms** |  **1.00** |    **0.03** |        **-** |   **13.02 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  | True        | 320.89 ms | 3.866 ms |  3.427 ms |  2.79 |    0.07 |        - | 1083.71 KB |       83.26 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  | True        | 112.98 ms | 2.180 ms |  2.677 ms |  0.98 |    0.03 |        - |   59.91 KB |        4.60 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** | **False**       | **346.65 ms** | **6.831 ms** | **12.142 ms** |  **1.00** |    **0.05** |        **-** |    **2.64 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 | False       | 614.07 ms | 6.183 ms |  5.481 ms |  1.77 |    0.06 |        - | 1088.55 KB |      412.23 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 | False       | 341.06 ms | 6.745 ms | 11.814 ms |  0.99 |    0.05 |        - |    10.8 KB |        4.09 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** | **True**        | **349.20 ms** | **6.955 ms** |  **6.830 ms** |  **1.00** |    **0.03** |        **-** |    **2.64 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 | True        | 773.85 ms | 6.443 ms |  5.030 ms |  2.22 |    0.04 |        - | 1088.55 KB |      412.23 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 | True        | 345.87 ms | 6.915 ms | 11.743 ms |  0.99 |    0.04 |        - |    10.8 KB |        4.09 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **False**       | **376.22 ms** | **7.421 ms** |  **9.649 ms** |  **1.00** |    **0.04** |        **-** |    **1.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | False       | 628.51 ms | 9.902 ms |  9.262 ms |  1.67 |    0.05 |        - | 1066.88 KB |      734.19 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 | False       | 362.26 ms | 7.177 ms | 11.791 ms |  0.96 |    0.04 |        - |     5.3 KB |        3.65 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **True**        | **372.87 ms** | **7.327 ms** | **12.639 ms** |  **1.00** |    **0.05** |        **-** |    **1.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | True        | 807.44 ms | 9.640 ms |  9.017 ms |  2.17 |    0.08 |        - | 1066.88 KB |      734.19 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 | True        | 366.57 ms | 7.307 ms | 15.412 ms |  0.98 |    0.05 |        - |     5.3 KB |        3.65 |
