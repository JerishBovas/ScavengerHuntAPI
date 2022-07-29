using System;
using System.Collections.Generic;

namespace ScavengerHuntFunctions.Models
{
    public record UserLog
    {
        public int UserScore { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public ICollection<ScoreLog> ScoreLog { get; set; }
    }
}
