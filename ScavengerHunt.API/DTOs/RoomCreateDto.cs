using System;
namespace ScavengerHunt.API.DTOs
{
	public record struct RoomCreateDto
	{
		public string Name { get; set; }
		public string Details { get; set; }
	}
}

