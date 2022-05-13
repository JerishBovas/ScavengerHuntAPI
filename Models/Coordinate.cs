using System.ComponentModel.DataAnnotations.Schema;

namespace ScavengerHunt_API.Models
{
    public class Coordinate
    {
        public int Id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}
