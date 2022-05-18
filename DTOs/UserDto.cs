using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.DTOs
{
    public record struct UserDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
    }
}
