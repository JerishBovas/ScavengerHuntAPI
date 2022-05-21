namespace ScavengerHunt.DTOs
{
    public record struct GroupDto
    {
        public int Id { get; set; }
        public Guid UniqueId { get; init; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string CreatedUser { get; set; }
    }
}
