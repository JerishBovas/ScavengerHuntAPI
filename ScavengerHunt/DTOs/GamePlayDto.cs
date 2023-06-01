using ScavengerHunt.Models;

namespace ScavengerHunt.DTOs
{
    public record struct GamePlayDto
    {
        public Guid Id { get; set; }
        public bool GameEnded { get; set; }
        public Guid GameId { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public List<Item> ItemsLeftToFind { get; set; }
        public int GameDuration {get; set; }
        public int Score { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset Deadline { get; set; }
    }
}
