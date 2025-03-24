using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharpToys.Snowflake.Test;

public class SnowflakeIdGeneratorTest
{
    #region Constructor Tests
    
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
    public void Constructor_NullOption_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SnowflakeIdGenerator(null!));
    }
    
    #endregion

    #region Next Method Tests
    
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
    
    [Fact]
    public void Next_WithDifferentWorkerIds_GeneratesDifferentIds()
    {
        // Arrange
        var generator1 = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });
        
        var generator2 = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 2,
            DatacenterId = 1,
        });

        // Act
        var id1 = generator1.Next();
        var id2 = generator2.Next();

        // Assert
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void Next_WithDifferentDatacenterIds_GeneratesDifferentIds()
    {
        // Arrange
        var generator1 = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });
        
        var generator2 = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 2,
        });

        // Act
        var id1 = generator1.Next();
        var id2 = generator2.Next();

        // Assert
        Assert.NotEqual(id1, id2);
    }
    
    #endregion

    #region NextBatch Method Tests
    
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void NextBatch_ReturnsCorrectNumberOfUniqueIds(int batchSize)
    {
        // Arrange
        var generator = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });

        // Act
        var ids = generator.NextBatch(batchSize);

        // Assert
        Assert.Equal(batchSize, ids.Length);
        Assert.Equal(batchSize, ids.Distinct().Count());
    }

    [Fact]
    public void NextBatch_WithInvalidCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var generator = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => generator.NextBatch(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => generator.NextBatch(-1));
    }

    [Fact]
    public void NextBatch_ComparedToIndividualCalls_GeneratesSameNumberOfIds()
    {
        // Arrange
        var generator = new SnowflakeIdGenerator(new SnowflakeIdOption
        {
            WorkerId = 1,
            DatacenterId = 1,
        });
        int count = 50;

        // Act
        var batchIds = generator.NextBatch(count);
        var individualIds = new List<long>(count);
        for (int i = 0; i < count; i++)
        {
            individualIds.Add(generator.Next());
        }

        // Assert
        Assert.Equal(count, batchIds.Length);
        Assert.Equal(count, individualIds.Count);
    }
    
    #endregion
}