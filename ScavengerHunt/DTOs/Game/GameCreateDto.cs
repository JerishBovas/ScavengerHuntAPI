﻿namespace ScavengerHunt.DTOs
{
	public struct GameCreateDto
	{
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public ICollection<ItemCreateDto> Items { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public int GameDuration { get; set; }
    }
}