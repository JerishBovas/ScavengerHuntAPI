namespace ScavengerHunt.Models
{
    public record ScoreLog
    {
        public DateTimeOffset DatePlayed { get; set; }
        public string LocationName { get; set; }
        public int Score { get; set; }

        public ScoreLog(string locationName, int score)
        {
            DatePlayed = DateTimeOffset.UtcNow;
            LocationName = locationName;
            Score = score;
        }

        public ScoreLog(DateTimeOffset datePlayed, string locationName, int score)
        {
            DatePlayed = datePlayed;
            LocationName = locationName;
            Score = score;
        }
    }
}