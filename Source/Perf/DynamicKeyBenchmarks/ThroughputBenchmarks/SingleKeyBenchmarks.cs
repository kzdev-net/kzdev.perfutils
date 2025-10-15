// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using KZDev.PerfUtils;

namespace DynamicKeyBenchmarks.ThroughputBenchmarks;

/// <summary>
/// Benchmarks comparing single DynamicKey instances against string conversion
/// for dictionary lookup performance across different dictionary sizes.
/// </summary>
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class SingleKeyBenchmarks
{
  private Dictionary<DynamicKey, string> _dynamicKeyDictionary100 = null!;
  private Dictionary<string, string> _stringDictionary100 = null!;
  private Dictionary<DynamicKey, string> _dynamicKeyDictionary1000 = null!;
  private Dictionary<string, string> _stringDictionary1000 = null!;
  private Dictionary<DynamicKey, string> _dynamicKeyDictionary10000 = null!;
  private Dictionary<string, string> _stringDictionary10000 = null!;

  [GlobalSetup]
  public void Setup()
  {
    SetupDictionary(100, out _dynamicKeyDictionary100, out _stringDictionary100);
    SetupDictionary(1000, out _dynamicKeyDictionary1000, out _stringDictionary1000);
    SetupDictionary(10000, out _dynamicKeyDictionary10000, out _stringDictionary10000);
  }

  private void SetupDictionary(int size, out Dictionary<DynamicKey, string> dynamicDict, out Dictionary<string, string> stringDict)
  {
    dynamicDict = new Dictionary<DynamicKey, string>();
    stringDict = new Dictionary<string, string>();

    for (int i = 0; i < size; i++)
    {
      string value = $"value-{i}";

      // Integer keys
      DynamicKey intKey = DynamicKey.GetKey(i);
      string intStr = i.ToString();
      dynamicDict[intKey] = value;
      stringDict[intStr] = value;

      // Long keys
      DynamicKey longKey = DynamicKey.GetKey((long)i);
      string longStr = ((long)i).ToString();
      dynamicDict[longKey] = value;
      stringDict[longStr] = value;

      // String keys
      DynamicKey stringKey = DynamicKey.GetKey($"key-{i}");
      string stringStr = $"key-{i}";
      dynamicDict[stringKey] = value;
      stringDict[stringStr] = value;

      // Guid keys
      Guid guid = Guid.NewGuid();
      DynamicKey guidKey = DynamicKey.GetKey(guid);
      string guidStr = guid.ToString();
      dynamicDict[guidKey] = value;
      stringDict[guidStr] = value;

      // Boolean keys
      DynamicKey boolKey = DynamicKey.GetKey(i % 2 == 0);
      string boolStr = (i % 2 == 0).ToString();
      dynamicDict[boolKey] = value;
      stringDict[boolStr] = value;
    }
  }

  #region Integer Key Benchmarks

  [Benchmark(Baseline = true)]
  [Arguments(50)]
  public string IntegerKey_DynamicKey_100(int index)
  {
    DynamicKey key = DynamicKey.GetKey(index);
    return _dynamicKeyDictionary100[key];
  }

  [Benchmark]
  [Arguments(50)]
  public string IntegerKey_String_100(int index)
  {
    string key = index.ToString();
    return _stringDictionary100[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string IntegerKey_DynamicKey_1000(int index)
  {
    DynamicKey key = DynamicKey.GetKey(index);
    return _dynamicKeyDictionary1000[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string IntegerKey_String_1000(int index)
  {
    string key = index.ToString();
    return _stringDictionary1000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string IntegerKey_DynamicKey_10000(int index)
  {
    DynamicKey key = DynamicKey.GetKey(index);
    return _dynamicKeyDictionary10000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string IntegerKey_String_10000(int index)
  {
    string key = index.ToString();
    return _stringDictionary10000[key];
  }

  #endregion

  #region Long Key Benchmarks

  [Benchmark]
  [Arguments(50)]
  public string LongKey_DynamicKey_100(int index)
  {
    DynamicKey key = DynamicKey.GetKey((long)index);
    return _dynamicKeyDictionary100[key];
  }

  [Benchmark]
  [Arguments(50)]
  public string LongKey_String_100(int index)
  {
    string key = ((long)index).ToString();
    return _stringDictionary100[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string LongKey_DynamicKey_1000(int index)
  {
    DynamicKey key = DynamicKey.GetKey((long)index);
    return _dynamicKeyDictionary1000[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string LongKey_String_1000(int index)
  {
    string key = ((long)index).ToString();
    return _stringDictionary1000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string LongKey_DynamicKey_10000(int index)
  {
    DynamicKey key = DynamicKey.GetKey((long)index);
    return _dynamicKeyDictionary10000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string LongKey_String_10000(int index)
  {
    string key = ((long)index).ToString();
    return _stringDictionary10000[key];
  }

  #endregion

  #region String Key Benchmarks

  [Benchmark]
  [Arguments(50)]
  public string StringKey_DynamicKey_100(int index)
  {
    DynamicKey key = DynamicKey.GetKey($"key-{index}");
    return _dynamicKeyDictionary100[key];
  }

  [Benchmark]
  [Arguments(50)]
  public string StringKey_String_100(int index)
  {
    string key = $"key-{index}";
    return _stringDictionary100[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string StringKey_DynamicKey_1000(int index)
  {
    DynamicKey key = DynamicKey.GetKey($"key-{index}");
    return _dynamicKeyDictionary1000[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string StringKey_String_1000(int index)
  {
    string key = $"key-{index}";
    return _stringDictionary1000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string StringKey_DynamicKey_10000(int index)
  {
    DynamicKey key = DynamicKey.GetKey($"key-{index}");
    return _dynamicKeyDictionary10000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string StringKey_String_10000(int index)
  {
    string key = $"key-{index}";
    return _stringDictionary10000[key];
  }

  #endregion

  #region Guid Key Benchmarks

  [Benchmark]
  [Arguments(50)]
  public string GuidKey_DynamicKey_100(int index)
  {
    // Use a deterministic Guid for benchmarking
    Guid guid = new Guid($"00000000-0000-0000-0000-{index:D12}");
    DynamicKey key = DynamicKey.GetKey(guid);
    return _dynamicKeyDictionary100[key];
  }

  [Benchmark]
  [Arguments(50)]
  public string GuidKey_String_100(int index)
  {
    // Use a deterministic Guid for benchmarking
    Guid guid = new Guid($"00000000-0000-0000-0000-{index:D12}");
    string key = guid.ToString();
    return _stringDictionary100[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string GuidKey_DynamicKey_1000(int index)
  {
    // Use a deterministic Guid for benchmarking
    Guid guid = new Guid($"00000000-0000-0000-0000-{index:D12}");
    DynamicKey key = DynamicKey.GetKey(guid);
    return _dynamicKeyDictionary1000[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string GuidKey_String_1000(int index)
  {
    // Use a deterministic Guid for benchmarking
    Guid guid = new Guid($"00000000-0000-0000-0000-{index:D12}");
    string key = guid.ToString();
    return _stringDictionary1000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string GuidKey_DynamicKey_10000(int index)
  {
    // Use a deterministic Guid for benchmarking
    Guid guid = new Guid($"00000000-0000-0000-0000-{index:D12}");
    DynamicKey key = DynamicKey.GetKey(guid);
    return _dynamicKeyDictionary10000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string GuidKey_String_10000(int index)
  {
    // Use a deterministic Guid for benchmarking
    Guid guid = new Guid($"00000000-0000-0000-0000-{index:D12}");
    string key = guid.ToString();
    return _stringDictionary10000[key];
  }

  #endregion

  #region Boolean Key Benchmarks

  [Benchmark]
  [Arguments(50)]
  public string BooleanKey_DynamicKey_100(int index)
  {
    DynamicKey key = DynamicKey.GetKey(index % 2 == 0);
    return _dynamicKeyDictionary100[key];
  }

  [Benchmark]
  [Arguments(50)]
  public string BooleanKey_String_100(int index)
  {
    string key = (index % 2 == 0).ToString();
    return _stringDictionary100[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string BooleanKey_DynamicKey_1000(int index)
  {
    DynamicKey key = DynamicKey.GetKey(index % 2 == 0);
    return _dynamicKeyDictionary1000[key];
  }

  [Benchmark]
  [Arguments(500)]
  public string BooleanKey_String_1000(int index)
  {
    string key = (index % 2 == 0).ToString();
    return _stringDictionary1000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string BooleanKey_DynamicKey_10000(int index)
  {
    DynamicKey key = DynamicKey.GetKey(index % 2 == 0);
    return _dynamicKeyDictionary10000[key];
  }

  [Benchmark]
  [Arguments(5000)]
  public string BooleanKey_String_10000(int index)
  {
    string key = (index % 2 == 0).ToString();
    return _stringDictionary10000[key];
  }

  #endregion
}
