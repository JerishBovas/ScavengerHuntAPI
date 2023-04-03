namespace ScavengerHunt.Models
{
    // This Object holds the data about each games
    // Every users can see other user's games as long as its public
    // Only owner can edit the game.
    public record Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsPrivate { get; set; }
        public bool IsReadyToPlay { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Address { get; set; } = "";
        public string Country { get; set; } = ""; 
        public Guid UserId { get; set; }
        public Coordinate Coordinate { get; set; } = new Coordinate();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public string ImageName { get; set; } = "";
        public int Difficulty { get; set; } = 0;
        public List<int> Ratings { get; set; } = new List<int>();
        public List<string> Tags { get; set; } = new List<string>();
        public int GameDuration {get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
        public int TimesPlayed { get; set; } = 0;
    }
}
