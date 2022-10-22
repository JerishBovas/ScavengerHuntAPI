﻿namespace ScavengerHunt.DTOs
{
	public record struct ItemCreateDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
        public string ImageName { get; set; }
		public string Label { get; set; }
	}
}

