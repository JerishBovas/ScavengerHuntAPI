using ScavengerHunt.Models;

namespace ScavengerHunt.DTOs
{
    public record struct GroupSendDto
    {
        public Guid Id { get; init; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string CreatedUser { get; init; }
        public ICollection<UserDto>? Members { get; init; }
        public ICollection<ScoreLog>? PastWinners { get; init; }
    }
}
