using Amazon.Rekognition.Model;

namespace ScavengerHunt.DTOs
{
	public record struct ItemCreateDto
	{
		public string Name { get; set; }
        public string ImageUrl { get; set; }
	}
}

