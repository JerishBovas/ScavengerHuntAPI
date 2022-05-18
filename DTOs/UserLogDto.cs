namespace ScavengerHunt_API.DTOs;
public record struct UserLogDto
{
    public int Id { get; init; }
    public string UserEmail { get; init; }
    public int UserScore { get; init; }
    public DateTime LastUpdated { get; init; }
    public ICollection<ScoreLogDto> ScoreLog { get; init; }
    public int MyLocations { get; init; }
    public int MyGroups { get; init; }
}
