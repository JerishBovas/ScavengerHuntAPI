namespace ScavengerHunt.Models
{
    public record Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }

        public Item(string name, string description, string imageName)
        {
            Name = name;
            Description = description;
            ImageName = imageName;
        }
    }
}