// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using KZDev.PerfUtils;

namespace KZDev.PerfUtils.Examples.DynamicKeys;

/// <summary>
/// Demonstrates using DynamicKey instances as cache keys for efficient caching scenarios.
/// Shows how composite keys can be used to create unique cache entries based on multiple parameters.
/// </summary>
public static class CachingExample
{
  /// <summary>
  /// Runs the caching examples demonstrating DynamicKey usage in cache scenarios.
  /// </summary>
  public static void Run ()
  {
    Console.WriteLine(@"=== DynamicKey Caching Examples ===");

    // Simulate a cache using Dictionary<DynamicKey, string>
    Dictionary<DynamicKey, string> cache = new Dictionary<DynamicKey, string>();

    // Example 1: User session cache
    Console.WriteLine(@"1. User Session Cache:");
    int userId = 12345;
    string sessionId = "session-abc-123";
    DynamicKey userSessionKey = DynamicKey.GetKey(userId, sessionId);

    // Cache user session data
    cache[userSessionKey] = "User session data: { userId: 12345, sessionId: 'session-abc-123', lastActivity: '2024-01-15T10:30:00Z' }";

    // Retrieve from cache
    if (cache.TryGetValue(userSessionKey, out string? sessionData))
    {
      Console.WriteLine($@"   Retrieved: {sessionData}");
    }
    Console.WriteLine($@"   Cache Key: {userSessionKey}");
    Console.WriteLine($"   Hash Code: {userSessionKey.GetHashCode()}");

    // Example 2: API response cache with multiple parameters
    Console.WriteLine(@"2. API Response Cache:");
    DynamicKey apiKey = DynamicKey.GetKey(
        "GET",                    // HTTP method
        "/api/users",             // endpoint
        userId,                   // user ID
        "page=1&size=10",         // query parameters
        "application/json"        // content type
    );

    cache[apiKey] = "API Response: { users: [...], totalCount: 150, page: 1 }";

    if (cache.TryGetValue(apiKey, out string? apiData))
    {
      Console.WriteLine($@"   Retrieved: {apiData}");
    }
    Console.WriteLine($@"   Cache Key: {apiKey}");
    Console.WriteLine($@"   Hash Code: {apiKey.GetHashCode()}");

    // Example 3: Database query cache
    Console.WriteLine(@"3. Database Query Cache:");
    DynamicKey queryKey = DynamicKey.GetKey(
        "SELECT * FROM Orders",   // SQL query
        userId,                   // user ID
        DateTime.Now.Date,        // date filter
        "status=completed"        // additional filters
    );

    cache[queryKey] = "Query Result: { orders: [...], count: 25 }";

    if (cache.TryGetValue(queryKey, out string? queryData))
    {
      Console.WriteLine($@"   Retrieved: {queryData}");
    }
    Console.WriteLine($@"   Cache Key: {queryKey}");
    Console.WriteLine($@"   Hash Code: {queryKey.GetHashCode()}");

    // Example 4: Feature flag cache
    Console.WriteLine(@"4. Feature Flag Cache:");
    DynamicKey featureKey = DynamicKey.GetKey(
        "advanced-search",        // feature name
        userId,                   // user ID
        "premium",                // user tier
        "en-US"                   // locale
    );

    cache[featureKey] = "Feature Flag: { enabled: true, config: { maxResults: 1000 } }";

    if (cache.TryGetValue(featureKey, out string? featureData))
    {
      Console.WriteLine($@"   Retrieved: {featureData}");
    }
    Console.WriteLine($@"   Cache Key: {featureKey}");
    Console.WriteLine($@"   Hash Code: {featureKey.GetHashCode()}");

    // Example 5: Performance comparison with string keys
    Console.WriteLine(@"5. Performance Comparison (String vs DynamicKey):");

    Dictionary<string, string> stringCache = new Dictionary<string, string>();
    Dictionary<DynamicKey, string> dynamicKeyCache = new Dictionary<DynamicKey, string>();

    // Populate caches
    for (int i = 0; i < 1000; i++)
    {
      int testUserId = userId + i;
      string testSessionId = sessionId + i;

      // String cache
      string stringKey = $"{testUserId}|{testSessionId}";
      stringCache[stringKey] = $"Data for user {testUserId}, session {testSessionId}";

      // DynamicKey cache
      DynamicKey dynamicKey = DynamicKey.GetKey(testUserId, testSessionId);
      dynamicKeyCache[dynamicKey] = $"Data for user {testUserId}, session {testSessionId}";
    }

    // Test retrieval performance
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < 1000; i++)
    {
      int testUserId = userId + i;
      string testSessionId = sessionId + i;
      string stringKey = $"{testUserId}|{testSessionId}";
      stringCache.TryGetValue(stringKey, out _);
    }
    stopwatch.Stop();
    long stringTime = stopwatch.ElapsedMilliseconds;

    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
      int testUserId = userId + i;
      string testSessionId = sessionId + i;
      DynamicKey dynamicKey = DynamicKey.GetKey(testUserId, testSessionId);
      dynamicKeyCache.TryGetValue(dynamicKey, out _);
    }
    stopwatch.Stop();
    long dynamicKeyTime = stopwatch.ElapsedMilliseconds;

    Console.WriteLine($"   String key retrieval (1000 lookups): {stringTime}ms");
    Console.WriteLine($"   DynamicKey retrieval (1000 lookups): {dynamicKeyTime}ms");
    Console.WriteLine($"   Performance ratio: {(double)stringTime / dynamicKeyTime:F2}x");

    // Example 6: Cache invalidation patterns
    Console.WriteLine(@"6. Cache Invalidation Patterns:");

    // Invalidate by user ID (partial key matching)
    int userInvalidationCount = 0;
    List<DynamicKey> keysToRemove = new List<DynamicKey>();

    foreach (KeyValuePair<DynamicKey, string> kvp in cache)
    {
      // Check if key contains the user ID (simplified example)
      if (kvp.Key.ToString().Contains(userId.ToString()))
      {
        keysToRemove.Add(kvp.Key);
        userInvalidationCount++;
      }
    }

    foreach (DynamicKey key in keysToRemove)
    {
      cache.Remove(key);
    }

    Console.WriteLine($"   Invalidated {userInvalidationCount} cache entries for user {userId}");
    Console.WriteLine($"   Remaining cache entries: {cache.Count}");

    // Example 7: Cache statistics
    Console.WriteLine(@"7. Cache Statistics:");
    Console.WriteLine($"   Total cache entries: {cache.Count}");
    Console.WriteLine($"   Cache hit rate: {cache.Count / (double)(cache.Count + 0) * 100:F1}%");
    Console.WriteLine($"   Average key complexity: {cache.Keys.Average(k => k.ToString().Length):F1} characters");

    Console.WriteLine(@"=== Caching Examples Complete ===");
  }
}
