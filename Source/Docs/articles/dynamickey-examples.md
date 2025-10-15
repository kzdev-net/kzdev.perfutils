# DynamicKey Examples

This document provides comprehensive examples demonstrating how to use DynamicKey in various scenarios. These examples complement the main [DynamicKey documentation](dynamickey.md) by showing practical usage patterns.

## Basic Single Key Creation

DynamicKey supports creating keys from all common .NET types:

```csharp
using KZDev.PerfUtils;

// Integer keys
DynamicKey intKey = DynamicKey.GetKey(42);

// String keys
DynamicKey stringKey = DynamicKey.GetKey("user-123");

// Guid keys
DynamicKey guidKey = DynamicKey.GetKey(Guid.NewGuid());

// Boolean keys
DynamicKey boolKey = DynamicKey.GetKey(true);

// Type keys
DynamicKey typeKey = DynamicKey.GetKey(typeof(string));

// Enum keys
DynamicKey enumKey = DynamicKey.GetEnumKey(ConsoleColor.Red);

// Object keys
DynamicKey objKey = DynamicKey.GetKey(new { Name = "Test", Value = 42 });

// Value type keys
DynamicKey valueKey = DynamicKey.GetValueKey(DateTime.Now.Date);
```

## Multi-Parameter Composite Keys (Recommended)

The most convenient approach for creating composite keys:

```csharp
// 2-parameter composite key
int userId = 12345;
string sessionId = "session-abc-123";
DynamicKey key2 = DynamicKey.GetKey(userId, sessionId);

// 3-parameter composite key
long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
DynamicKey key3 = DynamicKey.GetKey(userId, sessionId, timestamp);

// 5-parameter composite key
bool isAdmin = true;
string queryParams = "page=1&size=10&sort=name";
DynamicKey key5 = DynamicKey.GetKey(userId, sessionId, timestamp, isAdmin, queryParams);

// 12-parameter composite key (maximum supported)
uint tenantId = 999u;
ulong requestId = 9876543210UL;
string userAgent = "Mozilla/5.0";
string ipAddress = "192.168.1.100";
string feature = "advanced-search";
int pageSize = 25;
string sortOrder = "desc";

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

## All Four Composition Approaches

DynamicKey provides four different ways to create composite keys:

### 1. Multi-Parameter GetKey (Most Convenient)

```csharp
DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp, isAdmin);
```

### 2. Operator+ Overloading

```csharp
DynamicKey key = DynamicKey.GetKey(userId) +
                 DynamicKey.GetKey(sessionId) +
                 DynamicKey.GetKey(timestamp) +
                 DynamicKey.GetKey(isAdmin);
```

### 3. Combine Factory Method

```csharp
DynamicKey key = DynamicKey.Combine(
    DynamicKey.GetKey(userId),
    DynamicKey.GetKey(sessionId),
    DynamicKey.GetKey(timestamp),
    DynamicKey.GetKey(isAdmin)
);
```

### 4. Builder Pattern

```csharp
DynamicKey key = DynamicKeyBuilder.Create()
    .Add(userId)
    .Add(sessionId)
    .Add(timestamp)
    .Add(isAdmin)
    .Build();
```

## Caching Scenarios

DynamicKey excels in caching scenarios where you need composite keys:

### User Session Cache

```csharp
Dictionary<DynamicKey, string> cache = new ();

// Create session cache key
DynamicKey sessionKey = DynamicKey.GetKey(userId, sessionId);
cache[sessionKey] = "User session data: { userId: 12345, sessionId: 'session-abc-123', lastActivity: '2024-01-15T10:30:00Z' }";

// Retrieve from cache
if (cache.TryGetValue(sessionKey, out string sessionData))
{
    Console.WriteLine($"Retrieved: {sessionData}");
}
```

### API Response Cache

```csharp
// Create API cache key with multiple parameters
DynamicKey apiKey = DynamicKey.GetKey(
    "GET",                    // HTTP method
    "/api/users",             // endpoint
    userId,                   // user ID
    "page=1&size=10",         // query parameters
    "application/json"        // content type
);

cache[apiKey] = "API Response: { users: [...], totalCount: 150, page: 1 }";
```

### Database Query Cache

```csharp
// Create query cache key
DynamicKey queryKey = DynamicKey.GetKey(
    "SELECT * FROM Orders",   // SQL query
    userId,                   // user ID
    DateTime.Now.Date,        // date filter
    "status=completed"        // additional filters
);

cache[queryKey] = "Query Result: { orders: [...], count: 25 }";
```

### Feature Flag Cache

```csharp
// Create feature flag cache key
DynamicKey featureKey = DynamicKey.GetKey(
    "advanced-search",        // feature name
    userId,                   // user ID
    "premium",                // user tier
    "en-US"                   // locale
);

cache[featureKey] = "Feature Flag: { enabled: true, config: { maxResults: 1000 } }";
```

## Performance Comparison Example

Here's a practical example showing the performance difference between DynamicKey and string concatenation:

```csharp
// Performance comparison
Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

