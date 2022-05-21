namespace ScavengerHunt.DTOs
{
    public record struct UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserLogDto UserLog { get; set; }
    }
}
