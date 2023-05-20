// This is the object for each items added to a game
using Amazon.Rekognition.Model;

namespace ScavengerHunt.Models
{
    public record Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}