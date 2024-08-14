using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToys.Snowflake;

public class SnowflakeIdOption
{
    public readonly long _startDateUtcTicks;


    public long DatacenterId { get; init; }

    public long WorkerId { get; init; }

    public DateTimeOffset StartDateUtc => new(_startDateUtcTicks, TimeSpan.Zero);


    public SnowflakeIdOption()
    {
        _startDateUtcTicks = DateTimeOffset.UnixEpoch.UtcTicks;
    }

    public SnowflakeIdOption(long utcTicks)
    {
        _startDateUtcTicks = utcTicks;
    }

    public SnowflakeIdOption(DateTimeOffset dateTime)
    {
        _startDateUtcTicks = dateTime.UtcTicks;
    }

    public long GetTicks()
    {
        return _startDateUtcTicks;
    }
}
