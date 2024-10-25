```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method                                                | DataSize  | ZeroBuffers | Mean           | Error        | StdDev       | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated      | Alloc Ratio |
|------------------------------------------------------ |---------- |------------ |---------------:|-------------:|-------------:|------:|--------:|-----------:|-----------:|-----------:|---------------:|------------:|
| **&#39;MemoryStream set loop count fill and read&#39;**           | **131072**    | **False**       |     **2,189.0 μs** |     **12.78 μs** |     **11.95 μs** |  **1.00** |    **0.01** |  **2082.0313** |  **2082.0313** |  **2082.0313** |    **12610.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 131072    | False       |       129.8 μs |      0.75 μs |      0.70 μs |  0.06 |    0.00 |     0.7324 |          - |          - |       13.67 KB |       0.001 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 131072    | False       |       187.5 μs |      0.72 μs |      0.68 μs |  0.09 |    0.00 |     0.7324 |          - |          - |       14.06 KB |       0.001 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **131072**    | **True**        |     **2,188.4 μs** |     **16.82 μs** |     **15.74 μs** |  **1.00** |    **0.01** |  **2082.0313** |  **2082.0313** |  **2082.0313** |    **12610.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 131072    | True        |       208.5 μs |      1.44 μs |      1.34 μs |  0.10 |    0.00 |     0.7324 |          - |          - |       13.67 KB |       0.001 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 131072    | True        |       313.4 μs |      1.74 μs |      1.63 μs |  0.14 |    0.00 |     0.4883 |          - |          - |       14.06 KB |       0.001 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **983040**    | **False**       |    **27,584.8 μs** |    **190.65 μs** |    **178.33 μs** |  **1.00** |    **0.01** | **24984.3750** | **24984.3750** | **24984.3750** |   **102221.88 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 983040    | False       |     1,000.8 μs |      4.44 μs |      4.15 μs |  0.04 |    0.00 |     1.9531 |          - |          - |       44.53 KB |       0.000 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 983040    | False       |     1,091.5 μs |      3.34 μs |      3.13 μs |  0.04 |    0.00 |          - |          - |          - |       14.06 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **983040**    | **True**        |    **27,679.0 μs** |    **154.97 μs** |    **144.96 μs** |  **1.00** |    **0.01** | **24984.3750** | **24984.3750** | **24984.3750** |   **102221.88 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 983040    | True        |     1,588.1 μs |     10.71 μs |     10.02 μs |  0.06 |    0.00 |     1.9531 |          - |          - |       44.53 KB |       0.000 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 983040    | True        |     1,687.7 μs |     13.90 μs |     13.00 μs |  0.06 |    0.00 |          - |          - |          - |       14.06 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **16777216**  | **False**       |   **284,612.3 μs** |  **5,420.56 μs** |  **5,323.71 μs** |  **1.00** |    **0.03** | **26500.0000** | **26500.0000** | **26500.0000** |  **1638226.95 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 16777216  | False       |    27,028.1 μs |    291.92 μs |    243.76 μs |  0.09 |    0.00 |   187.5000 |          - |          - |     3635.17 KB |       0.002 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 16777216  | False       |    27,946.7 μs |    549.89 μs |    514.37 μs |  0.10 |    0.00 |          - |          - |          - |       14.07 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **16777216**  | **True**        |   **286,288.4 μs** |  **5,095.76 μs** |  **4,766.58 μs** |  **1.00** |    **0.02** | **26500.0000** | **26500.0000** | **26500.0000** |  **1638226.95 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 16777216  | True        |    41,887.7 μs |    462.75 μs |    432.85 μs |  0.15 |    0.00 |   166.6667 |          - |          - |     3635.19 KB |       0.002 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 16777216  | True        |    42,174.3 μs |    687.10 μs |    642.72 μs |  0.15 |    0.00 |          - |          - |          - |       14.09 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **100597760** | **False**       | **1,835,445.0 μs** | **12,303.08 μs** | **11,508.31 μs** |  **1.00** |    **0.01** | **29000.0000** | **29000.0000** | **29000.0000** | **13107031.48 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 100597760 | False       |   475,595.6 μs |  6,113.80 μs |  5,718.85 μs |  0.26 |    0.00 |  6000.0000 |          - |          - |   117458.98 KB |       0.009 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 100597760 | False       |   488,067.7 μs |  9,665.53 μs | 12,223.80 μs |  0.27 |    0.01 |          - |          - |          - |       14.45 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **100597760** | **True**        | **1,819,228.7 μs** |  **6,967.87 μs** |  **6,176.83 μs** |  **1.00** |    **0.00** | **29000.0000** | **29000.0000** | **29000.0000** | **13107031.48 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 100597760 | True        |   737,674.5 μs | 10,244.95 μs |  9,583.13 μs |  0.41 |    0.01 |  6000.0000 |          - |          - |   117458.98 KB |       0.009 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 100597760 | True        |   723,232.2 μs | 12,026.20 μs | 11,249.31 μs |  0.40 |    0.01 |          - |          - |          - |       14.45 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **209715200** | **False**       | **3,496,994.0 μs** | **29,180.01 μs** | **27,295.00 μs** |  **1.00** |    **0.01** | **30000.0000** | **30000.0000** | **30000.0000** | **26214232.98 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 209715200 | False       | 1,084,895.6 μs | 21,629.28 μs | 32,373.69 μs |  0.31 |    0.01 | 27000.0000 |  4000.0000 |          - |   505323.05 KB |       0.019 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 209715200 | False       | 1,110,824.8 μs | 19,685.33 μs | 18,413.67 μs |  0.32 |    0.01 |          - |          - |          - |       14.45 KB |       0.000 |
|                                                       |           |             |                |              |              |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **209715200** | **True**        | **3,491,903.1 μs** | **31,207.73 μs** | **29,191.73 μs** |  **1.00** |    **0.01** | **30000.0000** | **30000.0000** | **30000.0000** | **26214232.98 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 209715200 | True        | 1,650,876.1 μs | 17,315.58 μs | 16,197.01 μs |  0.47 |    0.01 | 27000.0000 |  4000.0000 |          - |   505322.72 KB |       0.019 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 209715200 | True        | 1,601,021.2 μs | 13,674.48 μs | 11,418.81 μs |  0.46 |    0.00 |          - |          - |          - |       14.45 KB |       0.000 |
