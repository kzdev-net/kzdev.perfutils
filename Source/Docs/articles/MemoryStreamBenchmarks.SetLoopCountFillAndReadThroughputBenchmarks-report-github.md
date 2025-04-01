```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                                | DataSize  | ZeroBuffers | Mean            | Error         | StdDev        | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated      | Alloc Ratio |
|------------------------------------------------------ |---------- |------------ |----------------:|--------------:|--------------:|------:|--------:|-----------:|-----------:|-----------:|---------------:|------------:|
| **&#39;MemoryStream set loop count fill and read&#39;**           | **131072**    | **False**       |     **1,006.36 μs** |      **5.858 μs** |      **5.193 μs** |  **1.00** |    **0.01** |  **1041.0156** |  **1041.0156** |  **1041.0156** |     **6305.42 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 131072    | False       |        70.82 μs |      0.134 μs |      0.104 μs |  0.07 |    0.00 |     0.3662 |          - |          - |        6.84 KB |       0.001 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 131072    | False       |        88.99 μs |      0.432 μs |      0.383 μs |  0.09 |    0.00 |     0.3662 |          - |          - |        7.42 KB |       0.001 |
|                                                       |           |             |                 |               |               |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **131072**    | **True**        |     **1,007.39 μs** |      **4.353 μs** |      **3.859 μs** |  **1.00** |    **0.01** |  **1041.0156** |  **1041.0156** |  **1041.0156** |     **6305.42 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 131072    | True        |       106.40 μs |      0.610 μs |      0.541 μs |  0.11 |    0.00 |     0.3662 |          - |          - |        6.84 KB |       0.001 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 131072    | True        |       135.00 μs |      0.501 μs |      0.444 μs |  0.13 |    0.00 |     0.2441 |          - |          - |        7.42 KB |       0.001 |
|                                                       |           |             |                 |               |               |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **16777216**  | **False**       |   **134,769.13 μs** |  **2,638.109 μs** |  **4,479.710 μs** |  **1.00** |    **0.05** | **37600.0000** | **37600.0000** | **37600.0000** |   **819121.24 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 16777216  | False       |    15,959.39 μs |    168.795 μs |    157.891 μs |  0.12 |    0.00 |    93.7500 |          - |          - |     1817.59 KB |       0.002 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 16777216  | False       |    15,314.08 μs |    139.620 μs |    130.601 μs |  0.11 |    0.00 |          - |          - |          - |        7.43 KB |       0.000 |
|                                                       |           |             |                 |               |               |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **16777216**  | **True**        |   **134,475.88 μs** |  **1,085.410 μs** |    **962.188 μs** |  **1.00** |    **0.01** | **37600.0000** | **37600.0000** | **37600.0000** |    **819121.3 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 16777216  | True        |    23,108.79 μs |    212.224 μs |    188.131 μs |  0.17 |    0.00 |    93.7500 |          - |          - |     1817.59 KB |       0.002 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 16777216  | True        |    22,486.14 μs |    191.408 μs |    169.678 μs |  0.17 |    0.00 |          - |          - |          - |        7.43 KB |       0.000 |
|                                                       |           |             |                 |               |               |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **209715200** | **False**       | **1,643,271.96 μs** | **19,251.826 μs** | **18,008.170 μs** |  **1.00** |    **0.02** | **18000.0000** | **18000.0000** | **18000.0000** | **13107117.38 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 209715200 | False       |   532,794.46 μs | 10,466.152 μs | 14,672.085 μs |  0.32 |    0.01 | 13000.0000 |  2000.0000 |          - |   252661.72 KB |       0.019 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 209715200 | False       |   498,798.79 μs |  9,761.817 μs | 14,000.107 μs |  0.30 |    0.01 |          - |          - |          - |        7.81 KB |       0.000 |
|                                                       |           |             |                 |               |               |       |         |            |            |            |                |             |
| **&#39;MemoryStream set loop count fill and read&#39;**           | **209715200** | **True**        | **1,632,078.79 μs** | **18,444.671 μs** | **17,253.157 μs** |  **1.00** |    **0.01** | **18000.0000** | **18000.0000** | **18000.0000** | **13107117.66 KB** |       **1.000** |
| &#39;RecyclableMemoryStream set loop count fill and read&#39; | 209715200 | True        |   800,268.48 μs | 14,847.050 μs | 13,887.940 μs |  0.49 |    0.01 | 13000.0000 |  2000.0000 |          - |   252661.39 KB |       0.019 |
| &#39;MemoryStreamSlim set loop count fill and read&#39;       | 209715200 | True        |   784,353.33 μs | 15,166.048 μs | 18,054.102 μs |  0.48 |    0.01 |          - |          - |          - |        7.81 KB |       0.000 |
