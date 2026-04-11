// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// <see cref="IRandomSource"/> backed by <see cref="Random"/> with a fixed seed for
/// reproducible test inputs. All members are thread-safe for parallel test tasks.
/// </summary>
public sealed class SeededRandomSource : IRandomSource
{
    /// <summary>
    /// Synchronizes access to the non-thread-safe <see cref="Random"/> instance.
    /// </summary>
    private readonly object _gate = new();

    /// <summary>
    /// The underlying pseudo-random generator.
    /// </summary>
    private readonly Random _random;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance with the specified seed.
    /// </summary>
    /// <param name="seed">
    /// The seed value.
    /// </param>
    public SeededRandomSource (int seed)
    {
        _random = new Random(seed);
    }
    //--------------------------------------------------------------------------------

    #region IRandomSource Implementation

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRandomInteger (int maxValue) => (maxValue > 0) ?
        GetRandomInteger(0, maxValue) :
        throw new ArgumentException($"{nameof(maxValue)} must be greater than zero", nameof(maxValue));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int GetRandomInteger (int minValue, int maxValue)
    {
        if (maxValue < minValue)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue),
                $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        lock (_gate)
        {
            return _random.Next(minValue, maxValue);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public uint GetRandomUnsignedInteger (uint maxValue) => (maxValue == 0) ? 0 : GetRandomUnsignedInteger(0, maxValue);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public uint GetRandomUnsignedInteger (uint minValue, uint maxValue)
    {
        if (maxValue < minValue)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue),
                $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        long range = (long)maxValue - minValue;
        lock (_gate)
        {
            long offset = _random.NextInt64(0, range);
            return (uint)(minValue + offset);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public long GetRandomLongInteger (long maxValue) => (maxValue > 0) ?
        GetRandomLongInteger(0, maxValue) :
        throw new ArgumentException($"{nameof(maxValue)} must be greater than zero", nameof(maxValue));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public long GetRandomLongInteger (long minValue, long maxValue)
    {
        if (maxValue < minValue)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue),
                $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        lock (_gate)
        {
            return _random.NextInt64(minValue, maxValue);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public ulong GetRandomUnsignedLongInteger (ulong maxValue) =>
        (maxValue == 0) ? 0 : GetRandomUnsignedLongInteger(0, maxValue);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public ulong GetRandomUnsignedLongInteger (ulong minValue, ulong maxValue)
    {
        if (maxValue < minValue)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue),
                $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        ulong range = maxValue - minValue;
        lock (_gate)
        {
            return minValue + SampleUniformUlongLessThan(range);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int GetRandomBytes (byte[] byteArray, int byteCount = -1)
    {
        ArgumentNullException.ThrowIfNull(byteArray);
        int useCount = (byteCount < 0) ? byteArray.Length : Math.Min(byteCount, byteArray.Length);
        if (useCount == 0)
        {
            return 0;
        }

        lock (_gate)
        {
            _random.NextBytes(byteArray.AsSpan(0, useCount));
        }

        return useCount;
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public bool GetRandomBoolean ()
    {
        lock (_gate)
        {
            return 0 == (_random.Next() & 1);
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public bool GetRandomFalse (int falseFrequency)
    {
        switch (falseFrequency)
        {
            case 0:
                return true;
            case 1:
                return false;
            default:
                lock (_gate)
                {
                    return 0 != _random.Next(0, falseFrequency);
                }
        }
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public bool GetRandomTrue (int trueFrequency)
    {
        switch (trueFrequency)
        {
            case 0:
                return false;
            case 1:
                return true;
            default:
                lock (_gate)
                {
                    return 0 == _random.Next(0, trueFrequency);
                }
        }
    }
    //--------------------------------------------------------------------------------

    #endregion

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns an integer uniformly distributed in <c>[0, range)</c> using rejection sampling so
    /// that <c>sample % range</c> is not biased when <c>range</c> does not divide <c>2^64</c>.
    /// </summary>
    /// <param name="range">
    /// The exclusive upper bound; must be greater than zero.
    /// </param>
    /// <returns>
    /// A value in <c>[0, range)</c>.
    /// </returns>
    private ulong SampleUniformUlongLessThan (ulong range)
    {
        ulong upperExclusive = (ulong.MaxValue / range) * range;
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        while (true)
        {
            _random.NextBytes(buffer);
            ulong sample = BitConverter.ToUInt64(buffer);
            if (sample < upperExclusive)
            {
                return sample % range;
            }
        }
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
