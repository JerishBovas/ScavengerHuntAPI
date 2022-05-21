namespace ScavengerHunt.Models
{
    public record UserLog
    {
        public int Id { get; set; }
        public int UserScore { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<ScoreLog> ScoreLog { get; set; } = new List<ScoreLog>();
    }
}
