namespace ScavengerHunt.Models
{
    public record ScoreLog
    {
        public int Id { get; set; }
        public DateTimeOffset DatePlayed { get; set; }
        public string LocationName { get; set; }
        public int Score { get; set; }
        public UserLog? UserLog { get; set; }
        public Group? Group { get; set; }
    }
}