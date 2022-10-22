namespace ScavengerHunt.DTOs
{
	public record struct GameCreateDto
	{
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public int GameDuration { get; set; }
    }
}