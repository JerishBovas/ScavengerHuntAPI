namespace ScavengerHunt.Models
{
    public record Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
