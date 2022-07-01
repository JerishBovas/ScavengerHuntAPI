namespace ScavengerHunt.DTOs
{
    public record struct UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public UserLogDto UserLog { get; set; }
    }
}
