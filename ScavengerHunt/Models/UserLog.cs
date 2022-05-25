namespace ScavengerHunt.Models
{
    public record UserLog
    {
        public int UserScore { get; set; }
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<ScoreLog> ScoreLog { get; set; } = new List<ScoreLog>();
    }
}
