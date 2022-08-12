namespace ScavengerHunt.Models
{
    public record GameScore
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public bool GameEnded { get; set; } = false;
        public Guid GameId { get; set; }
        public string GameName { get; set; } = "";
        public int NoOfItems { get; set; }
        public int ItemsFound { get; set; }
        public int Score { get; set; } = 0;
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset ExpiryTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}