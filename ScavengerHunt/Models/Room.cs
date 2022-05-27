namespace ScavengerHunt.Models
{
    public record Room
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public ICollection<Item> Items { get; set; }

        public Room(string name, string details)
        {
            Name = name;
            Details = details;
            Items = new List<Item>();
        }

        public Room(string name, string details, ICollection<Item> items) : this(name, details)
        {
            Items = items;
        }
    }
}
