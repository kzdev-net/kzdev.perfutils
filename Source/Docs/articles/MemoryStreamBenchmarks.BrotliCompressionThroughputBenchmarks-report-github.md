```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                            | DataSize  | CapacityOnCreate | Mean           | Error        | StdDev       | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated     | Alloc Ratio |
|-------------------------------------------------- |---------- |----------------- |---------------:|-------------:|-------------:|------:|--------:|----------:|----------:|----------:|--------------:|------------:|
| **&#39;MemoryStream BrotliStream Compression&#39;**           | **131072**    | **False**            |     **1,265.1 μs** |     **16.01 μs** |     **14.19 μs** |  **1.00** |    **0.02** |  **623.0469** |  **623.0469** |  **623.0469** |    **2241.79 KB** |       **1.000** |
| &#39;RecyclableMemoryStream BrotliStream Compression&#39; | 131072    | False            |       797.4 μs |      7.18 μs |      6.72 μs |  0.63 |    0.01 |         - |         - |         - |       2.11 KB |       0.001 |
| &#39;MemoryStreamSlim BrotliStream Compression&#39;       | 131072    | False            |       804.5 μs |      2.77 μs |      2.59 μs |  0.64 |    0.01 |         - |         - |         - |       2.07 KB |       0.001 |
|                                                   |           |                  |                |              |              |       |         |           |           |           |               |             |
| **&#39;MemoryStream BrotliStream Compression&#39;**           | **131072**    | **True**             |     **1,166.3 μs** |      **6.63 μs** |      **6.20 μs** |  **1.00** |    **0.01** |  **623.0469** |  **623.0469** |  **623.0469** |    **1921.67 KB** |       **1.000** |
| &#39;RecyclableMemoryStream BrotliStream Compression&#39; | 131072    | True             |       788.5 μs |      7.57 μs |      7.08 μs |  0.68 |    0.01 |         - |         - |         - |       2.11 KB |       0.001 |
| &#39;MemoryStreamSlim BrotliStream Compression&#39;       | 131072    | True             |       784.9 μs |      2.72 μs |      2.54 μs |  0.67 |    0.00 |         - |         - |         - |       2.07 KB |       0.001 |
|                                                   |           |                  |                |              |              |       |         |           |           |           |               |             |
| **&#39;MemoryStream BrotliStream Compression&#39;**           | **16777216**  | **False**            |   **160,363.5 μs** |  **3,118.43 μs** |  **5,123.67 μs** |  **1.00** |    **0.04** | **4666.6667** | **4666.6667** | **4666.6667** |  **327365.88 KB** |       **1.000** |
| &#39;RecyclableMemoryStream BrotliStream Compression&#39; | 16777216  | False            |   122,424.4 μs |  1,118.25 μs |    991.30 μs |  0.76 |    0.02 |         - |         - |         - |     364.39 KB |       0.001 |
| &#39;MemoryStreamSlim BrotliStream Compression&#39;       | 16777216  | False            |   123,307.7 μs |  1,845.81 μs |  1,636.26 μs |  0.77 |    0.03 |         - |         - |         - |       2.23 KB |       0.000 |
|                                                   |           |                  |                |              |              |       |         |           |           |           |               |             |
| **&#39;MemoryStream BrotliStream Compression&#39;**           | **16777216**  | **True**             |   **144,735.8 μs** |  **1,935.22 μs** |  **1,510.89 μs** |  **1.00** |    **0.01** | **4750.0000** | **4750.0000** | **4750.0000** |  **245767.18 KB** |       **1.000** |
| &#39;RecyclableMemoryStream BrotliStream Compression&#39; | 16777216  | True             |   123,696.0 μs |  2,400.16 μs |  2,245.11 μs |  0.85 |    0.02 |         - |         - |         - |      27.04 KB |       0.000 |
| &#39;MemoryStreamSlim BrotliStream Compression&#39;       | 16777216  | True             |   122,721.4 μs |  1,680.01 μs |  1,489.28 μs |  0.85 |    0.01 |         - |         - |         - |        2.3 KB |       0.000 |
|                                                   |           |                  |                |              |              |       |         |           |           |           |               |             |
| **&#39;MemoryStream BrotliStream Compression&#39;**           | **209715200** | **False**            | **1,657,996.1 μs** | **17,018.54 μs** | **15,919.15 μs** |  **1.00** |    **0.01** | **3000.0000** | **3000.0000** | **3000.0000** | **2621125.72 KB** |       **1.000** |
| &#39;RecyclableMemoryStream BrotliStream Compression&#39; | 209715200 | False            | 1,412,500.9 μs | 25,484.85 μs | 23,838.54 μs |  0.85 |    0.02 | 2000.0000 |         - |         - |   50534.08 KB |       0.019 |
| &#39;MemoryStreamSlim BrotliStream Compression&#39;       | 209715200 | False            | 1,394,513.0 μs | 11,134.35 μs |  9,297.68 μs |  0.84 |    0.01 |         - |         - |         - |          3 KB |       0.000 |
|                                                   |           |                  |                |              |              |       |         |           |           |           |               |             |
| **&#39;MemoryStream BrotliStream Compression&#39;**           | **209715200** | **True**             | **1,623,939.0 μs** | **18,348.17 μs** | **17,162.89 μs** |  **1.00** |    **0.01** | **1000.0000** | **1000.0000** | **1000.0000** | **3072002.65 KB** |       **1.000** |
| &#39;RecyclableMemoryStream BrotliStream Compression&#39; | 209715200 | True             | 1,393,359.5 μs | 17,534.00 μs | 15,543.43 μs |  0.86 |    0.01 |         - |         - |         - |     315.02 KB |       0.000 |
| &#39;MemoryStreamSlim BrotliStream Compression&#39;       | 209715200 | True             | 1,390,654.6 μs | 15,145.63 μs | 14,167.23 μs |  0.86 |    0.01 |         - |         - |         - |       2.72 KB |       0.000 |
