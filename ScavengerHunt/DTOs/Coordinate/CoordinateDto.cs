namespace ScavengerHunt.DTOs
{
    public record struct CoordinateDto
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
