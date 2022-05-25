namespace ScavengerHunt.DTOs
{
    public record struct GroupDto
    {
        public Guid Id { get; set; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
    }
}
