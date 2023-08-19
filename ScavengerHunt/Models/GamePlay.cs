// It is the score object thats created when each play starts playing a game
// Then name can be confusing but it is what it is
// It holds valuable data about the game like start time endtime
namespace ScavengerHunt.Models
{
    public class GamePlay
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool GameEnded { get; set; } = false;
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        public string GameUserId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Country { get; set; } = "";
        public string UserId { get; set; } = Guid.NewGuid().ToString();
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