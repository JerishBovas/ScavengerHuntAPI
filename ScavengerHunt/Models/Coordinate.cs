namespace ScavengerHunt.Models
{
    // This Object holds the latitude and longitude information of a location
    public record Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
