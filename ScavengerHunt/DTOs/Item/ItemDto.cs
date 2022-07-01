namespace ScavengerHunt.DTOs
{
    public record struct ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
    }
}