// DynamicKey approach
for (int i = 0; i < 1000; i++)
{
    DynamicKey perfKey = DynamicKey.GetKey(userId + i, sessionId + i, timestamp + i);
}
stopwatch.Stop();
long dynamicKeyTime = stopwatch.ElapsedMilliseconds;

// String concatenation approach
stopwatch.Restart();
for (int i = 0; i < 1000; i++)
{
    string perfString = $"{userId + i}|{sessionId + i}|{timestamp + i}";
}
stopwatch.Stop();
long stringTime = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"DynamicKey approach: {dynamicKeyTime}ms");
Console.WriteLine($"String concatenation: {stringTime}ms");
Console.WriteLine($"Performance ratio: {(double)stringTime / dynamicKeyTime:F2}x");
```

## Dictionary Usage Examples

### Basic Dictionary Operations

```csharp
Dictionary<DynamicKey, string> dictionary = new ();

// Add entries
dictionary[DynamicKey.GetKey(1, "user")] = "User 1 data";
dictionary[DynamicKey.GetKey(2, "admin")] = "Admin data";
dictionary[DynamicKey.GetKey(1, "guest")] = "Guest data";

// Retrieve entries
if (dictionary.TryGetValue(DynamicKey.GetKey(1, "user"), out string value))
{
    Console.WriteLine($"Found: {value}");
}

// Check if key exists
bool exists = dictionary.ContainsKey(DynamicKey.GetKey(2, "admin"));
```

### Cache Invalidation Patterns

```csharp
// Invalidate all entries for a specific user
List<DynamicKey> keysToRemove = new ();
foreach (var kvp in dictionary)
{
    // Check if key contains the user ID (simplified example)
    if (kvp.Key.ToString().Contains(userId.ToString()))
    {
        keysToRemove.Add(kvp.Key);
    }
}

foreach (DynamicKey key in keysToRemove)
{
    dictionary.Remove(key);
}
```

## Best Practices

### 1. Choose the Right Composition Approach

- **Multi-parameter GetKey**: For simple, known-at-compile-time compositions
- **Operator+**: For dynamic runtime composition
- **Combine**: For array-based key creation
- **Builder**: For complex, conditional key building

### 2. Optimize Key Order

```csharp
// Good: Most selective values first
DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp);

// Less optimal: Less selective values first
DynamicKey key = DynamicKey.GetKey(timestamp, sessionId, userId);
```

### 3. Use Appropriate Types

```csharp
// Good: Use enums instead of strings
DynamicKey key = DynamicKey.GetKey(userId, UserRole.Admin, PermissionLevel.Full);

// Less optimal: String constants
DynamicKey key = DynamicKey.GetKey(userId, "admin", "full");
```

### 4. Cache Key Reuse

```csharp
// Good: Reuse keys when possible
DynamicKey userKey = DynamicKey.GetKey(userId);
DynamicKey sessionKey = userKey + DynamicKey.GetKey(sessionId);
DynamicKey adminKey = userKey + DynamicKey.GetKey("admin");

// Less optimal: Creating new keys repeatedly
DynamicKey sessionKey = DynamicKey.GetKey(userId, sessionId);
DynamicKey adminKey = DynamicKey.GetKey(userId, "admin");
```

## Error Handling

```csharp
try
{
    // DynamicKey creation is generally safe, but handle edge cases
    DynamicKey key = DynamicKey.GetKey(userId, sessionId);

    if (key == null)
    {
        throw new InvalidOperationException("Failed to create DynamicKey");
    }
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Invalid parameters: {ex.Message}");
}
```

## Thread Safety

DynamicKey instances are thread-safe and can be used in multi-threaded scenarios:

```csharp
// Safe to use in parallel operations
Parallel.For(0, 1000, i =>
{
    DynamicKey key = DynamicKey.GetKey(i, $"thread-{Thread.CurrentThread.ManagedThreadId}");
    // Use key safely in parallel
});
```

## Integration with Existing Code

### Converting from String Keys

```csharp
// Before: String-based keys
string oldKey = $"{userId}|{sessionId}|{timestamp}";
cache[oldKey] = value;

// After: DynamicKey
DynamicKey newKey = DynamicKey.GetKey(userId, sessionId, timestamp);
cache[newKey] = value;
```

### Gradual Migration

```csharp
// Hybrid approach during migration
public class CacheService
{
    private readonly Dictionary<DynamicKey, string> _newCache = new();
    private readonly Dictionary<string, string> _oldCache = new();

    public string GetValue(int userId, string sessionId)
    {
        // Try new cache first
        DynamicKey newKey = DynamicKey.GetKey(userId, sessionId);
        if (_newCache.TryGetValue(newKey, out string newValue))
        {
            return newValue;
        }

        // Fall back to old cache
        string oldKey = $"{userId}|{sessionId}";
        if (_oldCache.TryGetValue(oldKey, out string oldValue))
        {
            // Migrate to new cache
            _newCache[newKey] = oldValue;
            return oldValue;
        }

        return null;
    }
}
```

These examples demonstrate the flexibility and power of DynamicKey for creating efficient composite keys in various scenarios. For more detailed information, see the [main DynamicKey documentation](dynamickey.md) and [performance benchmarks](dynamickey-benchmarks.md).
