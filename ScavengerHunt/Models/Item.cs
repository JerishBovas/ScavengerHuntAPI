namespace ScavengerHunt.Models
{
    public record Item
    {
        public Guid id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageName { get; set; } = "";
    }
}