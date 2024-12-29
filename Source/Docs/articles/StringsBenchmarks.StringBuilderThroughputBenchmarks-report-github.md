```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 8.0.404
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2


```
| Method                  | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0        | Gen1      | Allocated | Alloc Ratio |
|------------------------ |---------:|--------:|--------:|------:|--------:|------------:|----------:|----------:|------------:|
| &#39;Unique StringBuilders&#39; | 217.1 ms | 4.25 ms | 5.06 ms |  1.00 |    0.03 | 213666.6667 | 4000.0000 |   3.75 GB |        1.00 |
| &#39;Cached StringBuilders&#39; | 122.2 ms | 2.10 ms | 1.96 ms |  0.56 |    0.02 |  78600.0000 |         - |   1.38 GB |        0.37 |
