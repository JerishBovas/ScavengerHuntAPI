using System;
namespace ScavengerHunt.DTOs
{
	public record struct LocationDetailDto
	{
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public ICollection<RoomDto> Rooms { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public string Ratings { get; set; }
        public string Tags { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}

