// This is the object for each items added to a game
namespace ScavengerHunt.Models
{
    public record Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageName { get; set; } = "";
        public string Label { get; set; } = "";
    }
}