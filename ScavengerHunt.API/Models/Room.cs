namespace ScavengerHunt.Models
{
    public record class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocationTitle { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
