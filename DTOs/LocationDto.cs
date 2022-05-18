using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.DTOs
{
    public record struct LocationDto
    {
        public int Id { get; init; }
        public bool IsPrivate { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Address { get; init; }
        public CoordinateDto Coordinate { get; init; }
        public string ImageName { get; init; }
        public int Difficulty { get; init; }
        public String Tags { get; init; }
    }
}
