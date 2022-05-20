namespace ScavengerHunt.API.DTOs;
public record struct UserLogDto
{
    public int Id { get; init; }
    public string UserEmail { get; init; }
    public int UserScore { get; init; }
    public DateTimeOffset LastUpdated { get; init; }
}
