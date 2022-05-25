namespace ScavengerHunt.Models
{
    public record ScoreLog
    {
        public DateTimeOffset DatePlayed { get; set; } = DateTimeOffset.UtcNow;
        public string LocationName { get; set; }
        public int Score { get; set; }
    }
}