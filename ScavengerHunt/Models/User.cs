// This model holds the users datas
// Its created alongside the account object
namespace ScavengerHunt.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public int Score { get; set; }
        public List<string> Games { get; set; }
        public List<string> Teams { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public User(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            Email = "";
            ProfileImage = "";
            Score = 0;
            Games = new();
            Teams = new();
            LastUpdated = DateTimeOffset.UtcNow;
        }
    }
}
