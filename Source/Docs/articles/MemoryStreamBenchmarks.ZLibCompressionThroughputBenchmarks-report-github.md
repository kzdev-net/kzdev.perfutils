```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                    | DataSize  | CapacityOnCreate | Mean          | Error      | StdDev     | Ratio | Gen0      | Gen1      | Gen2      | Allocated     | Alloc Ratio |
|------------------------------------------ |---------- |----------------- |--------------:|-----------:|-----------:|------:|----------:|----------:|----------:|--------------:|------------:|
| **&#39;MemoryStream ZLib Compression&#39;**           | **131072**    | **False**            |      **9.991 ms** |  **0.0547 ms** |  **0.0485 ms** |  **1.00** |  **609.3750** |  **609.3750** |  **609.3750** |    **2522.56 KB** |       **1.000** |
| &#39;RecyclableMemoryStream ZLib Compression&#39; | 131072    | False            |      9.535 ms |  0.0606 ms |  0.0537 ms |  0.95 |         - |         - |         - |       2.66 KB |       0.001 |
| &#39;MemoryStreamSlim ZLib Compression&#39;       | 131072    | False            |      9.691 ms |  0.0703 ms |  0.0658 ms |  0.97 |         - |         - |         - |       2.63 KB |       0.001 |
|                                           |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream ZLib Compression&#39;**           | **131072**    | **True**             |     **10.015 ms** |  **0.0580 ms** |  **0.0543 ms** |  **1.00** |  **609.3750** |  **609.3750** |  **609.3750** |    **1922.09 KB** |       **1.000** |
| &#39;RecyclableMemoryStream ZLib Compression&#39; | 131072    | True             |      9.587 ms |  0.0334 ms |  0.0279 ms |  0.96 |         - |         - |         - |       2.66 KB |       0.001 |
| &#39;MemoryStreamSlim ZLib Compression&#39;       | 131072    | True             |      9.602 ms |  0.0710 ms |  0.0664 ms |  0.96 |         - |         - |         - |       2.63 KB |       0.001 |
|                                           |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream ZLib Compression&#39;**           | **16777216**  | **False**            |  **1,312.773 ms** |  **6.2521 ms** |  **5.8482 ms** |  **1.00** | **3000.0000** | **3000.0000** | **3000.0000** |  **327645.63 KB** |       **1.000** |
| &#39;RecyclableMemoryStream ZLib Compression&#39; | 16777216  | False            |  1,276.056 ms |  5.1711 ms |  4.8371 ms |  0.97 |         - |         - |         - |     365.52 KB |       0.001 |
| &#39;MemoryStreamSlim ZLib Compression&#39;       | 16777216  | False            |  1,275.155 ms |  7.9215 ms |  7.4098 ms |  0.97 |         - |         - |         - |       3.05 KB |       0.000 |
|                                           |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream ZLib Compression&#39;**           | **16777216**  | **True**             |  **1,290.594 ms** |  **7.9316 ms** |  **7.4193 ms** |  **1.00** | **1000.0000** | **1000.0000** | **1000.0000** |  **245763.05 KB** |       **1.000** |
| &#39;RecyclableMemoryStream ZLib Compression&#39; | 16777216  | True             |  1,270.175 ms |  4.3321 ms |  4.0523 ms |  0.98 |         - |         - |         - |       27.9 KB |       0.000 |
| &#39;MemoryStreamSlim ZLib Compression&#39;       | 16777216  | True             |  1,269.116 ms |  3.1212 ms |  2.9195 ms |  0.98 |         - |         - |         - |       3.34 KB |       0.000 |
|                                           |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream ZLib Compression&#39;**           | **209715200** | **False**            | **16,089.546 ms** | **54.9184 ms** | **51.3707 ms** |  **1.00** | **3000.0000** | **3000.0000** | **3000.0000** | **2621405.98 KB** |       **1.000** |
| &#39;RecyclableMemoryStream ZLib Compression&#39; | 209715200 | False            | 15,873.406 ms | 63.7087 ms | 56.4761 ms |  0.99 | 2000.0000 |         - |         - |   50534.93 KB |       0.019 |
| &#39;MemoryStreamSlim ZLib Compression&#39;       | 209715200 | False            | 15,865.850 ms | 41.1266 ms | 38.4698 ms |  0.99 |         - |         - |         - |       3.34 KB |       0.000 |
|                                           |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream ZLib Compression&#39;**           | **209715200** | **True**             | **16,028.777 ms** | **20.8391 ms** | **19.4929 ms** |  **1.00** | **1000.0000** | **1000.0000** | **1000.0000** | **3072003.05 KB** |       **1.000** |
| &#39;RecyclableMemoryStream ZLib Compression&#39; | 209715200 | True             | 15,842.508 ms | 24.4629 ms | 21.6857 ms |  0.99 |         - |         - |         - |      315.4 KB |       0.000 |
| &#39;MemoryStreamSlim ZLib Compression&#39;       | 209715200 | True             | 15,853.586 ms | 52.8778 ms | 49.4619 ms |  0.99 |         - |         - |         - |       3.05 KB |       0.000 |
