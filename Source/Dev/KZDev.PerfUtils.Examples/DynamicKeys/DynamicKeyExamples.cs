// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Examples.DynamicKeys;

/// <summary>
/// Main entry point for running all DynamicKey examples.
/// </summary>
public static class DynamicKeyExamples
{
  /// <summary>
  /// Runs all DynamicKey examples in sequence.
  /// </summary>
  public static void RunAll ()
  {
    Console.WriteLine(@"DynamicKey Examples - KZDev.PerfUtils");
    Console.WriteLine(@"=====================================");

    try
    {
      // Run all examples
      BasicUsageExample.Run();
      MultiParameterCompositeExample.Run();
      CompositeKeyExample.Run();
      CachingExample.Run();

      Console.WriteLine(@"All examples completed successfully!");
    }
    catch (Exception ex)
    {
      Console.WriteLine($@"Error running examples: {ex.Message}");
      Console.WriteLine($@"Stack trace: {ex.StackTrace}");
    }
  }

  /// <summary>
  /// Runs a specific example by name.
  /// </summary>
  /// <param name="exampleName">The name of the example to run.</param>
  public static void RunExample (string exampleName)
  {
    Console.WriteLine($@"Running DynamicKey example: {exampleName}");
    Console.WriteLine(@"=====================================");

    try
    {
      switch (exampleName.ToLowerInvariant())
      {
        case "basic":
        case "basicusage":
          BasicUsageExample.Run();
          break;
        case "multiparameter":
        case "multiparametercomposite":
          MultiParameterCompositeExample.Run();
          break;
        case "composite":
        case "compositekey":
          CompositeKeyExample.Run();
          break;
        case "caching":
        case "cache":
          CachingExample.Run();
          break;
        default:
          Console.WriteLine($@"Unknown example: {exampleName}");
          Console.WriteLine(@"Available examples: basic, multiparameter, composite, caching");
          break;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($@"Error running example '{exampleName}': {ex.Message}");
      Console.WriteLine($@"Stack trace: {ex.StackTrace}");
    }
  }
}
