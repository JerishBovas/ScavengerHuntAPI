// It is the score object thats created when each play starts playing a game
// Then name can be confusing but it is what it is
// It holds valuable data about the game like start time endtime
namespace ScavengerHunt.Models
{
    public record GamePlay
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool GameEnded { get; set; } = false;
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        public string GameName { get; set; } = "";
        public int NoOfItems { get; set; }
        public int GameDuration {get; set; }
        public List<Item> ItemsLeftToFind { get; set; } = new();
        public int Score { get; set; } = 0;
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}