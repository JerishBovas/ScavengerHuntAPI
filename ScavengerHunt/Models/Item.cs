// This is the object for each items added to a game

namespace ScavengerHunt.Models
{
    public class Item
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}