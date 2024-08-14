using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToys.Snowflake;

public class SnowflakeIdOption
{
    public long DatacenterId { get; init; }

    public long WorkerId { get; init; }

    public DateTimeOffset StartDate { get; private set; }


    public SnowflakeIdOption()
    {
        StartDate = DateTimeOffset.UnixEpoch;
    }

    public SnowflakeIdOption(DateTimeOffset dateTime)
    {
        StartDate = dateTime;
    }


    public long GetTicks()
    {
        return StartDate.UtcTicks;
    }
}
