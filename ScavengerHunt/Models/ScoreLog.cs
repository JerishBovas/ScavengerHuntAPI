namespace ScavengerHunt.Models
{
    public record ScoreLog
    {
        public DateTimeOffset DatePlayed { get; set; }
        public string GameName { get; set; }
        public int Score { get; set; }

        public ScoreLog(string gameName, int score)
        {
            DatePlayed = DateTimeOffset.UtcNow;
            GameName = gameName;
            Score = score;
        }

        public ScoreLog(DateTimeOffset datePlayed, string gameName, int score)
        {
            DatePlayed = datePlayed;
            GameName = gameName;
            Score = score;
        }
    }
}