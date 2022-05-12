
namespace ScavengerHunt_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public UserSection UserSection { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}
