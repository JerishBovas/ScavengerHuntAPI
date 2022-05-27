namespace ScavengerHunt.Models
{
    public record Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemLocation { get; set; }
        public string ImageName { get; set; }

        public Item(string name, string description, string itemLocation, string imageName)
        {
            Name = name;
            Description = description;
            ItemLocation = itemLocation;
            ImageName = imageName;
        }
    }
}