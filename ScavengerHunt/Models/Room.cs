namespace ScavengerHunt.Models
{
    public record Room
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
