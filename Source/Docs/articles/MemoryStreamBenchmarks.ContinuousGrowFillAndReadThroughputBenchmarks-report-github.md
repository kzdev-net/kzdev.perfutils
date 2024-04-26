```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method                                        | ZeroBuffers | ExponentialBufferGrowth | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated     | Alloc Ratio |
|---------------------------------------------- |------------ |------------------------ |------------:|----------:|----------:|------:|--------:|-----------:|-----------:|-----------:|--------------:|------------:|
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **False**                   |   **517.92 ms** | **10.113 ms** |  **9.460 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.86 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | False                   | 1,994.67 ms | 19.647 ms | 18.378 ms |  3.85 |    0.08 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | False                   |    96.10 ms |  1.530 ms |  1.431 ms |  0.19 |    0.00 |          - |          - |          - |    6247.39 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **True**                    |   **511.74 ms** |  **6.960 ms** |  **6.170 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.88 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | True                    |   190.17 ms |  3.758 ms |  6.175 ms |  0.37 |    0.01 |          - |          - |          - |       19.6 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | True                    |    98.83 ms |  1.939 ms |  3.345 ms |  0.19 |    0.01 |          - |          - |          - |    6247.39 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **False**                   |   **520.97 ms** |  **8.836 ms** |  **8.265 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.86 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | False                   | 3,934.99 ms | 20.300 ms | 18.989 ms |  7.55 |    0.12 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | False                   |   145.29 ms |  2.854 ms |  4.183 ms |  0.28 |    0.01 |          - |          - |          - |    6247.41 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **True**                    |   **506.72 ms** | **10.088 ms** |  **9.908 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | True                    |   360.09 ms |  4.884 ms |  4.568 ms |  0.71 |    0.02 |          - |          - |          - |      19.86 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | True                    |   146.15 ms |  2.854 ms |  4.272 ms |  0.29 |    0.01 |          - |          - |          - |    6247.41 KB |       0.002 |
