using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SharpToys.Snowflake;

/// <summary>
/// Generates unique distributed IDs based on the Snowflake algorithm.
/// This implementation provides time-sorted, unique 64-bit IDs suitable for distributed systems.
/// </summary>
/// <remarks>
/// The ID structure consists of:
/// - 41 bits for timestamp (milliseconds since a custom epoch)
/// - 5 bits for datacenter ID
/// - 5 bits for worker ID
/// - 5 bits for sequence number within the same millisecond
/// 
/// This provides approximately 69 years of ID generation capability from the epoch start date.
/// </remarks>
public sealed class SnowflakeIdGenerator
{
    private const int _workerIdBitsLength = 5;
    private const int _datacenterIdBitsLength = 5;
    private const int _sequenceBitsLength = 5;
    private const int _workerIdShift = _sequenceBitsLength;
    private const int _datacenterIdShift = _sequenceBitsLength + _workerIdBitsLength;
    private const int _timestampShift = _sequenceBitsLength + _workerIdBitsLength + _datacenterIdBitsLength;
    private const long _sequenceMask = -1L ^ (-1L << _sequenceBitsLength);
    private const int _spinWaitThreshold = 10; // Number of spin iterations before yielding

    /// <summary>
    /// The minimum allowed worker ID value.
    /// </summary>
    public const long MinWorkerId = 0;

    /// <summary>
    /// The minimum allowed datacenter ID value.
    /// </summary>
    public const long MinDatacenterId = 0;

    /// <summary>
    /// The maximum allowed worker ID value (31).
    /// </summary>
    public const long MaxWorkerId = -1L ^ (-1L << _workerIdBitsLength);

    /// <summary>
    /// The maximum allowed datacenter ID value (31).
    /// </summary>
    public const long MaxDatacenterId = -1L ^ (-1L << _datacenterIdBitsLength);

    private long _sequence = 0L;
    private long _lastTimestamp = -1L;
    private readonly SnowflakeIdOption _option;
    private readonly object _lock = new();

    // Pre-calculated values for better performance
    private readonly long _datacenterIdBits;
    private readonly long _workerIdBits;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeIdGenerator"/> class.
    /// </summary>
    /// <param name="option">Configuration options for the Snowflake ID generator.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the worker ID or datacenter ID is outside the allowed range.
    /// </exception>
    public SnowflakeIdGenerator(SnowflakeIdOption option)
    {
        Guard.IsNotNull(option);
        Guard.IsBetweenOrEqualTo(option.WorkerId, MinWorkerId, MaxWorkerId, nameof(option.WorkerId));
        Guard.IsBetweenOrEqualTo(option.DatacenterId, MinDatacenterId, MaxDatacenterId, nameof(option.DatacenterId));

        _option = option;
        
        // Pre-calculate these values for better performance in the hot path
        _datacenterIdBits = option.DatacenterId << _datacenterIdShift;
        _workerIdBits = option.WorkerId << _workerIdShift;
    }

    /// <summary>
    /// Generates the next unique ID.
    /// </summary>
    /// <returns>A unique 64-bit ID.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the system clock moves backwards, which could lead to duplicate IDs.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Next()
    {
        lock (_lock)
        {
            long timestamp = GetTimeStamp();

            if (_lastTimestamp > timestamp)
            {
                ThrowHelper.ThrowInvalidOperationException($"Clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp} milliseconds.");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & _sequenceMask;

                if (_sequence == 0)
                {
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            // Use pre-calculated values and combine in one step for better performance
            return (timestamp << _timestampShift) | _datacenterIdBits | _workerIdBits | _sequence;
        }
    }

    /// <summary>
    /// Generates a batch of unique IDs.
    /// </summary>
    /// <param name="count">The number of IDs to generate.</param>
    /// <returns>An array of unique 64-bit IDs.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when count is less than 1.</exception>
    public long[] NextBatch(int count)
    {
        Guard.IsGreaterThan(count, 0, nameof(count));
        
        var ids = new long[count];
        for (int i = 0; i < count; i++)
        {
            ids[i] = Next();
        }
        return ids;
    }

    /// <summary>
    /// Waits until the next millisecond to avoid sequence overflow.
    /// </summary>
    /// <param name="lastTimestamp">The last timestamp when an ID was generated.</param>
    /// <returns>The timestamp of the next millisecond.</returns>
    private long TilNextMillis(long lastTimestamp)
    {
        long timestamp = GetTimeStamp();
        int spinCount = 0;
        
        while (timestamp <= lastTimestamp)
        {
            if (spinCount++ > _spinWaitThreshold)
            {
                // After spinning for a while, yield the thread to avoid excessive CPU usage
                Thread.Yield();
            }
            timestamp = GetTimeStamp();
        }
        
        return timestamp;
    }

    /// <summary>
    /// Gets the current timestamp in milliseconds since the configured epoch.
    /// </summary>
    /// <returns>The current timestamp in milliseconds.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long GetTimeStamp()
    {
        return (DateTime.UtcNow.Ticks - _option.GetTicks()) / 10000;
    }
}
