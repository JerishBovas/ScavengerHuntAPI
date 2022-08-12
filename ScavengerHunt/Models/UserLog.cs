namespace ScavengerHunt.Models
{
    public record UserLog
    {
        public int UserScore { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public ICollection<GameScore> ScoreLog { get; set; }

        public UserLog()
        {
            UserScore = 0;
            LastUpdated = DateTimeOffset.UtcNow;
            ScoreLog = new List<GameScore>();
        }

        public UserLog(int userScore)
        {
            UserScore = userScore;
            LastUpdated = DateTimeOffset.UtcNow;
            ScoreLog = new List<GameScore>();
        }

        public UserLog(int userScore, DateTimeOffset lastUpdated, ICollection<GameScore> scoreLog) : this(userScore)
        {
            LastUpdated = lastUpdated;
            ScoreLog = scoreLog;
        }
    }
}
