namespace ScavengerHunt.DTOs
{
    public record struct GameScoreDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public int NoOfItems { get; set; }
        public int ItemsFound { get; set; }
        public int Score { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}
