```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8037/25H2/2025Update/HudsonValley2)
Intel Core i9-14900K 3.20GHz, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3


```
| Method                                                    | ZeroBuffers | ExponentialBufferGrowth | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Gen2       | Allocated     | Alloc Ratio |
|---------------------------------------------------------- |------------ |------------------------ |------------:|----------:|----------:|------:|--------:|-----------:|-----------:|-----------:|--------------:|------------:|
| **&#39;MemoryStream growth fill and read&#39;**                       | **False**       | **False**                   |   **531.72 ms** | **10.172 ms** | **21.896 ms** |  **1.00** |    **0.06** | **37000.0000** | **37000.0000** | **37000.0000** | **3496981.23 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read (GetBuffer)&#39; | False       | False                   | 2,039.65 ms | 15.581 ms | 13.812 ms |  3.84 |    0.16 |          - |          - |          - |      73.25 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read (ToMemory)&#39;        | False       | False                   |    96.91 ms |  1.838 ms |  3.837 ms |  0.18 |    0.01 |          - |          - |          - |       8.44 KB |       0.000 |
|                                                           |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**                       | **False**       | **True**                    |   **450.36 ms** |  **7.620 ms** |  **6.363 ms** |  **1.00** |    **0.02** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.23 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read (GetBuffer)&#39; | False       | True                    |   197.55 ms |  3.913 ms |  7.156 ms |  0.44 |    0.02 |          - |          - |          - |      19.47 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read (ToMemory)&#39;        | False       | True                    |    96.33 ms |  1.924 ms |  4.498 ms |  0.21 |    0.01 |          - |          - |          - |       8.44 KB |       0.000 |
|                                                           |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**                       | **True**        | **False**                   |   **451.95 ms** |  **8.932 ms** | **10.286 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.19 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read (GetBuffer)&#39; | True        | False                   | 4,016.96 ms | 22.319 ms | 20.877 ms |  8.89 |    0.20 |          - |          - |          - |      73.25 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read (ToMemory)&#39;        | True        | False                   |   145.68 ms |  2.909 ms |  4.528 ms |  0.32 |    0.01 |          - |          - |          - |       8.44 KB |       0.000 |
|                                                           |             |                         |             |           |           |       |         |            |            |            |               |             |
| **&#39;MemoryStream growth fill and read&#39;**                       | **True**        | **True**                    |   **447.30 ms** |  **8.656 ms** |  **8.889 ms** |  **1.00** |    **0.03** | **17000.0000** | **17000.0000** | **17000.0000** | **3496975.26 KB** |       **1.000** |
| &#39;RecyclableMemoryStream growth fill and read (GetBuffer)&#39; | True        | True                    |   375.62 ms |  7.399 ms | 10.611 ms |  0.84 |    0.03 |          - |          - |          - |      19.47 KB |       0.000 |
| &#39;MemoryStreamSlim growth fill and read (ToMemory)&#39;        | True        | True                    |   146.68 ms |  2.867 ms |  4.789 ms |  0.33 |    0.01 |          - |          - |          - |       8.44 KB |       0.000 |
