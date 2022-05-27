namespace ScavengerHunt.Models
{
    public record UserLog
    {
        public int UserScore { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public ICollection<ScoreLog> ScoreLog { get; set; }

        public UserLog()
        {
            UserScore = 0;
            LastUpdated = DateTimeOffset.UtcNow;
            ScoreLog = new List<ScoreLog>();
        }

        public UserLog(int userScore)
        {
            UserScore = userScore;
            LastUpdated = DateTimeOffset.UtcNow;
            ScoreLog = new List<ScoreLog>();
        }

        public UserLog(int userScore, DateTimeOffset lastUpdated, ICollection<ScoreLog> scoreLog) : this(userScore)
        {
            LastUpdated = lastUpdated;
            ScoreLog = scoreLog;
        }
    }
}
