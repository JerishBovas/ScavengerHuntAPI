using Newtonsoft.Json;

namespace ScavengerHunt.Models
{
    // This is the account thats created when a user creates account with Scavenger Hunt
    // Its holds the crucial information needed for authentication like email, password, role etc.
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = "";
        public string Roles { get; set; } = "User";
        public string PasswordHash { get; set; } = "";
        public string PasswordSalt { get; set; } = ""; 
        public string? RefToken { get; set; }
        public DateTime? RefTokenExpiry { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;
    }
}
