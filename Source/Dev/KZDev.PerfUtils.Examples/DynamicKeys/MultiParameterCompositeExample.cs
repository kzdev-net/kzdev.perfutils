// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using KZDev.PerfUtils;

namespace KZDev.PerfUtils.Examples.DynamicKeys;

/// <summary>
/// Demonstrates the multi-parameter generic GetKey approach for creating composite keys.
/// This is the most convenient and recommended approach for creating composite keys.
/// </summary>
public static class MultiParameterCompositeExample
{
  /// <summary>
  /// Runs the multi-parameter composite key examples.
  /// </summary>
  public static void Run ()
  {
    Console.WriteLine(@"=== Multi-Parameter Composite Key Examples ===");

    // 2-parameter composite key
    Console.WriteLine(@"1. Two-Parameter Composite Key:");
    int userId = 12345;
    string sessionId = "session-abc-123";
    DynamicKey key2 = DynamicKey.GetKey(userId, sessionId);
    Console.WriteLine($@"   DynamicKey.GetKey({userId}, ""{sessionId}"") = {key2}");
    Console.WriteLine($@"   Hash Code: {key2.GetHashCode()}");

    // 3-parameter composite key
    Console.WriteLine(@"2. Three-Parameter Composite Key:");
    long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    DynamicKey key3 = DynamicKey.GetKey(userId, sessionId, timestamp);
    Console.WriteLine($@"   DynamicKey.GetKey({userId}, ""{sessionId}"", {timestamp}) = {key3}");
    Console.WriteLine($@"   Hash Code: {key3.GetHashCode()}");

    // 5-parameter composite key
    Console.WriteLine(@"3. Five-Parameter Composite Key:");
    bool isAdmin = true;
    string queryParams = "page=1&size=10&sort=name";
    DynamicKey key5 = DynamicKey.GetKey(userId, sessionId, timestamp, isAdmin, queryParams);
    Console.WriteLine($@"   DynamicKey.GetKey({userId}, ""{sessionId}"", {timestamp}, {isAdmin}, ""{queryParams}"") = {key5}");
    Console.WriteLine($@"   Hash Code: {key5.GetHashCode()}");

    // 12-parameter composite key (maximum supported)
    Console.WriteLine(@"4. Twelve-Parameter Composite Key (Maximum):");
    uint tenantId = 999u;
    ulong requestId = 9876543210UL;
    string userAgent = "Mozilla/5.0";
    string ipAddress = "192.168.1.100";
    string feature = "advanced-search";
    int pageSize = 25;
    string sortOrder = "desc";

    DynamicKey key12 = DynamicKey.GetKey(
        userId, sessionId, timestamp, isAdmin, queryParams,
        tenantId, requestId, userAgent, ipAddress, feature, pageSize, sortOrder);

    Console.WriteLine($@"   DynamicKey.GetKey(12 parameters) = {key12}");
    Console.WriteLine($@"   Hash Code: {key12.GetHashCode()}");

    // Mixed type parameters
    Console.WriteLine(@"5. Mixed Type Parameters:");
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
    Console.WriteLine($@"   DynamicKey.GetKey(9 mixed types) = {mixedKey}");
    Console.WriteLine($@"   Hash Code: {mixedKey.GetHashCode()}");

    // Performance comparison with string concatenation
    Console.WriteLine(@"6. Performance Comparison:");
    Console.WriteLine(@"   Creating 1000 composite keys...");

    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < 1000; i++)
    {
      DynamicKey perfKey = DynamicKey.GetKey(userId + i, sessionId + i, timestamp + i);
    }
    stopwatch.Stop();
    Console.WriteLine($@"   DynamicKey approach: {stopwatch.ElapsedMilliseconds}ms");

    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
      string perfString = $"{userId + i}|{sessionId + i}|{timestamp + i}";
    }
    stopwatch.Stop();
    Console.WriteLine($@"   String concatenation: {stopwatch.ElapsedMilliseconds}ms");

    Console.WriteLine(@"=== Multi-Parameter Composite Key Examples Complete ===");
  }
}
