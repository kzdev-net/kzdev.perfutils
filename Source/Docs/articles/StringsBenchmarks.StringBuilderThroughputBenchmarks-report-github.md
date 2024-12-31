```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4602/23H2/2023Update/SunValley3)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2


```
| Method                  | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0        | Gen1      | Allocated | Alloc Ratio |
|------------------------ |---------:|--------:|--------:|------:|--------:|------------:|----------:|----------:|------------:|
| &#39;Unique StringBuilders&#39; | 241.6 ms | 3.38 ms | 3.16 ms |  1.00 |    0.02 | 222000.0000 | 4333.3333 |   3.89 GB |        1.00 |
| &#39;Cached StringBuilders&#39; | 116.3 ms | 1.37 ms | 1.21 ms |  0.48 |    0.01 |  80400.0000 |         - |   1.41 GB |        0.36 |
