namespace ScavengerHunt.API.DTOs
{
	public record struct CoordinateCreateDto
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}

