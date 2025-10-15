# DynamicKey

DynamicKey provides a high-performance alternative to string-based keys for scenarios where keys are composed of multiple elements that may not be known at compile time. Instead of concatenating strings (which is expensive), DynamicKey creates efficient composite keys that can be compared, hashed, and used in dictionaries with superior performance.

## Problem Statement

When building applications, you often need to create composite keys from multiple values:

```csharp
// Traditional approach - expensive string concatenation
string cacheKey = $"{userId}|{sessionId}|{timestamp}|{isAdmin}";
string dictionaryKey = $"{httpMethod}|{endpoint}|{queryParams}";
```

This approach has several problems:

- **Performance**: String concatenation is expensive, especially in hot paths
- **Memory**: Creates temporary strings that need garbage collection
- **Comparison**: String comparison is slower than value type comparison
- **Hash Collisions**: String hashing can be less efficient than specialized hash codes

## DynamicKey Solution

DynamicKey solves these problems by providing:

- **Type-specific key classes** for optimal performance
- **Efficient composite key creation** from multiple values
- **Optimized hash codes** and comparison operations
- **Thread-safe caching** for frequently used keys
- **Multiple composition approaches** for different scenarios

## Supported Types

DynamicKey supports all common .NET types:

### Primitive Types

- `int`, `long`, `uint`, `ulong`
- `bool`
- `string`
- `Guid`

### Complex Types

- `Type` (for type-based keys)
- `object` (reference types)
- Value types (via `GetValueKey<T>`)
- Enums (via `GetEnumKey<TEnum>`)

## Single Key Usage

Creating single keys is straightforward:

```csharp
// Integer key
DynamicKey intKey = DynamicKey.GetKey(42);

// String key
DynamicKey stringKey = DynamicKey.GetKey("user-123");

// Guid key
DynamicKey guidKey = DynamicKey.GetKey(Guid.NewGuid());

// Boolean key
DynamicKey boolKey = DynamicKey.GetKey(true);

// Type key
DynamicKey typeKey = DynamicKey.GetKey(typeof(string));

// Enum key
DynamicKey enumKey = DynamicKey.GetEnumKey(ConsoleColor.Red);

// Object key
DynamicKey objKey = DynamicKey.GetKey(new { Name = "Test", Value = 42 });

// Value type key
DynamicKey valueKey = DynamicKey.GetValueKey(DateTime.Now.Date);
```

## Composite Key Creation

DynamicKey provides **four approaches** for creating composite keys:

### 1. Multi-Parameter Generic GetKey (Recommended)

The most convenient approach for creating composite keys:

```csharp
// 2-parameter composite key
DynamicKey key2 = DynamicKey.GetKey(userId, sessionId);

// 3-parameter composite key
DynamicKey key3 = DynamicKey.GetKey(userId, sessionId, timestamp);

// 5-parameter composite key
DynamicKey key5 = DynamicKey.GetKey(userId, sessionId, timestamp, isAdmin, queryParams);

// 12-parameter composite key (maximum supported)
DynamicKey key12 = DynamicKey.GetKey(
    userId, sessionId, timestamp, isAdmin, queryParams,
    tenantId, requestId, userAgent, ipAddress, feature, pageSize, sortOrder
);

// Mixed types
DynamicKey mixedKey = DynamicKey.GetKey(
    userId,                    // int
    sessionId,                 // string
    timestamp,                 // long
    isAdmin,                   // bool
    Guid.NewGuid(),            // Guid
    typeof(string),            // Type
    ConsoleColor.Blue,         // Enum
    DateTime.Now.Date,         // DateTime (value type)
    new { Name = "Test" }      // object
);
```

### 2. Operator+ Overloading

For dynamic composition at runtime:

```csharp
DynamicKey key = DynamicKey.GetKey(userId) +
          DynamicKey.GetKey(sessionId) +
          DynamicKey.GetKey(timestamp) +
          DynamicKey.GetKey(isAdmin);
```

### 3. Combine Factory Method

When you have an array of keys:

```csharp
DynamicKey key = DynamicKey.Combine(
    DynamicKey.GetKey(userId),
    DynamicKey.GetKey(sessionId),
    DynamicKey.GetKey(timestamp),
    DynamicKey.GetKey(isAdmin)
);
```

### 4. Builder Pattern

For complex, conditional key building:

```csharp
DynamicKey key = DynamicKeyBuilder.Create()
    .Add(userId)
    .Add(sessionId)
    .Add(timestamp)
    .Add(isAdmin)
    .Build();
```

## Performance Characteristics

DynamicKey provides significant performance improvements over string concatenation:

### Key Creation Performance

- **Much faster** than string concatenation
- **Reduced memory allocation** (no temporary strings)
- **Optimized hash codes** for better dictionary performance

### Dictionary Lookup Performance

- **Faster hash code generation** than strings
- **Efficient comparison operations** using value types
- **Better cache locality** due to reduced object creation

### Memory Usage

- **Lower memory footprint** than string keys
- **Reduced garbage collection pressure**
- **Thread-static caching** for frequently used keys

## When to Use DynamicKey

### Use DynamicKey When:

- Creating composite keys from multiple values
- Performance is critical (hot paths, high-frequency operations)
- Using keys in dictionaries or caches
- Need consistent, efficient hash codes
- Working with variable numbers of key components

### Use Strings When:

- Simple, single-value keys
- Human-readable keys are required
- Interoperating with external systems that expect strings
- Keys are rarely created or compared

## Caching Examples

DynamicKey excels in caching scenarios:

```csharp
// User session cache
DynamicKey sessionKey = DynamicKey.GetKey(userId, sessionId);
cache[sessionKey] = sessionData;

// API response cache
DynamicKey apiKey = DynamicKey.GetKey(httpMethod, endpoint, userId, queryParams, contentType);
cache[apiKey] = apiResponse;

// Database query cache
DynamicKey queryKey = DynamicKey.GetKey(sqlQuery, userId, dateFilter, additionalFilters);
cache[queryKey] = queryResult;

// Feature flag cache
DynamicKey featureKey = DynamicKey.GetKey(featureName, userId, userTier, locale);
cache[featureKey] = featureConfig;
```

## Best Practices

### 1. Choose the Right Composition Approach

- **Multi-parameter GetKey**: For simple, known-at-compile-time compositions
- **Operator+**: For dynamic runtime composition
- **Combine**: For array-based key creation
- **Builder**: For complex, conditional key building

### 2. Optimize Key Order

- Place **most selective** values first
- Group **related values** together
- Consider **cache invalidation patterns**

### 3. Use Appropriate Types

- Prefer **primitive types** when possible
- Use **enums** instead of string constants
- Consider **value types** for small objects

### 4. Cache Key Reuse

- **Reuse keys** when possible (DynamicKey provides thread-static caching)
- **Avoid creating keys** in tight loops
- **Consider key pooling** for high-frequency scenarios

## Examples

See the [DynamicKey Examples](dynamickey-examples.md) for comprehensive usage examples including:

- Basic single key creation
- Multi-parameter composite keys
- All four composition approaches
- Caching scenarios
- Performance comparisons

## Benchmarks

See the [DynamicKey Benchmarks](dynamickey-benchmarks.md) for detailed performance comparisons:

- Key creation performance
- Dictionary lookup performance
- Memory usage analysis
- Comparison with string-based approaches

## API Reference

For complete API documentation, see the [DynamicKey API Reference](../api/KZDev.PerfUtils.DynamicKey.yml).
