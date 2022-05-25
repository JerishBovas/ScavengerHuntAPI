namespace ScavengerHunt.Models
{
    public record Location
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public Guid UserId { get; set; }
        public Coordinate Coordinate { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public List<int> Ratings { get; set; } = new List<int>();
        public List<string> Tags { get; set; } = new List<string>();
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
    }
}
