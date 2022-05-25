namespace ScavengerHunt.DTOs
{
	public record struct LocationCreateDto
	{
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
    }
}