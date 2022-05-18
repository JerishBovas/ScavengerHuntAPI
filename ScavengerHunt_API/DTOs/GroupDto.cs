namespace ScavengerHunt_API.DTOs
{
    public record struct GroupDto
    {
        public int Id { get; init; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string CreatedUser { get; set; }
        public ICollection<UserDto> Members { get; init; }
        public ICollection<ScoreLogDto> PastWinners { get; init; }
    }
}
