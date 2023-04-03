using Amazon.Rekognition.Model;

namespace ScavengerHunt.DTOs
{
	public record struct ItemCreateDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
        public string ImageUrl { get; set; }
		public string Label { get; set; }
		public BoundingBox BoundingBox { get; set; }
	}
}

