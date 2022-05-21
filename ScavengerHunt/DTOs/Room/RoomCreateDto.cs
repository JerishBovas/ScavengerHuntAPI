using System;
namespace ScavengerHunt.DTOs
{
	public record struct RoomCreateDto
	{
		public string Name { get; set; }
		public string Details { get; set; }
	}
}

