
namespace ScavengerHunt.Models
{
    public record class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public UserLog UserLog { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}
