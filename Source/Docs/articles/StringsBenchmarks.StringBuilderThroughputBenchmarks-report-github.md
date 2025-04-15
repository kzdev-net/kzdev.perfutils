```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                  | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1     | Allocated | Alloc Ratio |
|------------------------ |---------:|---------:|---------:|------:|-----------:|---------:|----------:|------------:|
| &#39;Unique StringBuilders&#39; | 39.07 ms | 0.397 ms | 0.371 ms |  1.00 | 42846.1538 | 769.2308 | 769.55 MB |        1.00 |
| &#39;Cached StringBuilders&#39; | 19.93 ms | 0.285 ms | 0.253 ms |  0.51 | 15812.5000 |        - |  283.9 MB |        0.37 |
