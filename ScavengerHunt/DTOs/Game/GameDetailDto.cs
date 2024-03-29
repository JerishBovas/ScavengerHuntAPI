﻿namespace ScavengerHunt.DTOs
{
	public struct GameDetailDto
	{
        public string Id { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsReadyToPlay { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string UserId { get; set; }
        public bool IsUser { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public ICollection<ItemDto> Items { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public Double Ratings { get; set; }
        public List<string> Tags { get; set; }
        public int GameDuration { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}

