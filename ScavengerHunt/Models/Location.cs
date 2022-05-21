namespace ScavengerHunt.Models
{
    public record Location
    {
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Coordinate Coordinate { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public string Ratings { get; set; } = "";
        public string Tags { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdated { get; set; }
    }
}
