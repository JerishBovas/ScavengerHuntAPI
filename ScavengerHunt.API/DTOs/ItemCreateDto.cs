namespace ScavengerHunt.API.DTOs
{
	public record struct ItemCreateDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ItemLocation { get; set; }
		public string ImageName { get; set; }
	}
}

