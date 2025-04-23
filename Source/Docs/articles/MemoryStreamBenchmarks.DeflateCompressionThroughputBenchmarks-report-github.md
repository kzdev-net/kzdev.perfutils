```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                                       | DataSize  | CapacityOnCreate | Mean          | Error      | StdDev     | Ratio | Gen0      | Gen1      | Gen2      | Allocated     | Alloc Ratio |
|--------------------------------------------- |---------- |----------------- |--------------:|-----------:|-----------:|------:|----------:|----------:|----------:|--------------:|------------:|
| **&#39;MemoryStream Deflate Compression&#39;**           | **131072**    | **False**            |     **10.161 ms** |  **0.0373 ms** |  **0.0349 ms** |  **1.00** |  **609.3750** |  **609.3750** |  **609.3750** |     **2522.4 KB** |       **1.000** |
| &#39;RecyclableMemoryStream Deflate Compression&#39; | 131072    | False            |      9.501 ms |  0.0369 ms |  0.0327 ms |  0.94 |         - |         - |         - |       2.51 KB |       0.001 |
| &#39;MemoryStreamSlim Deflate Compression&#39;       | 131072    | False            |      9.590 ms |  0.0813 ms |  0.0760 ms |  0.94 |         - |         - |         - |       2.47 KB |       0.001 |
|                                              |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream Deflate Compression&#39;**           | **131072**    | **True**             |     **10.061 ms** |  **0.0383 ms** |  **0.0340 ms** |  **1.00** |  **609.3750** |  **609.3750** |  **609.3750** |    **1921.93 KB** |       **1.000** |
| &#39;RecyclableMemoryStream Deflate Compression&#39; | 131072    | True             |      9.585 ms |  0.0732 ms |  0.0685 ms |  0.95 |         - |         - |         - |       2.51 KB |       0.001 |
| &#39;MemoryStreamSlim Deflate Compression&#39;       | 131072    | True             |      9.590 ms |  0.0809 ms |  0.0756 ms |  0.95 |         - |         - |         - |       2.47 KB |       0.001 |
|                                              |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream Deflate Compression&#39;**           | **16777216**  | **False**            |  **1,319.166 ms** |  **7.5423 ms** |  **7.0551 ms** |  **1.00** | **3000.0000** | **3000.0000** | **3000.0000** |  **327645.48 KB** |       **1.000** |
| &#39;RecyclableMemoryStream Deflate Compression&#39; | 16777216  | False            |  1,274.450 ms |  3.9225 ms |  3.6691 ms |  0.97 |         - |         - |         - |     365.37 KB |       0.001 |
| &#39;MemoryStreamSlim Deflate Compression&#39;       | 16777216  | False            |  1,278.473 ms |  3.6578 ms |  3.2425 ms |  0.97 |         - |         - |         - |        2.9 KB |       0.000 |
|                                              |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream Deflate Compression&#39;**           | **16777216**  | **True**             |  **1,295.906 ms** |  **3.4763 ms** |  **3.2518 ms** |  **1.00** | **1000.0000** | **1000.0000** | **1000.0000** |  **245762.62 KB** |       **1.000** |
| &#39;RecyclableMemoryStream Deflate Compression&#39; | 16777216  | True             |  1,277.056 ms |  3.2037 ms |  2.8400 ms |  0.99 |         - |         - |         - |       28.3 KB |       0.000 |
| &#39;MemoryStreamSlim Deflate Compression&#39;       | 16777216  | True             |  1,278.826 ms |  4.2683 ms |  3.9926 ms |  0.99 |         - |         - |         - |        2.9 KB |       0.000 |
|                                              |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream Deflate Compression&#39;**           | **209715200** | **False**            | **16,143.340 ms** | **39.1144 ms** | **36.5876 ms** |  **1.00** | **3000.0000** | **3000.0000** | **3000.0000** | **2621405.55 KB** |       **1.000** |
| &#39;RecyclableMemoryStream Deflate Compression&#39; | 209715200 | False            | 15,904.731 ms | 41.4735 ms | 36.7652 ms |  0.99 | 2000.0000 |         - |         - |   50533.79 KB |       0.019 |
| &#39;MemoryStreamSlim Deflate Compression&#39;       | 209715200 | False            | 15,864.483 ms | 35.5313 ms | 33.2360 ms |  0.98 |         - |         - |         - |       3.18 KB |       0.000 |
|                                              |           |                  |               |            |            |       |           |           |           |               |             |
| **&#39;MemoryStream Deflate Compression&#39;**           | **209715200** | **True**             | **16,053.706 ms** | **25.8005 ms** | **22.8715 ms** |  **1.00** | **1000.0000** | **1000.0000** | **1000.0000** | **3072002.62 KB** |       **1.000** |
| &#39;RecyclableMemoryStream Deflate Compression&#39; | 209715200 | True             | 15,884.394 ms | 42.9453 ms | 40.1711 ms |  0.99 |         - |         - |         - |     315.52 KB |       0.000 |
| &#39;MemoryStreamSlim Deflate Compression&#39;       | 209715200 | True             | 15,849.313 ms | 43.2807 ms | 40.4848 ms |  0.99 |         - |         - |         - |        2.9 KB |       0.000 |
