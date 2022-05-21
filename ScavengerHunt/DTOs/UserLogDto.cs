namespace ScavengerHunt.DTOs;
public record struct UserLogDto
{
    public int UserScore { get; init; }
    public DateTimeOffset LastUpdated { get; init; }
}
