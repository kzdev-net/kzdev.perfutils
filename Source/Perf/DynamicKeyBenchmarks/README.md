# DynamicKey Benchmarks

This project contains performance benchmarks for the DynamicKey feature, comparing different approaches to creating composite keys against string concatenation.

## Benchmarks

### CompositeKeyLookupBenchmarks

- **Multi-Parameter GetKey**: Compare 2, 3, 5, and 12 parameter composite keys vs string concatenation
- **Operator+ Approach**: Compare operator+ composition vs string concatenation
- **Combine Method**: Compare Combine factory method vs string concatenation
- **Builder Pattern**: Compare Builder pattern vs string concatenation
- **Dictionary Lookup Performance**: Performance of each approach in dictionary lookups

### SingleKeyBenchmarks

- **Single Primitive Keys**: Compare single primitive keys vs string conversion
- **Dictionary Sizes**: Test with 100, 1000, and 10000 entries

## Running Benchmarks

```bash
dotnet run --configuration Release
```

To run specific benchmarks:

```bash
dotnet run --configuration Release --filter "*CompositeKeyLookupBenchmarks*"
dotnet run --configuration Release --filter "*SingleKeyBenchmarks*"
```

## Results

Benchmark results are generated in the `BenchmarkDotNet.Artifacts` folder and include:

- HTML reports
- CSV data
- GitHub markdown reports
