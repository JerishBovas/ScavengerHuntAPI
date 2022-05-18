namespace ScavengerHunt.DTOs
{
    public record struct GroupShortDto
    {
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
    }
}
