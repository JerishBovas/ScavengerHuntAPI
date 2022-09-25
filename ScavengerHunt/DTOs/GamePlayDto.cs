namespace ScavengerHunt.DTOs
{
    public record struct GamePlayDto
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        public string GameName { get; set; }
        public int NoOfItems { get; set; }
        public int GameDuration {get; set; }
        public int Score { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}
