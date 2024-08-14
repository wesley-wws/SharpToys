using System;
using AutoFixture;

namespace SharpToys.Snowflake.Test;

public class SnowflakeIdOptionTest
{
    [Fact]
    public void GetTicks_UTC_ExpectedResult()
    {
        var fixture = new Fixture();

        

        var time = DateTimeOffset.UtcNow;
        var option = new SnowflakeIdOption(time);

        var result = option.GetTicks();

        Assert.Equal(time.UtcTicks,result);
    }
}
