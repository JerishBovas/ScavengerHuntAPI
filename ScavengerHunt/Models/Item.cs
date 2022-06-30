namespace ScavengerHunt.Models
{
    public record Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemGame { get; set; }
        public string ImageName { get; set; }

        public Item(string name, string description, string itemGame, string imageName)
        {
            Name = name;
            Description = description;
            ItemGame = itemGame;
            ImageName = imageName;
        }
    }
}