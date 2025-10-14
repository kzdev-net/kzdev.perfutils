# DynamicKey Feature Implementation Plan

## Overview

Implement a complete DynamicKey feature that provides high-performance composite key functionality as an alternative to string concatenation. The feature will support single and composite keys with multiple approaches for composition, comprehensive testing, benchmarking, and documentation.

## 1. Core Implementation

### 1.1 Fix Generic Type Handling

Fix the compiler errors in `DynamicKey<T>.GetKey()` method in `Source/Src/KZDev.PerfUtils/DynamicKey/DynamicKey\`.cs`:

**Problem**: Lines 52-56 try to call `DynamicValKey<T>` or `DynamicRefKey<T>` but `T` is unconstrained, causing compiler errors.

**Solution**: Replace the problematic default case with a check-and-cast approach:

- For value types: Box the value, then use pattern matching to route to specific key types or create `DynamicObjectKey` as fallback
- For reference types: Keep existing `DynamicRefKey<T>` path but only when `T` is constrained
- Alternatively, create new internal helper methods `GetValueTypeKey<TVal>(TVal value) where TVal : struct` and call via runtime check

**Specific approach**:

1. Change default case to just return `DynamicObjectKey.GetKey(value)` which handles all types generically
2. This removes the need for runtime type checking while maintaining correctness
3. For value types where performance matters (int, long, bool, Guid), they're already handled in the switch cases above

### 1.2 Add New Primitive Type Keys

Create specialized key types for additional primitive types in `Source/Src/KZDev.PerfUtils/DynamicKey/`:

- **DynamicLongKey.cs** - 64-bit integer keys (similar to DynamicIntKey.cs pattern)
- **DynamicUIntKey.cs** - unsigned 32-bit integer keys
- **DynamicULongKey.cs** - unsigned 64-bit integer keys
- **DynamicEnumKey.cs** - generic enum key type `DynamicEnumKey<TEnum> where TEnum : struct, Enum`

Each should follow the existing pattern: thread-static caching, IComparable implementation, GetHashCode optimization.

Add corresponding cases to `DynamicKey<T>.GetKey()` switch statement and `DynamicValKey<T>.GetKey()` switch statement for the new types.

### 1.2 Composite Key Implementation

Create `DynamicCompositeKey.cs` to support multi-element keys:

- Internal class that holds an immutable array of DynamicKey elements
- GetHashCode combines hash codes using HashCode.Combine for up to 5 elements
- Equals performs sequential comparison of all elements
- CompareTo compares elements in order until difference found
- Thread-static caching for common composite patterns (2-3 element composites)

### 1.3 Key Composition Methods

Add to `DynamicKey.cs`:

- **Operator overloading**: `operator +` to combine keys (e.g., `key1 + key2 + key3`)
- **Factory method**: `static DynamicKey Combine(params DynamicKey[] keys)`
- **Builder pattern**: `DynamicKeyBuilder` class with fluent API:
- `DynamicKeyBuilder Create()`
- `Add(DynamicKey key)` / `Add<T>(T value)`
- `Build()` returns composite DynamicKey

### 1.4 Update DynamicKey Base Class

Update `Source/Src/KZDev.PerfUtils/DynamicKey/DynamicKey.cs`:

- Add GetKey overloads for long, uint, ulong
- Add GetKey<TEnum> for enum types
- Add implicit conversion operators for new types
- Update XML documentation for all new methods

## 2. Comprehensive Unit Tests

Create `Source/Tst/KZDev.PerfUtils.DynamicKey.UnitTests/` project:

- **Project setup**: xUnit test project with FluentAssertions
- **UsingDynamicKey_SingleKeys.cs**: Test all single key types (int, long, uint, ulong, bool, string, Guid, Type, enums, objects)
- **UsingDynamicKey_CompositeKeys.cs**: Test composite keys with 2-5 elements, various type combinations
- **UsingDynamicKey_Equality.cs**: Test Equals, GetHashCode consistency, operator== and !=
- **UsingDynamicKey_Comparison.cs**: Test CompareTo, IComparable behavior, cross-type comparisons
- **UsingDynamicKey_EdgeCases.cs**: Null values, empty strings, default GUIDs, large numbers, mixed nulls
- **UsingDynamicKey_Caching.cs**: Verify thread-static caching behavior, cache hits
- **UsingDynamicKey_Builders.cs**: Test all composition approaches (operator+, Combine, Builder)
- **UsingDynamicKey_DictionaryUsage.cs**: Test as Dictionary/HashSet keys

