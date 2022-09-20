// This model holds the team and user together
// Since user and team are multiply dependant, needed to create linking table
namespace ScavengerHunt.Models
{
    public class TeamMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
    }
}
