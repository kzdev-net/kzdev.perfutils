```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                               | DataSize  | CapacityOnCreate | BulkInitialFill | Mean         | Error      | StdDev     | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated     | Alloc Ratio |
|------------------------------------- |---------- |----------------- |---------------- |-------------:|-----------:|-----------:|------:|--------:|----------:|----------:|----------:|--------------:|------------:|
| **&#39;MemoryStream CopyToAsync&#39;**           | **131072**    | **False**            | **False**           |     **4.897 ms** |  **0.0404 ms** |  **0.0378 ms** |  **1.00** |    **0.01** |  **203.1250** |  **203.1250** |  **203.1250** |       **1262 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 131072    | False            | False           |     4.898 ms |  0.0343 ms |  0.0321 ms |  1.00 |    0.01 |         - |         - |         - |       2.23 KB |       0.002 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 131072    | False            | False           |     4.864 ms |  0.0211 ms |  0.0198 ms |  0.99 |    0.01 |         - |         - |         - |       2.43 KB |       0.002 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **131072**    | **False**            | **True**            |     **4.886 ms** |  **0.0432 ms** |  **0.0405 ms** |  **1.00** |    **0.01** |  **203.1250** |  **203.1250** |  **203.1250** |     **641.42 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 131072    | False            | True            |     4.912 ms |  0.0328 ms |  0.0306 ms |  1.01 |    0.01 |         - |         - |         - |       2.23 KB |       0.003 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 131072    | False            | True            |     4.889 ms |  0.0384 ms |  0.0359 ms |  1.00 |    0.01 |         - |         - |         - |       2.43 KB |       0.004 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **131072**    | **True**             | **False**           |     **4.887 ms** |  **0.0244 ms** |  **0.0228 ms** |  **1.00** |    **0.01** |  **203.1250** |  **203.1250** |  **203.1250** |     **641.42 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 131072    | True             | False           |     4.914 ms |  0.0340 ms |  0.0318 ms |  1.01 |    0.01 |         - |         - |         - |       2.23 KB |       0.003 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 131072    | True             | False           |     4.901 ms |  0.0413 ms |  0.0386 ms |  1.00 |    0.01 |         - |         - |         - |       2.43 KB |       0.004 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **131072**    | **True**             | **True**            |     **4.903 ms** |  **0.0395 ms** |  **0.0369 ms** |  **1.00** |    **0.01** |  **203.1250** |  **203.1250** |  **203.1250** |     **641.41 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 131072    | True             | True            |     4.902 ms |  0.0403 ms |  0.0377 ms |  1.00 |    0.01 |         - |         - |         - |       2.22 KB |       0.003 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 131072    | True             | True            |     4.894 ms |  0.0279 ms |  0.0261 ms |  1.00 |    0.01 |         - |         - |         - |       2.42 KB |       0.004 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **16777216**  | **False**            | **False**           |    **81.477 ms** |  **1.5778 ms** |  **1.6203 ms** |  **1.00** |    **0.03** | **3500.0000** | **3500.0000** | **3500.0000** |  **163827.26 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 16777216  | False            | False           |   624.013 ms |  3.1840 ms |  2.9783 ms |  7.66 |    0.15 |         - |         - |         - |     382.54 KB |       0.002 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 16777216  | False            | False           |   103.947 ms |  1.7337 ms |  1.6217 ms |  1.28 |    0.03 |         - |         - |         - |       7.03 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **16777216**  | **False**            | **True**            |    **79.419 ms** |  **1.3835 ms** |  **1.2942 ms** |  **1.00** |    **0.02** |  **857.1429** |  **857.1429** |  **857.1429** |    **81924.4 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 16777216  | False            | True            |   624.408 ms |  4.5190 ms |  4.2271 ms |  7.86 |    0.14 |         - |         - |         - |      45.02 KB |       0.001 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 16777216  | False            | True            |   125.978 ms |  0.8135 ms |  0.7609 ms |  1.59 |    0.03 |         - |         - |         - |       7.56 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **16777216**  | **True**             | **False**           |    **78.796 ms** |  **0.9133 ms** |  **0.8543 ms** |  **1.00** |    **0.01** |  **857.1429** |  **857.1429** |  **857.1429** |    **81924.4 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 16777216  | True             | False           |   624.585 ms |  5.7377 ms |  5.3670 ms |  7.93 |    0.11 |         - |         - |         - |       45.2 KB |       0.001 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 16777216  | True             | False           |   125.744 ms |  0.9553 ms |  0.8936 ms |  1.60 |    0.02 |         - |         - |         - |       7.63 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **16777216**  | **True**             | **True**            |    **79.449 ms** |  **1.2619 ms** |  **1.1804 ms** |  **1.00** |    **0.02** |  **857.1429** |  **857.1429** |  **857.1429** |    **81924.4 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 16777216  | True             | True            |   628.713 ms |  3.8275 ms |  3.5802 ms |  7.92 |    0.12 |         - |         - |         - |      45.44 KB |       0.001 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 16777216  | True             | True            |   126.012 ms |  0.8654 ms |  0.8095 ms |  1.59 |    0.03 |         - |         - |         - |       7.63 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **209715200** | **False**            | **False**           |   **895.971 ms** | **12.9062 ms** | **12.0725 ms** |  **1.00** |    **0.02** | **2000.0000** | **2000.0000** | **2000.0000** | **2621427.52 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 209715200 | False            | False           | 7,920.621 ms | 20.7709 ms | 19.4291 ms |  8.84 |    0.12 | 2000.0000 |         - |         - |   50742.32 KB |       0.019 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 209715200 | False            | False           | 1,388.802 ms | 11.4180 ms | 10.6804 ms |  1.55 |    0.02 |         - |         - |         - |      40.12 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **209715200** | **False**            | **True**            |   **717.077 ms** |  **6.3184 ms** |  **5.9102 ms** |  **1.00** |    **0.01** |         **-** |         **-** |         **-** | **1024004.46 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 209715200 | False            | True            | 7,915.937 ms | 31.6060 ms | 29.5642 ms | 11.04 |    0.10 |         - |         - |         - |      523.6 KB |       0.001 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 209715200 | False            | True            | 1,500.233 ms |  9.7613 ms |  9.1307 ms |  2.09 |    0.02 |         - |         - |         - |      43.52 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **209715200** | **True**             | **False**           |   **729.782 ms** |  **5.4017 ms** |  **5.0528 ms** |  **1.00** |    **0.01** |         **-** |         **-** |         **-** | **1024004.22 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 209715200 | True             | False           | 7,918.119 ms | 26.7204 ms | 24.9943 ms | 10.85 |    0.08 |         - |         - |         - |     523.16 KB |       0.001 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 209715200 | True             | False           | 1,502.928 ms | 10.3252 ms |  9.6582 ms |  2.06 |    0.02 |         - |         - |         - |      43.84 KB |       0.000 |
|                                      |           |                  |                 |              |            |            |       |         |           |           |           |               |             |
| **&#39;MemoryStream CopyToAsync&#39;**           | **209715200** | **True**             | **True**            |   **722.332 ms** |  **6.1485 ms** |  **5.7513 ms** |  **1.00** |    **0.01** |         **-** |         **-** |         **-** | **1024004.46 KB** |       **1.000** |
| &#39;RecyclableMemoryStream CopyToAsync&#39; | 209715200 | True             | True            | 7,923.251 ms | 21.6769 ms | 20.2766 ms | 10.97 |    0.09 |         - |         - |         - |     523.08 KB |       0.001 |
| &#39;MemoryStreamSlim CopyToAsync&#39;       | 209715200 | True             | True            | 1,504.902 ms |  8.7599 ms |  8.1940 ms |  2.08 |    0.02 |         - |         - |         - |       43.8 KB |       0.000 |
