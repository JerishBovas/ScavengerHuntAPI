namespace ScavengerHunt.DTOs
{
	public record struct GroupCreateDto
	{
		public bool IsOpen { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string GroupIcon { get; set; }
	}
}

