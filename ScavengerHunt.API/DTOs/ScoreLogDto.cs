namespace ScavengerHunt.DTOs
{
    public record struct ScoreLogDto
    {
        public int Id { get; init; }
        public DateTime DatePlayed { get; init; }
        public string LocationName { get; init; }
        public int Score { get; init; }
    }
}
