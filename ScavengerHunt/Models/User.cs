namespace ScavengerHunt.Models
{
    public record User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public UserLog UserLog { get; set; } = new UserLog();
        public ICollection<Guid> Locations { get; set; } = new HashSet<Guid>();
        public ICollection<Guid> Groups { get; set; } = new HashSet<Guid>();
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
