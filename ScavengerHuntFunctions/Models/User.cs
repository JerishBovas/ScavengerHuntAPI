using System;

namespace ScavengerHuntFunctions.Models
{
    public record User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public UserLog UserLog { get; set; }
    }
}
