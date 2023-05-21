namespace ScavengerHunt.DTOs
{
    public record struct ItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}