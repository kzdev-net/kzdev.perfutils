// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using KZDev.PerfUtils;

namespace DynamicKeyBenchmarks.ThroughputBenchmarks;

/// <summary>
/// Benchmarks comparing DynamicKey composite key approaches against string concatenation
/// for dictionary lookup performance.
/// </summary>
[MemoryDiagnoser]
public class CompositeKeyLookupBenchmarks
{
    private Dictionary<DynamicKey, string> _dynamicKeyDictionary = null!;
    private Dictionary<string, string> _stringDictionary = null!;

    // Test data
    private const int NumEntries = 1000;
    private const int UserId = 12345;
    private const string SessionId = "session-abc-123";
    private const long Timestamp = 1704067200000L;
    private const bool IsAdmin = true;
    private const string QueryParams = "page=1&size=10&sort=name";
    private const uint TenantId = 999u;
    private const ulong RequestId = 9876543210UL;
    private const string UserAgent = "Mozilla/5.0";
    private const string IpAddress = "192.168.1.100";
    private const string Feature = "advanced-search";
    private const int PageSize = 25;
    private const string SortOrder = "desc";

    private static readonly int[] UserIds = new int[NumEntries];
    private static readonly string[] SessionIds = new string[NumEntries];
    private static readonly long[] Timestamps = new long[NumEntries];

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Common global setup for all benchmarks.
    /// </summary>
    [GlobalSetup]
    public void GlobalSetup ()
    {
        // Create dictionaries with test data
        _dynamicKeyDictionary = new Dictionary<DynamicKey, string>();
        _stringDictionary = new Dictionary<string, string>();

        // Populate with NumEntries entries
        for (int i = 0; i < NumEntries; i++)
        {
            int userId = UserId + i;
            string sessionId = $"{SessionId}-{i}";
            long timestamp = Timestamp + i;
            string value = $"value-{i}";

            // Store for lookup benchmarks
            UserIds[i] = userId;
            SessionIds[i] = sessionId;
            Timestamps[i] = timestamp;

            // DynamicKey entries
            DynamicKey key2 = DynamicKey.GetKey(userId, sessionId);
            DynamicKey key3 = DynamicKey.GetKey(userId, sessionId, timestamp);
            DynamicKey key4 = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin);
            DynamicKey key5 = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin, QueryParams);
            DynamicKey key12 = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin, QueryParams,
                TenantId, RequestId, UserAgent, IpAddress, Feature, PageSize, SortOrder);

            _dynamicKeyDictionary[key2] = value;
            _dynamicKeyDictionary[key3] = value;
            _dynamicKeyDictionary[key4] = value;
            _dynamicKeyDictionary[key5] = value;
            _dynamicKeyDictionary[key12] = value;

            // String entries
            string str2 = $"{userId}|{sessionId}";
            string str3 = $"{userId}|{sessionId}|{timestamp}";
            string str4 = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}";
            string str5 = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}|{QueryParams}";
            string str12 = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}|{QueryParams}|{TenantId}|{RequestId}|{UserAgent}|{IpAddress}|{Feature}|{PageSize}|{SortOrder}";

            _stringDictionary[str2] = value;
            _stringDictionary[str3] = value;
            _stringDictionary[str4] = value;
            _stringDictionary[str5] = value;
            _stringDictionary[str12] = value;
        }
    }
    //--------------------------------------------------------------------------------

    #region Multi-Parameter GetKey Benchmarks

    //--------------------------------------------------------------------------------
    [Benchmark(Baseline = true)]
    [Arguments(NumEntries / 2)]
    public string MultiParameterGetKey_2Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        DynamicKey key = DynamicKey.GetKey(userId, sessionId);
        return _dynamicKeyDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string StringConcatenation_2Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        string key = $"{userId}|{sessionId}";
        return _stringDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string MultiParameterGetKey_3Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        long timestamp = Timestamps[index];
        DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp);
        return _dynamicKeyDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string StringConcatenation_3Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        long timestamp = Timestamps[index];
        string key = $"{userId}|{sessionId}|{timestamp}";
        return _stringDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string MultiParameterGetKey_4Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        long timestamp = Timestamps[index];
        DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin);
        return _dynamicKeyDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string StringConcatenation_4Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        long timestamp = Timestamps[index];
        string key = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}";
        return _stringDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string MultiParameterGetKey_5Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        long timestamp = Timestamps[index];
        DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin, QueryParams);
        return _dynamicKeyDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    [Arguments(NumEntries / 2)]
    public string StringConcatenation_5Params (int index)
    {
        int userId = UserIds[index];
        string sessionId = SessionIds[index];
        long timestamp = Timestamps[index];
        string key = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}|{QueryParams}";
        return _stringDictionary[key];
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    public void MultiParameterGetKey_4Params_WithLoop ()
    {
        for (int index = 0; index < NumEntries; index += 10)
        {
            int userId = UserIds[index];
            string sessionId = SessionIds[index];
            long timestamp = Timestamps[index];
            DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin);
            _ = _dynamicKeyDictionary[key];
        }
    }
    //--------------------------------------------------------------------------------
    [Benchmark]
    public void StringConcatenation_4Params_WithLoop ()
    {
        for (int index = 0; index < NumEntries; index += 10)
        {
            int userId = UserIds[index];
            string sessionId = SessionIds[index];
            long timestamp = Timestamps[index];
            string key = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}";
            _ = _stringDictionary[key];
        }
    }
    //--------------------------------------------------------------------------------
    //[Benchmark]
    //[Arguments(NumEntries / 2)]
    //public string MultiParameterGetKey_12Params (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    long timestamp = Timestamps[index];
    //    DynamicKey key = DynamicKey.GetKey(userId, sessionId, timestamp, IsAdmin, QueryParams,
    //        TenantId, RequestId, UserAgent, IpAddress, Feature, PageSize, SortOrder);
    //    return _dynamicKeyDictionary[key];
    //}
    ////--------------------------------------------------------------------------------
    //[Benchmark]
    //[Arguments(NumEntries / 2)]
    //public string StringConcatenation_12Params (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    long timestamp = Timestamps[index];
    //    string key = $"{userId}|{sessionId}|{timestamp}|{IsAdmin}|{QueryParams}|{TenantId}|{RequestId}|{UserAgent}|{IpAddress}|{Feature}|{PageSize}|{SortOrder}";
    //    return _stringDictionary[key];
    //}
    ////--------------------------------------------------------------------------------

    #endregion

    #region Operator+ Benchmarks

    //[Benchmark]
    //[Arguments(NumEntries / 2)]
    //public string OperatorPlus_2Keys (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    DynamicKey key = DynamicKey.GetKey(userId) + DynamicKey.GetKey(sessionId);
    //    return _dynamicKeyDictionary[key];
    //}

    //[Benchmark]
    //[Arguments(NumEntries / 2)]
    //public string OperatorPlus_3Keys (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    long timestamp = Timestamps[index];
    //    DynamicKey key = DynamicKey.GetKey(userId) + DynamicKey.GetKey(sessionId) + DynamicKey.GetKey(timestamp);
    //    return _dynamicKeyDictionary[key];
    //}

    #endregion

    //#region Combine Method Benchmarks

    //[Benchmark]
    //[Arguments(500)]
    //public string Combine_2Keys (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    DynamicKey key = DynamicKey.Combine(DynamicKey.GetKey(userId), DynamicKey.GetKey(sessionId));
    //    return _dynamicKeyDictionary[key];
    //}

    //[Benchmark]
    //[Arguments(500)]
    //public string Combine_3Keys (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    long timestamp = Timestamps[index];
    //    DynamicKey key = DynamicKey.Combine(
    //        DynamicKey.GetKey(userId),
    //        DynamicKey.GetKey(sessionId),
    //        DynamicKey.GetKey(timestamp));
    //    return _dynamicKeyDictionary[key];
    //}

    //#endregion

    //#region Builder Pattern Benchmarks

    //[Benchmark]
    //[Arguments(500)]
    //public string Builder_2Keys (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    DynamicKey key = DynamicKeyBuilder.Create()
    //        .Add(userId)
    //        .Add(sessionId)
    //        .Build();
    //    return _dynamicKeyDictionary[key];
    //}

    //[Benchmark]
    //[Arguments(500)]
    //public string Builder_3Keys (int index)
    //{
    //    int userId = UserIds[index];
    //    string sessionId = SessionIds[index];
    //    long timestamp = Timestamps[index];
    //    DynamicKey key = DynamicKeyBuilder.Create()
    //        .Add(userId)
    //        .Add(sessionId)
    //        .Add(timestamp)
    //        .Build();
    //    return _dynamicKeyDictionary[key];
    //}

    //#endregion
}
