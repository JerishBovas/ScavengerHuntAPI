// This model holds the users datas
// Its created alongside the account object
namespace ScavengerHunt.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string ProfileImage { get; set; } = "";
        public int Score { get; set; }
        public int Games { get; set; }
        public int Teams { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
