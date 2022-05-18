using ScavengerHunt.Models;

namespace ScavengerHunt.DTOs
{
    public record struct UserDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
    }
}
