using System.ComponentModel.DataAnnotations.Schema;

namespace ScavengerHunt_API.Models
{
    public record class Coordinate
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
