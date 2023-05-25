namespace ScavengerHunt.DTOs
{
    public record struct GameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public Guid UserId { get; set; }
        public string ImageName { get; set; }
    }
}
