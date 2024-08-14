using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharpToys.Snowflake.Test;

public class SnowflakeIdGeneratorTest
{
    [Theory]
    [InlineData(SnowflakeIdGenerator.MinWorkerId, SnowflakeIdGenerator.MinDatacenterId, true)]
    [InlineData(SnowflakeIdGenerator.MaxWorkerId, SnowflakeIdGenerator.MaxDatacenterId, true)]
    [InlineData(SnowflakeIdGenerator.MinWorkerId, SnowflakeIdGenerator.MinDatacenterId - 1, false)]
    [InlineData(SnowflakeIdGenerator.MinWorkerId - 1, SnowflakeIdGenerator.MinDatacenterId, false)]
    [InlineData(SnowflakeIdGenerator.MinWorkerId - 1, SnowflakeIdGenerator.MinDatacenterId - 1, false)]
    [InlineData(SnowflakeIdGenerator.MinWorkerId, SnowflakeIdGenerator.MaxDatacenterId + 1, false)]
    [InlineData(SnowflakeIdGenerator.MaxWorkerId + 1, SnowflakeIdGenerator.MinDatacenterId, false)]
    [InlineData(SnowflakeIdGenerator.MaxWorkerId + 1, SnowflakeIdGenerator.MaxDatacenterId + 1, false)]
    public void Constructor_MultipleOptionCases_ExpectedResult(int workerId, int datacenterId, bool isValide)
    {
        var option = new SnowflakeIdOption
        {
            WorkerId = workerId,
            DatacenterId = datacenterId,
        };

        var ex = Record.Exception(() => new SnowflakeIdGenerator(option));

        if (isValide)
        {
            Assert.Null(ex);
        }
        else
        {
            Assert.IsType<ArgumentOutOfRangeException>(ex);
        }
    }

    [Fact]
    public void Next_ShortTime_NoDuplicates()
    {
        var generator = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });

        var list = new List<long>();
        var count = 100000;

        for (int i = 0; i < count; i++)
        {
            var id = generator.Next();
            list.Add(id);
        }

        Assert.Distinct(list);
        Assert.Equal(count, list.Count);
    }

    [Fact]
    public async Task Next_Multithreading_NoDuplicates()
    {
        var generator = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });

        var t1 = Task.Run(GenerateList);
        var t2 = Task.Run(GenerateList);
        var t3 = Task.Run(GenerateList);

        var results = await Task.WhenAll(t1, t2, t3);

        var list = results.Aggregate(new List<long>(), (x, y) => x.Concat(y).ToList());

        var count = results.Sum(x => x.Count);

        Assert.Distinct(list);
        Assert.Equal(count, list.Count);

        List<long> GenerateList()
        {
            var list = new List<long>();
            var count = 100000;

            for (int i = 0; i < count; i++)
            {
                var id = generator.Next();
                list.Add(id);
            }

            return list;
        }
    }
}