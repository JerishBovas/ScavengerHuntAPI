using ScavengerHunt.Models;

namespace ScavengerHunt.DTOs
{
    public struct GamePlayDto
    {
        public string Id { get; set; }
        public bool GameEnded { get; set; }
        public string GameId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public List<Item> Items { get; set; }
        public int GameDuration {get; set; }
        public int Score { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset Deadline { get; set; }
    }

    public struct VerifiedItemDto
    {
        public bool GameEnded { get; set; }
        public string? ItemToRemove { get; set; }
        public int Score { get; set; }

        public VerifiedItemDto(bool gameEnded, string? itemToRemove, int score)
        {
            this.GameEnded = gameEnded;
            this.ItemToRemove = itemToRemove;
            this.Score = score;
        }
    }
}
