using Newtonsoft.Json;

namespace ScavengerHunt.Models
{
    // This is the account thats created when a user creates account with Scavenger Hunt
    // Its holds the crucial information needed for authentication like email, password, role etc.
    public class Account
    {
        public string Email { get; set; } = "";
        public Guid UserId { get; set; }
        public Guid? AppleId { get; set; }
        public Guid? GoogleId { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public string Roles { get; set; } = "User";
        public string? RefToken { get; set; }
        public DateTime? RefTokenExpiry { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
