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
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Country { get; set; } = "";
        public Guid UserId { get; set; }
        public Coordinate Coordinate { get; set; } = new Coordinate();
        public List<Item> Items { get; set; } = new();
        public List<String> ItemsFound { get; set; } = new();
        public int GameDuration {get; set; }
        public int Score { get; set; } = 0;
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset Deadline { get; set; } = DateTimeOffset.UtcNow.AddMinutes(15);
        public DateTimeOffset? EndTime { get; set; } = null;
    }
}