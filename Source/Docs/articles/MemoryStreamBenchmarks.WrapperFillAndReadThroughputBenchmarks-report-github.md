```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method                                               | DataSize  | ZeroBuffers | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0     | Allocated  | Alloc Ratio |
|----------------------------------------------------- |---------- |------------ |----------:|----------:|----------:|----------:|------:|--------:|---------:|-----------:|------------:|
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    | **False**       |  **59.50 ms** |  **0.120 ms** |  **0.112 ms** |  **59.50 ms** |  **1.00** |    **0.00** |        **-** | **1443.67 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    | False       | 105.12 ms |  0.265 ms |  0.248 ms | 105.08 ms |  1.77 |    0.01 | 200.0000 | 6315.94 KB |        4.37 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    | False       |  57.94 ms |  0.256 ms |  0.240 ms |  57.87 ms |  0.97 |    0.00 | 333.3333 | 6676.81 KB |        4.62 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    | **True**        |  **59.46 ms** |  **0.223 ms** |  **0.209 ms** |  **59.52 ms** |  **1.00** |    **0.00** |        **-** | **1443.63 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    | True        | 133.36 ms |  2.587 ms |  2.541 ms | 133.12 ms |  2.24 |    0.04 | 250.0000 | 6315.96 KB |        4.38 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    | True        |  58.18 ms |  0.367 ms |  0.343 ms |  58.16 ms |  0.98 |    0.01 | 333.3333 | 6676.81 KB |        4.63 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    | **False**       |  **63.50 ms** |  **0.497 ms** |  **0.441 ms** |  **63.56 ms** |  **1.00** |    **0.01** |        **-** |  **204.17 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    | False       | 112.07 ms |  1.672 ms |  1.564 ms | 112.22 ms |  1.77 |    0.03 |        - | 1760.59 KB |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    | False       |  64.12 ms |  1.258 ms |  2.067 ms |  64.74 ms |  1.01 |    0.03 |        - |  944.12 KB |        4.62 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    | **True**        |  **63.22 ms** |  **0.396 ms** |  **0.351 ms** |  **63.16 ms** |  **1.00** |    **0.01** |        **-** |  **204.17 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    | True        | 145.84 ms |  1.863 ms |  1.743 ms | 145.31 ms |  2.31 |    0.03 |        - | 1760.59 KB |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    | True        |  58.73 ms |  0.614 ms |  0.513 ms |  58.85 ms |  0.93 |    0.01 |        - |  944.12 KB |        4.62 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  | **False**       | **106.86 ms** |  **2.109 ms** |  **4.494 ms** | **104.96 ms** |  **1.00** |    **0.06** |        **-** |   **13.02 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  | False       | 241.41 ms |  4.181 ms |  3.911 ms | 240.31 ms |  2.26 |    0.10 |        - | 1083.65 KB |       83.26 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  | False       | 106.12 ms |  2.106 ms |  3.460 ms | 105.98 ms |  0.99 |    0.05 |        - |   59.91 KB |        4.60 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  | **True**        | **103.23 ms** |  **1.930 ms** |  **2.509 ms** | **102.04 ms** |  **1.00** |    **0.03** |        **-** |   **12.96 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  | True        | 299.49 ms |  1.873 ms |  1.462 ms | 299.50 ms |  2.90 |    0.07 |        - | 1083.71 KB |       83.63 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  | True        | 105.39 ms |  2.098 ms |  3.266 ms | 103.88 ms |  1.02 |    0.04 |        - |   59.91 KB |        4.62 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** | **False**       | **343.56 ms** |  **6.748 ms** | **11.458 ms** | **342.46 ms** |  **1.00** |    **0.05** |        **-** |    **2.64 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 | False       | 610.32 ms | 10.420 ms |  9.747 ms | 606.48 ms |  1.78 |    0.06 |        - | 1088.55 KB |      412.23 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 | False       | 323.15 ms |  6.252 ms |  5.849 ms | 322.60 ms |  0.94 |    0.03 |        - |    10.8 KB |        4.09 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** | **True**        | **333.40 ms** |  **4.456 ms** |  **4.168 ms** | **333.77 ms** |  **1.00** |    **0.02** |        **-** |     **2.3 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 | True        | 758.48 ms |  4.570 ms |  4.051 ms | 757.36 ms |  2.28 |    0.03 |        - | 1088.22 KB |      472.18 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 | True        | 325.51 ms |  3.494 ms |  3.268 ms | 324.04 ms |  0.98 |    0.02 |        - |    10.8 KB |        4.68 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **False**       | **351.23 ms** |  **3.172 ms** |  **2.812 ms** | **351.22 ms** |  **1.00** |    **0.01** |        **-** |    **1.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | False       | 629.79 ms |  8.504 ms |  7.955 ms | 630.55 ms |  1.79 |    0.03 |        - | 1066.88 KB |      734.19 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 | False       | 361.86 ms |  7.196 ms | 17.102 ms | 359.39 ms |  1.03 |    0.05 |        - |     5.3 KB |        3.65 |
|                                                      |           |             |           |           |           |           |       |         |          |            |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **True**        | **363.44 ms** |  **7.097 ms** | **12.798 ms** | **358.58 ms** |  **1.00** |    **0.05** |        **-** |    **1.45 KB** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | True        | 799.36 ms | 12.229 ms | 11.439 ms | 803.14 ms |  2.20 |    0.08 |        - | 1066.88 KB |      734.19 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 | True        | 349.30 ms |  5.338 ms |  4.457 ms | 347.99 ms |  0.96 |    0.03 |        - |     5.3 KB |        3.65 |