## 3. Benchmarks

Create `Source/Perf/DynamicKeyBenchmarks/` project:

- **Project setup**: BenchmarkDotNet console app (copy pattern from MemoryStreamBenchmarks)
- **Program.cs**: Standard BenchmarkSwitcher setup
- **ThroughputBenchmarks/CompositeKeyLookupBenchmarks.cs**:
- Compare 2-element composite vs concatenated string as Dictionary key
- Compare 3-element composite vs concatenated string
- Compare 5-element composite vs concatenated string
- Mix of types (int+string, Guid+int+bool, etc.)
- Both creation and lookup operations
- **ThroughputBenchmarks/SingleKeyBenchmarks.cs**:
- Single primitive key vs string conversion for Dictionary lookups
- Benchmark parameters: dictionary sizes (100, 1000, 10000 entries), lookup counts

## 4. Example Code

Add to `Source/Dev/KZDev.PerfUtils.Examples/`:

- Create `DynamicKey/` subfolder
- **BasicUsageExample.cs**: Simple single key creation and usage
- **CompositeKeyExample.cs**: Show all three composition approaches (operator+, Combine, Builder)
- **CachingExample.cs**: Demonstrate using composite keys as cache keys in Dictionary

## 5. Documentation

### 5.1 Concept Documentation

Create `Source/Docs/articles/dynamickey.md`:

- Explain the problem: string concatenation performance cost
- Introduce DynamicKey concept and benefits
- Show supported types
- Explain single vs composite keys
- Usage examples for all composition approaches
- When to use DynamicKey vs strings
- Performance characteristics

### 5.2 Benchmark Documentation

Create benchmark report articles (auto-generated from benchmarks):

- `Source/Docs/articles/dynamickey-benchmarks.md` - overview
- `Source/Docs/articles/DynamicKeyBenchmarks.CompositeKeyLookupBenchmarks-report-github.md`
- `Source/Docs/articles/DynamicKeyBenchmarks.CompositeKeyLookupBenchmarks-report.html`
- `Source/Docs/articles/DynamicKeyBenchmarks.SingleKeyBenchmarks-report-github.md`
- `Source/Docs/articles/DynamicKeyBenchmarks.SingleKeyBenchmarks-report.html`

### 5.3 Update Getting Started

Update `Source/Docs/articles/getting-started.md`:

- Add DynamicKey section similar to MemoryStreamSlim/StringBuilderCache sections
- Link to main DynamicKey article and benchmark articles

### 5.4 Update TOC

Update `Source/Docs/articles/toc.yml` to include DynamicKey documentation

## 6. Integration & Validation

- Run all unit tests across all target frameworks (net6.0, net8.0, net9.0)
- Execute benchmarks and capture results
- Generate benchmark reports for documentation
- Build DocFx documentation and verify DynamicKey API appears correctly
- Verify no linter errors
- Update solution file if needed for new test/benchmark projects

## Key Files to Modify

Core implementation:

- `Source/Src/KZDev.PerfUtils/DynamicKey/DynamicKey.cs` - add new GetKey methods, operators, builder
- `Source/Src/KZDev.PerfUtils/DynamicKey/*.cs` - new key type classes

Testing:

- New test project under `Source/Tst/`

Benchmarks:

- New benchmark project under `Source/Perf/`

Documentation:

- `Source/Docs/articles/dynamickey.md` - new
- `Source/Docs/articles/getting-started.md` - update
- `Source/Docs/articles/toc.yml` - update

Examples:

- `Source/Dev/KZDev.PerfUtils.Examples/DynamicKey/*.cs` - new examples
