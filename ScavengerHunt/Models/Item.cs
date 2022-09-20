// This is the object for each items added to a game
namespace ScavengerHunt.Models
{
    public record Item
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageName { get; set; } = "";
    }
}