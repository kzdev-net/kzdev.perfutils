```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2


```
| Method                                        | ZeroBuffers | ExponentialBufferGrowth | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated     | Alloc Ratio |
|---------------------------------------------- |------------ |------------------------ |------------:|----------:|----------:|------:|--------:|-----------:|-----------:|-----------:|--------------:|------------:|
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **False**                   |   **524.65 ms** | **10.262 ms** |  **9.599 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.79 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | False                   | 2,025.67 ms |  8.172 ms |  7.644 ms |  3.86 |    0.07 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | False                   |    98.26 ms |  1.911 ms |  2.417 ms |  0.19 |    0.01 |          - |          - |          - |    6247.38 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **False**       | **True**                    |   **518.24 ms** |  **9.831 ms** |  **9.656 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.84 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | False       | True                    |   194.18 ms |  3.718 ms |  3.978 ms |  0.37 |    0.01 |          - |          - |          - |       19.6 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | False       | True                    |    97.41 ms |  1.935 ms |  2.304 ms |  0.19 |    0.01 |          - |          - |          - |    6247.38 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **False**                   |   **525.31 ms** |  **8.007 ms** |  **7.490 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.86 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | False                   | 3,984.86 ms | 16.415 ms | 13.707 ms |  7.59 |    0.11 |          - |          - |          - |      73.64 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | False                   |   140.64 ms |  2.369 ms |  2.327 ms |  0.27 |    0.01 |          - |          - |          - |    6247.41 KB |       0.002 |
|                                               |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**           | **True**        | **True**                    |   **524.13 ms** |  **9.965 ms** |  **9.321 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.86 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read&#39; | True        | True                    |   364.83 ms |  6.461 ms |  6.044 ms |  0.70 |    0.02 |          - |          - |          - |      19.86 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read&#39;       | True        | True                    |   140.76 ms |  2.788 ms |  3.424 ms |  0.27 |    0.01 |          - |          - |          - |    6247.41 KB |       0.002 |
