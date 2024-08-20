# SharpToys

## SharpToys.Snowflake
[![NuGet Status](http://img.shields.io/nuget/v/SharpToys.Snowflake.svg?style=flat)](https://www.nuget.org/packages/SharpToys.Snowflake/)

```C#
var generator = new SnowflakeIdGenerator(new SnowflakeIdOption(DateTimeOffset.UtcNow)
{
    DatacenterId = SnowflakeIdGenerator.MinDatacenterId,
    WorkerId = SnowflakeIdGenerator.MinWorkerId
});

generator.Next();
```
