namespace ScavengerHunt.DTOs
{
    public record struct TeamDto
    {
        public Guid Id { get; set; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string TeamIcon { get; set; }
    }
}
