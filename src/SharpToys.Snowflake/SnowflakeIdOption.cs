using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToys.Snowflake
{
    public class SnowflakeIdOption
    {
        public long DatacenterId { get; init; }

        public long WorkerId { get; init; }

        public DateTime StartDateTimeUtc { get; set; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
