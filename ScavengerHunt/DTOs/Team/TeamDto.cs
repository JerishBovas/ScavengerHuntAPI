namespace ScavengerHunt.DTOs
{
    public struct TeamDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string TeamIcon { get; set; }
    }
}
