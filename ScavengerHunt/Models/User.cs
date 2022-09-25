// This model holds the users datas
// Its created alongside the account object
namespace ScavengerHunt.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public string ProfileImage { get; set; } = "";
        public int Score { get; set; }
        public List<string> Games { get; set; } = new();
        public List<string> Teams { get; set; } = new();
        public DateTimeOffset LastUpdated { get; set; }
    }
}
