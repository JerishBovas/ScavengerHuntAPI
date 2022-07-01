namespace ScavengerHunt.Models
{
    public record Game
    {
        public Guid Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public Guid UserId { get; set; }
        public Coordinate Coordinate { get; set; }
        public ICollection<Item> Items { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public List<int> Ratings { get; set; }
        public List<string> Tags { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public Game()
        {
            Id = Guid.NewGuid();
            Name = "";
            Description = "";
            Address = "";
            Country = "";
            Coordinate = new Coordinate();
            Items = new List<Item>();
            ImageName = "";
            Difficulty = 0;
            Ratings = new List<int>();
            Tags = new List<string>();
            CreatedDate = DateTimeOffset.UtcNow;
            LastUpdated = DateTimeOffset.UtcNow;
        }
        
        public Game(bool isPrivate, string name, string description, string address, string country, Guid userId, Coordinate coordinate,  string imageName, int difficulty, List<string> tags)
        {
            Id = Guid.NewGuid();
            IsPrivate = isPrivate;
            Name = name;
            Description = description;
            Address = address;
            Country = country;
            UserId = userId;
            Coordinate = coordinate;
            Items = new List<Item>();
            ImageName = imageName;
            Difficulty = difficulty;
            Ratings = new List<int>();
            Tags = tags;
            CreatedDate = DateTimeOffset.UtcNow;
            LastUpdated = DateTimeOffset.UtcNow;
        }
    }
}
