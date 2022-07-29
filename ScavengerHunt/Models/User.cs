namespace ScavengerHunt.Models
{
    public record User
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string ProfileImage { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string PasswordSalt { get; set; } = "";
        public string? RefToken { get; set; }
        public DateTime? RefTokenExpiry { get; set; }
        public UserLog UserLog { get; set; } = new UserLog();
        public ICollection<Guid> Games { get; set; } = new List<Guid>();
        public ICollection<Guid> Teams { get; set; } = new List<Guid>();
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
