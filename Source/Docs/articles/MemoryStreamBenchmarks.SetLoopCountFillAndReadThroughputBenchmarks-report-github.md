```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method                                                | DataSize  | ZeroBuffers | Mean           | Error        | StdDev       | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated      | Alloc Ratio |
|------------------------------------------------------ |---------- |------------ |---------------:|-------------:|-------------:|------:|--------:|-----------:|-----------:|-----------:|---------------:|------------:|
| **&#39;MemoryStream set loop count fill and read&#39;**           | **131072**    | **False**       |     **2,178.4 μs** |     **42.16 μs** |     **39.44 μs** |  **1.00** |    **0.02** |  **2082.0313** |  **2082.0313** |  **2082.0313** |    **12610.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 131072    | False       |       141.9 μs |      0.35 μs |      0.29 μs |  0.07 |    0.00 |     0.7324 |          - |          - |       13.67 KB |       0.001 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 131072    | False       |       188.5 μs |      1.07 μs |      1.00 μs |  0.09 |    0.00 |     0.7324 |          - |          - |       14.06 KB |       0.001 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **131072**    | **True**        |     **2,146.4 μs** |     **42.27 μs** |     **41.52 μs** |  **1.00** |    **0.03** |  **2082.0313** |  **2082.0313** |  **2082.0313** |    **12610.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 131072    | True        |       207.3 μs |      0.68 μs |      0.57 μs |  0.10 |    0.00 |     0.7324 |          - |          - |       13.67 KB |       0.001 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 131072    | True        |       308.2 μs |      1.35 μs |      1.26 μs |  0.14 |    0.00 |     0.4883 |          - |          - |       14.06 KB |       0.001 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **983040**    | **False**       |    **27,419.8 μs** |    **188.39 μs** |    **176.22 μs** |  **1.00** |    **0.01** | **24984.3750** | **24984.3750** | **24984.3750** |   **102221.88 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 983040    | False       |     1,009.6 μs |      3.93 μs |      3.28 μs |  0.04 |    0.00 |     1.9531 |          - |          - |       44.53 KB |       0.000 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 983040    | False       |     1,128.5 μs |      4.57 μs |      4.27 μs |  0.04 |    0.00 |          - |          - |          - |       14.06 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **983040**    | **True**        |    **27,203.9 μs** |    **175.68 μs** |    **164.33 μs** |  **1.00** |    **0.01** | **24984.3750** | **24984.3750** | **24984.3750** |   **102221.88 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 983040    | True        |     1,586.7 μs |     15.95 μs |     14.92 μs |  0.06 |    0.00 |     1.9531 |          - |          - |       44.53 KB |       0.000 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 983040    | True        |     1,791.2 μs |      8.74 μs |      7.30 μs |  0.07 |    0.00 |          - |          - |          - |       14.06 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **16777216**  | **False**       |   **280,398.1 μs** |  **5,245.68 μs** |  **4,906.81 μs** |  **1.00** |    **0.02** | **26500.0000** | **26500.0000** | **26500.0000** |  **1638226.95 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 16777216  | False       |    26,840.9 μs |    536.11 μs |    617.39 μs |  0.10 |    0.00 |   187.5000 |          - |          - |     3635.17 KB |       0.002 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 16777216  | False       |    29,056.1 μs |    494.74 μs |    438.58 μs |  0.10 |    0.00 |          - |          - |          - |       14.07 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **16777216**  | **True**        |   **283,307.6 μs** |  **5,506.57 μs** |  **6,964.04 μs** |  **1.00** |    **0.03** | **74000.0000** | **74000.0000** | **74000.0000** |  **1638242.21 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 16777216  | True        |    41,263.6 μs |    793.92 μs |    779.74 μs |  0.15 |    0.00 |   153.8462 |          - |          - |     3635.19 KB |       0.002 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 16777216  | True        |    45,778.9 μs |    682.74 μs |    638.64 μs |  0.16 |    0.00 |          - |          - |          - |        14.1 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **100597760** | **False**       | **1,815,965.8 μs** | **23,980.37 μs** | **22,431.25 μs** |  **1.00** |    **0.02** | **29000.0000** | **29000.0000** | **29000.0000** | **13107031.48 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 100597760 | False       |   459,120.2 μs |  4,476.40 μs |  3,968.21 μs |  0.25 |    0.00 |  6000.0000 |          - |          - |   117458.98 KB |       0.009 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 100597760 | False       |   482,377.9 μs |  6,590.20 μs |  5,145.20 μs |  0.27 |    0.00 |          - |          - |          - |       14.45 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **100597760** | **True**        | **1,807,403.2 μs** | **11,208.85 μs** | **10,484.77 μs** |  **1.00** |    **0.01** | **29000.0000** | **29000.0000** | **29000.0000** | **13107031.48 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 100597760 | True        |   722,276.6 μs | 12,030.32 μs | 11,253.16 μs |  0.40 |    0.01 |  6000.0000 |          - |          - |   117458.66 KB |       0.009 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 100597760 | True        |   741,095.3 μs | 14,792.65 μs | 17,035.23 μs |  0.41 |    0.01 |          - |          - |          - |       14.45 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **209715200** | **False**       | **3,441,390.4 μs** | **22,283.38 μs** | **20,843.89 μs** |  **1.00** |    **0.01** | **30000.0000** | **30000.0000** | **30000.0000** | **26214232.98 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 209715200 | False       | 1,069,621.8 μs | 19,156.30 μs | 17,918.82 μs |  0.31 |    0.01 | 27000.0000 |  4000.0000 |          - |   505323.05 KB |       0.019 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 209715200 | False       | 1,098,957.3 μs | 12,552.72 μs | 11,127.66 μs |  0.32 |    0.00 |          - |          - |          - |       14.45 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **209715200** | **True**        | **3,412,609.9 μs** | **16,188.89 μs** | **14,351.03 μs** |  **1.00** |    **0.01** | **30000.0000** | **30000.0000** | **30000.0000** | **26214232.98 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 209715200 | True        | 1,571,388.3 μs | 15,013.19 μs | 14,043.34 μs |  0.46 |    0.00 | 27000.0000 |  4000.0000 |          - |   505323.05 KB |       0.019 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 209715200 | True        | 1,654,859.3 μs | 19,586.67 μs | 18,321.39 μs |  0.48 |    0.01 |          - |          - |          - |       14.45 KB |       0.000 |
