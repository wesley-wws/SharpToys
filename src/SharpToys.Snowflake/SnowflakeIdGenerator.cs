using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToys.Snowflake;

public class SnowflakeIdGenerator
{
    private const int _workerIdBitsLength = 5;

    private const int _datacenterIdBitsLength = 5;

    private const int _sequenceBitsLength = 5;

    private const int _workerIdShift = _sequenceBitsLength;

    private const int _datacenterIdShift = _sequenceBitsLength + _workerIdBitsLength;

    private const int _timestampShift = _sequenceBitsLength + _workerIdBitsLength + _datacenterIdBitsLength;

    private const long _sequenceMask = -1L ^ (-1L << _sequenceBitsLength);


    public const long MinWorkerId = 0;

    public const long MinDatacenterId = 0;

    public const long MaxWorkerId = -1L ^ (-1L << _workerIdBitsLength);

    public const long MaxDatacenterId = -1L ^ (-1L << _datacenterIdBitsLength);


    private long _sequence = 0L;

    private long _lastTimestamp = -1L;

    private readonly SnowflakeIdOption _option;

    private readonly object _lock = new();


    public SnowflakeIdGenerator(SnowflakeIdOption option)
    {
        Guard.IsBetweenOrEqualTo(option.WorkerId, MinWorkerId, MaxWorkerId);
        Guard.IsBetweenOrEqualTo(option.DatacenterId, MinDatacenterId, MaxDatacenterId);

        _option = option;
    }

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

            long id = timestamp << _timestampShift;
            id |= (uint)(_option.DatacenterId << _datacenterIdShift);
            id |= (uint)(_option.WorkerId << _workerIdShift);
            id |= _sequence;

            return id;
        }
    }

    protected long TilNextMillis(long lastTimestamp)
    {
        var timestamp = GetTimeStamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetTimeStamp();
        }
        return timestamp;
    }

    protected long GetTimeStamp()
    {
        return (DateTime.UtcNow.Ticks - _option.GetTicks()) / 10000;
    }

}
