namespace ScavengerHunt.DTOs
{
    public record struct ScoreLogDto
    {
        public DateTimeOffset DatePlayed { get; init; }
        public string LocationName { get; init; }
        public int Score { get; init; }
    }
}
