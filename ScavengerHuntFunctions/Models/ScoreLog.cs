using System;

namespace ScavengerHuntFunctions.Models
{
    public record ScoreLog
    {
        public DateTimeOffset DatePlayed { get; set; }
        public string GameName { get; set; }
        public int Score { get; set; }
    }
}