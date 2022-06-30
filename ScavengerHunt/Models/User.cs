namespace ScavengerHunt.Models
{
    public record User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ProfileImage { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string? RefToken { get; set; }
        public DateTime? RefTokenExpiry { get; set; }
        public UserLog UserLog { get; set; }
        public ICollection<Guid> Games { get; set; }
        public ICollection<Guid> Groups { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
            Name = "";
            Email = "";
            Role = "";
            ProfileImage = "";
            PasswordHash = "";
            PasswordSalt = "";
            RefToken = null;
            RefTokenExpiry = null;
            UserLog = new();
            Games = new HashSet<Guid>();
            Groups = new HashSet<Guid>();
            CreatedDate = DateTimeOffset.UtcNow;
        }

        public User(string name, string email, string passwordHash, string passwordSalt)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Role = "user";
            ProfileImage = "";
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            UserLog = new();
            Games = new HashSet<Guid>();
            Groups = new HashSet<Guid>();
            CreatedDate = DateTimeOffset.UtcNow;
        }

        public User(Guid id, string name, string email, string passwordHash, string passwordSalt, UserLog userLog, ICollection<Guid> games, ICollection<Guid> groups, DateTimeOffset createdDate) : this(name, email, passwordHash, passwordSalt)
        {
            Id = id;
            UserLog = userLog;
            Games = games;
            Groups = groups;
            CreatedDate = createdDate;
        }
    }
}
