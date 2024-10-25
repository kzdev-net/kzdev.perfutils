```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method                                        | ZeroBuffers | ExponentialBufferGrowth | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated     | Alloc Ratio |
|---------------------------------------------- |------------ |------------------------ |------------:|----------:|----------:|------:|--------:|-----------:|-----------:|-----------:|--------------:|------------:|
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **False**                   |   **517.23 ms** | **10.047 ms** | **10.318 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.86 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | False                   | 2,054.24 ms | 17.069 ms | 15.966 ms |  3.97 |    0.08 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | False                   |    98.48 ms |  1.957 ms |  2.254 ms |  0.19 |    0.01 |          - |          - |          - |    6247.38 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **True**                    |   **526.32 ms** |  **8.146 ms** |  **7.620 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.81 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | True                    |   193.30 ms |  3.844 ms |  4.427 ms |  0.37 |    0.01 |          - |          - |          - |       19.6 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | True                    |    93.86 ms |  1.586 ms |  1.484 ms |  0.18 |    0.00 |          - |          - |          - |    6247.33 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **False**                   |   **522.99 ms** |  **7.341 ms** |  **6.866 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | False                   | 3,994.58 ms | 23.239 ms | 21.738 ms |  7.64 |    0.11 |          - |          - |          - |      73.31 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | False                   |   144.28 ms |  2.770 ms |  3.298 ms |  0.28 |    0.01 |          - |          - |          - |    6247.41 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **True**                    |   **527.70 ms** | **10.026 ms** |  **9.378 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.81 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | True                    |   370.41 ms |  3.702 ms |  3.282 ms |  0.70 |    0.01 |          - |          - |          - |      19.86 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | True                    |   141.66 ms |  1.979 ms |  1.852 ms |  0.27 |    0.01 |          - |          - |          - |    6247.41 KB |       0.002 |
