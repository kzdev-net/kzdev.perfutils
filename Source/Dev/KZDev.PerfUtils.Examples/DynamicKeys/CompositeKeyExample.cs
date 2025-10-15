// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using KZDev.PerfUtils;

namespace KZDev.PerfUtils.Examples.DynamicKeys;

/// <summary>
/// Demonstrates all four approaches for creating composite DynamicKey instances:
/// 1. Multi-parameter generic GetKey (most convenient)
/// 2. Operator+ overloading
/// 3. Combine factory method
/// 4. Builder pattern
/// </summary>
public static class CompositeKeyExample
{
  /// <summary>
  /// Runs the composite key examples showing all four composition approaches.
  /// </summary>
  public static void Run ()
  {
    Console.WriteLine(@"=== Composite Key Examples - All Four Approaches ===");

    // Test data
    int userId = 12345;
    string sessionId = "session-abc-123";
    long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    bool isAdmin = true;

    // Approach 1: Multi-parameter generic GetKey (RECOMMENDED)
    Console.WriteLine(@"1. Multi-Parameter Generic GetKey (RECOMMENDED):");
    DynamicKey key1 = DynamicKey.GetKey(userId, sessionId, timestamp, isAdmin);
    Console.WriteLine($"   DynamicKey.GetKey({userId}, \"{sessionId}\", {timestamp}, {isAdmin})");
    Console.WriteLine($@"   Result: {key1}");
    Console.WriteLine($"   Hash Code: {key1.GetHashCode()}");

    // Approach 2: Operator+ overloading
    Console.WriteLine(@"2. Operator+ Overloading:");
    DynamicKey key2 = DynamicKey.GetKey(userId) + DynamicKey.GetKey(sessionId) +
                      DynamicKey.GetKey(timestamp) + DynamicKey.GetKey(isAdmin);
    Console.WriteLine($"   DynamicKey.GetKey({userId}) + DynamicKey.GetKey(\"{sessionId}\") + DynamicKey.GetKey({timestamp}) + DynamicKey.GetKey({isAdmin})");
    Console.WriteLine($@"   Result: {key2}");
    Console.WriteLine($"   Hash Code: {key2.GetHashCode()}");

    // Approach 3: Combine factory method
    Console.WriteLine(@"3. Combine Factory Method:");
    DynamicKey key3 = DynamicKey.Combine(
        DynamicKey.GetKey(userId),
        DynamicKey.GetKey(sessionId),
        DynamicKey.GetKey(timestamp),
        DynamicKey.GetKey(isAdmin)
    );
    Console.WriteLine($"   DynamicKey.Combine(DynamicKey.GetKey({userId}), DynamicKey.GetKey(\"{sessionId}\"), DynamicKey.GetKey({timestamp}), DynamicKey.GetKey({isAdmin}))");
    Console.WriteLine($@"   Result: {key3}");
    Console.WriteLine($"   Hash Code: {key3.GetHashCode()}");

    // Approach 4: Builder pattern
    Console.WriteLine(@"4. Builder Pattern:");
    DynamicKey key4 = DynamicKeyBuilder.Create()
        .Add(userId)
        .Add(sessionId)
        .Add(timestamp)
        .Add(isAdmin)
        .Build();
    Console.WriteLine($"   DynamicKeyBuilder.Create().Add({userId}).Add(\"{sessionId}\").Add({timestamp}).Add({isAdmin}).Build()");
    Console.WriteLine($@"   Result: {key4}");
    Console.WriteLine($"   Hash Code: {key4.GetHashCode()}");

    // Verify all approaches produce equivalent keys
    Console.WriteLine(@"5. Equivalence Check:");
    Console.WriteLine($"   All keys are equal: {key1.Equals(key2) && key2.Equals(key3) && key3.Equals(key4)}");
    Console.WriteLine($"   All hash codes are equal: {key1.GetHashCode() == key2.GetHashCode() && key2.GetHashCode() == key3.GetHashCode() && key3.GetHashCode() == key4.GetHashCode()}");

    // Performance comparison
    Console.WriteLine(@"6. Performance Comparison (1000 iterations):");

    // Multi-parameter approach
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < 1000; i++)
    {
      DynamicKey perfKey1 = DynamicKey.GetKey(userId + i, sessionId + i, timestamp + i, isAdmin);
    }
    stopwatch.Stop();
    long time1 = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"   Multi-parameter GetKey: {time1}ms");

    // Operator+ approach
    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
      DynamicKey perfKey2 = DynamicKey.GetKey(userId + i) + DynamicKey.GetKey(sessionId + i) +
                            DynamicKey.GetKey(timestamp + i) + DynamicKey.GetKey(isAdmin);
    }
    stopwatch.Stop();
    long time2 = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"   Operator+ approach: {time2}ms");

    // Combine approach
    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
      DynamicKey perfKey3 = DynamicKey.Combine(
          DynamicKey.GetKey(userId + i),
          DynamicKey.GetKey(sessionId + i),
          DynamicKey.GetKey(timestamp + i),
          DynamicKey.GetKey(isAdmin)
      );
    }
    stopwatch.Stop();
    long time3 = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"   Combine approach: {time3}ms");

    // Builder approach
    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
      DynamicKey perfKey4 = DynamicKeyBuilder.Create()
          .Add(userId + i)
          .Add(sessionId + i)
          .Add(timestamp + i)
          .Add(isAdmin)
          .Build();
    }
    stopwatch.Stop();
    long time4 = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"   Builder approach: {time4}ms");

    // String concatenation comparison
    stopwatch.Restart();
    for (int i = 0; i < 1000; i++)
    {
      string perfString = $"{userId + i}|{sessionId + i}|{timestamp + i}|{isAdmin}";
    }
    stopwatch.Stop();
    long timeString = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"   String concatenation: {timeString}ms");

    // Recommendations
    Console.WriteLine(@"7. Recommendations:");
    Console.WriteLine(@"   • Use multi-parameter GetKey for simple cases (most convenient)");
    Console.WriteLine(@"   • Use operator+ for dynamic composition at runtime");
    Console.WriteLine(@"   • Use Combine for when you have an array of keys");
    Console.WriteLine(@"   • Use Builder for complex, conditional key building");
    Console.WriteLine(@"   • All approaches are equivalent in terms of performance and functionality");

    Console.WriteLine(@"=== Composite Key Examples Complete ===");
  }
}
