using System;
namespace ScavengerHunt.DTOs
{
	public record struct GroupDetailDto
	{
        public Guid Id { get; set; }
        public bool IsOpen { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string GroupIcon { get; set; }
        public ICollection<Guid> Members { get; init; }
        public ICollection<ScoreLogDto> PastWinners { get; init; }
    }
}

