```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                        | ZeroBuffers | ExponentialBufferGrowth | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated     | Alloc Ratio |
|---------------------------------------------- |------------ |------------------------ |------------:|----------:|----------:|------:|--------:|-----------:|-----------:|-----------:|--------------:|------------:|
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **False**                   |   **437.15 ms** |  **6.667 ms** |  **5.910 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** |  **3496975.6 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | False                   | 1,995.68 ms | 18.776 ms | 17.564 ms |  4.57 |    0.07 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | False                   |    90.91 ms |  1.784 ms |  3.030 ms |  0.21 |    0.01 |          - |          - |          - |    6247.75 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **True**                    |   **435.53 ms** |  **5.438 ms** |  **4.821 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.63 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | True                    |   187.23 ms |  3.669 ms |  6.029 ms |  0.43 |    0.01 |          - |          - |          - |       19.5 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | True                    |    90.28 ms |  1.739 ms |  1.626 ms |  0.21 |    0.00 |          - |          - |          - |    6247.75 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **False**                   |   **435.19 ms** |  **4.666 ms** |  **4.364 ms** |  **1.00** |    **0.01** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.63 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | False                   | 3,958.35 ms | 15.603 ms | 14.595 ms |  9.10 |    0.09 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | False                   |   141.67 ms |  2.822 ms |  5.160 ms |  0.33 |    0.01 |          - |          - |          - |    6247.79 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **True**                    |   **436.13 ms** |  **8.499 ms** |  **9.093 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.63 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | True                    |   363.51 ms |  7.220 ms |  9.132 ms |  0.83 |    0.03 |          - |          - |          - |      19.58 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | True                    |   141.54 ms |  2.817 ms |  4.217 ms |  0.32 |    0.01 |          - |          - |          - |    6247.71 KB |       0.002 |
