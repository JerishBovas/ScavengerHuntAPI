using System;
namespace ScavengerHunt.DTOs
{
	public record struct TeamDetailDto
	{
        public Guid Id { get; set; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string TeamIcon { get; set; }
        public ICollection<Guid> Members { get; init; }
        public ICollection<GameScoreDto> PastWinners { get; init; }
    }
}

