```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                    | DataSize  | CapacityOnCreate | Mean          | Error      | StdDev     | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated     | Alloc Ratio |
|------------------------------------------ |---------- |----------------- |--------------:|-----------:|-----------:|------:|--------:|----------:|----------:|----------:|--------------:|------------:|
| **&#39;MemoryStream GZip Compression&#39;**           | **131072**    | **False**            |     **10.099 ms** |  **0.1433 ms** |  **0.1341 ms** |  **1.00** |    **0.02** |  **609.3750** |  **609.3750** |  **609.3750** |    **2522.56 KB** |       **1.000** |
| &#39;RecyclableMemoryStream GZip Compression&#39; | 131072    | False            |      9.561 ms |  0.0615 ms |  0.0480 ms |  0.95 |    0.01 |         - |         - |         - |       2.67 KB |       0.001 |
| &#39;MemoryStreamSlim GZip Compression&#39;       | 131072    | False            |      9.535 ms |  0.0658 ms |  0.0616 ms |  0.94 |    0.01 |         - |         - |         - |       2.63 KB |       0.001 |
|                                           |           |                  |               |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream GZip Compression&#39;**           | **131072**    | **True**             |      **9.888 ms** |  **0.0737 ms** |  **0.0689 ms** |  **1.00** |    **0.01** |  **609.3750** |  **609.3750** |  **609.3750** |    **1922.09 KB** |       **1.000** |
| &#39;RecyclableMemoryStream GZip Compression&#39; | 131072    | True             |      9.522 ms |  0.0519 ms |  0.0485 ms |  0.96 |    0.01 |         - |         - |         - |       2.67 KB |       0.001 |
| &#39;MemoryStreamSlim GZip Compression&#39;       | 131072    | True             |      9.550 ms |  0.0493 ms |  0.0462 ms |  0.97 |    0.01 |         - |         - |         - |       2.62 KB |       0.001 |
|                                           |           |                  |               |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream GZip Compression&#39;**           | **16777216**  | **False**            |  **1,308.009 ms** |  **3.9998 ms** |  **3.7414 ms** |  **1.00** |    **0.00** | **3000.0000** | **3000.0000** | **3000.0000** |  **327645.35 KB** |       **1.000** |
| &#39;RecyclableMemoryStream GZip Compression&#39; | 16777216  | False            |  1,271.541 ms |  4.9122 ms |  4.5948 ms |  0.97 |    0.00 |         - |         - |         - |     365.52 KB |       0.001 |
| &#39;MemoryStreamSlim GZip Compression&#39;       | 16777216  | False            |  1,270.226 ms |  7.1360 ms |  6.6750 ms |  0.97 |    0.01 |         - |         - |         - |       3.34 KB |       0.000 |
|                                           |           |                  |               |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream GZip Compression&#39;**           | **16777216**  | **True**             |  **1,288.504 ms** |  **3.8238 ms** |  **3.5768 ms** |  **1.00** |    **0.00** | **1000.0000** | **1000.0000** | **1000.0000** |  **245763.05 KB** |       **1.000** |
| &#39;RecyclableMemoryStream GZip Compression&#39; | 16777216  | True             |  1,271.058 ms |  3.1034 ms |  2.7510 ms |  0.99 |    0.00 |         - |         - |         - |      28.18 KB |       0.000 |
| &#39;MemoryStreamSlim GZip Compression&#39;       | 16777216  | True             |  1,274.414 ms |  5.5392 ms |  5.1814 ms |  0.99 |    0.00 |         - |         - |         - |       3.05 KB |       0.000 |
|                                           |           |                  |               |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream GZip Compression&#39;**           | **209715200** | **False**            | **16,086.608 ms** | **32.7362 ms** | **30.6215 ms** |  **1.00** |    **0.00** | **3000.0000** | **3000.0000** | **3000.0000** | **2621405.98 KB** |       **1.000** |
| &#39;RecyclableMemoryStream GZip Compression&#39; | 209715200 | False            | 15,858.875 ms | 41.2183 ms | 38.5556 ms |  0.99 |    0.00 | 2000.0000 |         - |         - |   50534.27 KB |       0.019 |
| &#39;MemoryStreamSlim GZip Compression&#39;       | 209715200 | False            | 15,866.391 ms | 26.9045 ms | 25.1665 ms |  0.99 |    0.00 |         - |         - |         - |       3.34 KB |       0.000 |
|                                           |           |                  |               |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream GZip Compression&#39;**           | **209715200** | **True**             | **16,085.437 ms** | **34.7819 ms** | **27.1554 ms** |  **1.00** |    **0.00** | **1000.0000** | **1000.0000** | **1000.0000** | **3072002.77 KB** |       **1.000** |
| &#39;RecyclableMemoryStream GZip Compression&#39; | 209715200 | True             | 15,838.077 ms | 25.3705 ms | 23.7316 ms |  0.98 |    0.00 |         - |         - |         - |     315.68 KB |       0.000 |
| &#39;MemoryStreamSlim GZip Compression&#39;       | 209715200 | True             | 15,865.440 ms | 33.1069 ms | 30.9682 ms |  0.99 |    0.00 |         - |         - |         - |       3.71 KB |       0.000 |
