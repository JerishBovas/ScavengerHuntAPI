namespace ScavengerHunt.DTOs
{
	public record struct TeamCreateDto
	{
		public bool IsOpen { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}
}

