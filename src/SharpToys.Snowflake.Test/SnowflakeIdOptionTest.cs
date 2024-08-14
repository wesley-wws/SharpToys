
namespace SharpToys.Snowflake.Test;

public class SnowflakeIdOptionTest
{
    [Fact]
    public void GetTicks_DateTimeOffset_ExpectedResult()
    {
        var time = DateTimeOffset.Now;

        var option = new SnowflakeIdOption(time);

        var result = option.GetTicks();

        Assert.Equal(time.UtcTicks, result);
    }

    [Fact]
    public void GetTicks_UtcTicks_ExpectedResult()
    {
        var time = DateTimeOffset.Now;

        var option = new SnowflakeIdOption(time.UtcTicks);

        var result = option.GetTicks();

        Assert.Equal(time.UtcTicks, result);
    }
}
