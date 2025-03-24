using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToys.Snowflake;

/// <summary>
/// Configuration options for the Snowflake ID generator.
/// </summary>
/// <remarks>
/// This class provides configuration for the <see cref="SnowflakeIdGenerator"/>, including:
/// - The epoch start date (custom or Unix epoch)
/// - Datacenter ID
/// - Worker ID
/// 
/// The epoch start date is used as the reference point for timestamp calculations.
/// </remarks>
public class SnowflakeIdOption
{
    public readonly long _startDateUtcTicks;

    /// <summary>
    /// Gets or initializes the datacenter ID (0-31).
    /// </summary>
    /// <remarks>
    /// This value should be unique across different datacenters in a distributed system.
    /// Valid range is 0 to 31 (5 bits).
    /// </remarks>
    public long DatacenterId { get; init; }

    /// <summary>
    /// Gets or initializes the worker ID (0-31).
    /// </summary>
    /// <remarks>
    /// This value should be unique across different worker nodes within the same datacenter.
    /// Valid range is 0 to 31 (5 bits).
    /// </remarks>
    public long WorkerId { get; init; }

    /// <summary>
    /// Gets the start date in UTC used as the epoch for ID generation.
    /// </summary>
    public DateTimeOffset StartDateUtc => new(_startDateUtcTicks, TimeSpan.Zero);

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeIdOption"/> class with the Unix epoch (1970-01-01) as the start date.
    /// </summary>
    public SnowflakeIdOption()
    {
        _startDateUtcTicks = DateTimeOffset.UnixEpoch.UtcTicks;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeIdOption"/> class with a custom epoch.
    /// </summary>
    /// <param name="utcTicks">The custom epoch start time in UTC ticks.</param>
    public SnowflakeIdOption(long utcTicks)
    {
        _startDateUtcTicks = utcTicks;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeIdOption"/> class with a custom epoch.
    /// </summary>
    /// <param name="dateTime">The custom epoch start time.</param>
    public SnowflakeIdOption(DateTimeOffset dateTime)
    {
        _startDateUtcTicks = dateTime.UtcTicks;
    }

    /// <summary>
    /// Gets the epoch start time in ticks.
    /// </summary>
    /// <returns>The epoch start time in ticks.</returns>
    public long GetTicks()
    {
        return _startDateUtcTicks;
    }
}
