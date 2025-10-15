// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using KZDev.PerfUtils;

namespace KZDev.PerfUtils.Examples.DynamicKeys;

/// <summary>
/// Demonstrates basic usage of DynamicKey for creating single keys from various types.
/// </summary>
public static class BasicUsageExample
{
  /// <summary>
  /// Runs the basic usage examples.
  /// </summary>
  public static void Run ()
  {
    Console.WriteLine("=== DynamicKey Basic Usage Examples ===");

    // Integer keys
    Console.WriteLine(@"1. Integer Keys:");
    DynamicKey intKey = DynamicKey.GetKey(42);
    Console.WriteLine($@"   DynamicKey.GetKey(42) = {intKey}");
    Console.WriteLine($"   Hash Code: {intKey.GetHashCode()}");

    // Long keys
    Console.WriteLine(@"2. Long Keys:");
    DynamicKey longKey = DynamicKey.GetKey(123456789L);
    Console.WriteLine($@"   DynamicKey.GetKey(123456789L) = {longKey}");
    Console.WriteLine($"   Hash Code: {longKey.GetHashCode()}");

    // String keys
    Console.WriteLine(@"3. String Keys:");
    DynamicKey stringKey = DynamicKey.GetKey("user-123");
    Console.WriteLine($@"   DynamicKey.GetKey(""user-123"") = {stringKey}");
    Console.WriteLine($"   Hash Code: {stringKey.GetHashCode()}");

    // Guid keys
    Console.WriteLine(@"4. Guid Keys:");
    Guid guid = Guid.NewGuid();
    DynamicKey guidKey = DynamicKey.GetKey(guid);
    Console.WriteLine($@"   DynamicKey.GetKey(guid) = {guidKey}");
    Console.WriteLine($"   Hash Code: {guidKey.GetHashCode()}");

    // Boolean keys
    Console.WriteLine(@"5. Boolean Keys:");
    DynamicKey boolKey = DynamicKey.GetKey(true);
    Console.WriteLine($@"   DynamicKey.GetKey(true) = {boolKey}");
    Console.WriteLine($"   Hash Code: {boolKey.GetHashCode()}");

    // Type keys
    Console.WriteLine(@"6. Type Keys:");
    DynamicKey typeKey = DynamicKey.GetKey(typeof(string));
    Console.WriteLine($@"   DynamicKey.GetKey(typeof(string)) = {typeKey}");
    Console.WriteLine($"   Hash Code: {typeKey.GetHashCode()}");

    // Enum keys
    Console.WriteLine(@"7. Enum Keys:");
    DynamicKey enumKey = DynamicKey.GetEnumKey(ConsoleColor.Red);
    Console.WriteLine($@"   DynamicKey.GetEnumKey(ConsoleColor.Red) = {enumKey}");
    Console.WriteLine($"   Hash Code: {enumKey.GetHashCode()}");

    // Object keys
    Console.WriteLine(@"8. Object Keys:");
    var obj = new { Name = "Test", Value = 42 };
    DynamicKey objKey = DynamicKey.GetKey(obj);
    Console.WriteLine($@"   DynamicKey.GetKey(anonymous object) = {objKey}");
    Console.WriteLine($"   Hash Code: {objKey.GetHashCode()}");

    // Value type keys
    Console.WriteLine(@"9. Value Type Keys:");
    DynamicKey valueKey = DynamicKey.GetValueKey(DateTime.Now.Date);
    Console.WriteLine($@"   DynamicKey.GetValueKey(DateTime.Now.Date) = {valueKey}");
    Console.WriteLine($"   Hash Code: {valueKey.GetHashCode()}");

    Console.WriteLine(@"=== Basic Usage Examples Complete ===");
  }
}
