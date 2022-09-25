namespace ScavengerHunt.DTOs
{
    public record struct GameDto
    {
        public Guid Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public int Items { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public Double Ratings { get; set; }
        public List<string> Tags { get; set; }
        public int GameDuration {get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
