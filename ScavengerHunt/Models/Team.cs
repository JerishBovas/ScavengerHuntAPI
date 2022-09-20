// This is the team object that holds each team's information
namespace ScavengerHunt.Models
{
    public record Team
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public bool IsOpen { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string TeamIcon { get; set; } = "";
    }
}
