```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                               | DataSize  | Mean          | Error        | StdDev       | Median        | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------------------------------- |---------- |--------------:|-------------:|-------------:|--------------:|------:|--------:|-------:|----------:|------------:|
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **131072**    |      **12.93 μs** |     **0.041 μs** |     **0.039 μs** |      **12.92 μs** |  **1.00** |    **0.00** | **0.0153** |     **320 B** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 131072    |      21.81 μs |     0.075 μs |     0.083 μs |      21.81 μs |  1.69 |    0.01 | 0.0610 |    1400 B |        4.38 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 131072    |      12.58 μs |     0.050 μs |     0.047 μs |      12.56 μs |  0.97 |    0.00 | 0.0763 |    1440 B |        4.50 |
|                                                      |           |               |              |              |               |       |         |        |           |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **983040**    |      **97.62 μs** |     **0.582 μs** |     **0.544 μs** |      **97.53 μs** |  **1.00** |    **0.01** |      **-** |     **320 B** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 983040    |     196.72 μs |     0.987 μs |     0.875 μs |     196.89 μs |  2.02 |    0.01 |      - |    2760 B |        8.62 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 983040    |      90.47 μs |     0.825 μs |     0.771 μs |      90.55 μs |  0.93 |    0.01 |      - |    1440 B |        4.50 |
|                                                      |           |               |              |              |               |       |         |        |           |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **16777216**  |   **2,858.21 μs** |    **27.081 μs** |    **25.332 μs** |   **2,856.75 μs** |  **1.00** |    **0.01** |      **-** |     **322 B** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 16777216  |   7,353.10 μs |   102.158 μs |    90.561 μs |   7,328.03 μs |  2.57 |    0.04 |      - |   26803 B |       83.24 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 16777216  |   2,940.73 μs |    32.737 μs |    30.622 μs |   2,945.06 μs |  1.03 |    0.01 |      - |    1442 B |        4.48 |
|                                                      |           |               |              |              |               |       |         |        |           |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **100597760** |  **49,116.09 μs** |   **972.633 μs** | **2,030.248 μs** |  **48,832.93 μs** |  **1.00** |    **0.06** |      **-** |     **356 B** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 100597760 |  85,172.71 μs | 1,587.355 μs | 1,484.813 μs |  85,462.35 μs |  1.74 |    0.08 |      - |  154827 B |      434.91 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 100597760 |  44,512.66 μs |   860.848 μs | 2,371.025 μs |  43,508.22 μs |  0.91 |    0.06 |      - |    1473 B |        4.14 |
|                                                      |           |               |              |              |               |       |         |        |           |             |
| **&#39;MemoryStream array wrapper fill and read&#39;**           | **209715200** | **106,800.53 μs** | **2,102.541 μs** | **4,342.113 μs** | **105,549.30 μs** |  **1.00** |    **0.06** |      **-** |     **400 B** |        **1.00** |
| &#39;RecyclableMemoryStream array wrapper fill and read&#39; | 209715200 | 179,092.79 μs | 1,881.964 μs | 1,571.525 μs | 178,662.53 μs |  1.68 |    0.07 |      - |  321221 B |      803.05 |
| &#39;MemoryStreamSlim array wrapper fill and read&#39;       | 209715200 |  98,981.43 μs | 1,870.470 μs | 3,862.846 μs |  98,050.22 μs |  0.93 |    0.05 |      - |    1462 B |        3.65 |
