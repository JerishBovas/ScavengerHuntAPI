
namespace ScavengerHunt.Models
{
    public record User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public UserLog UserLog { get; set; } = new UserLog();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
