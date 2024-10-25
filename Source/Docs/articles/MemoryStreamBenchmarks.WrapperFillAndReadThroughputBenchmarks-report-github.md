```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method                                               | DataSize  | ZeroBuffers | Mean      | Error    | StdDev    | Ratio | RatioSD | Gen0     | Allocated  | Alloc Ratio |
|----------------------------------------------------- |---------- |------------ |----------:|---------:|----------:|------:|--------:|---------:|-----------:|------------:|
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    | **False**       |  **59.03 ms** | **0.312 ms** |  **0.292 ms** |  **1.00** |    **0.01** |        **-** | **1443.67 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    | False       | 106.68 ms | 0.708 ms |  0.628 ms |  1.81 |    0.01 | 200.0000 | 6315.94 KB |        4.37 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    | False       |  62.58 ms | 1.171 ms |  1.253 ms |  1.06 |    0.02 | 333.3333 | 6676.81 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    | **True**        |  **60.89 ms** | **0.370 ms** |  **0.346 ms** |  **1.00** |    **0.01** |        **-** | **1445.01 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    | True        | 139.14 ms | 1.287 ms |  1.204 ms |  2.29 |    0.02 | 250.0000 | 6315.96 KB |        4.37 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    | True        |  58.87 ms | 0.332 ms |  0.294 ms |  0.97 |    0.01 | 333.3333 | 6676.81 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    | **False**       |  **64.01 ms** | **0.610 ms** |  **0.571 ms** |  **1.00** |    **0.01** |        **-** |  **204.17 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    | False       | 110.11 ms | 0.740 ms |  0.692 ms |  1.72 |    0.02 |        - | 1760.66 KB |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    | False       |  58.26 ms | 0.381 ms |  0.338 ms |  0.91 |    0.01 |        - |  944.12 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    | **True**        |  **63.28 ms** | **0.441 ms** |  **0.413 ms** |  **1.00** |    **0.01** |        **-** |  **204.17 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    | True        | 147.09 ms | 0.931 ms |  0.870 ms |  2.32 |    0.02 |        - | 1760.68 KB |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    | True        |  64.17 ms | 1.229 ms |  1.641 ms |  1.01 |    0.03 |        - |  944.12 KB |        4.62 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  | **False**       | **111.95 ms** | **2.186 ms** |  **3.272 ms** |  **1.00** |    **0.04** |        **-** |   **13.02 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  | False       | 251.04 ms | 3.175 ms |  2.652 ms |  2.24 |    0.07 |        - | 1083.71 KB |       83.26 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  | False       | 109.84 ms | 2.150 ms |  3.014 ms |  0.98 |    0.04 |        - |   59.91 KB |        4.60 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  | **True**        | **110.51 ms** | **2.202 ms** |  **3.619 ms** |  **1.00** |    **0.05** |        **-** |   **13.02 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  | True        | 305.49 ms | 3.454 ms |  3.231 ms |  2.77 |    0.09 |        - | 1083.71 KB |       83.26 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  | True        | 108.99 ms | 2.112 ms |  2.891 ms |  0.99 |    0.04 |        - |   59.91 KB |        4.60 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** | **False**       | **356.41 ms** | **6.619 ms** |  **6.192 ms** |  **1.00** |    **0.02** |        **-** |    **2.64 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 | False       | 622.39 ms | 4.881 ms |  4.566 ms |  1.75 |    0.03 |        - | 1088.55 KB |      412.23 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 | False       | 344.51 ms | 6.665 ms |  9.769 ms |  0.97 |    0.03 |        - |    10.8 KB |        4.09 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** | **True**        | **354.11 ms** | **5.549 ms** |  **4.919 ms** |  **1.00** |    **0.02** |        **-** |    **2.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 | True        | 778.38 ms | 9.719 ms |  9.091 ms |  2.20 |    0.04 |        - | 1088.55 KB |      445.16 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 | True        | 340.54 ms | 4.440 ms |  3.936 ms |  0.96 |    0.02 |        - |    10.8 KB |        4.42 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **False**       | **363.19 ms** | **6.176 ms** |  **6.865 ms** |  **1.00** |    **0.03** |        **-** |    **1.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | False       | 636.63 ms | 8.349 ms |  7.810 ms |  1.75 |    0.04 |        - | 1066.88 KB |      734.19 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 | False       | 385.97 ms | 7.204 ms |  6.739 ms |  1.06 |    0.03 |        - |     5.3 KB |        3.65 |
|                                                      |           |             |           |          |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **True**        | **366.39 ms** | **7.148 ms** | **11.745 ms** |  **1.00** |    **0.04** |        **-** |    **1.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | True        | 800.63 ms | 6.437 ms |  6.022 ms |  2.19 |    0.07 |        - | 1066.88 KB |      734.19 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 | True        | 378.82 ms | 7.430 ms |  8.845 ms |  1.03 |    0.04 |        - |     5.3 KB |        3.65 |
