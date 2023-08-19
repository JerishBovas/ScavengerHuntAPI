// This is the team object that holds each team's information
namespace ScavengerHunt.Models
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AdminId { get; set; } = Guid.NewGuid().ToString();
        public bool IsOpen { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Members { get; set; } = new();
        public string TeamIcon { get; set; } = "";
    }
}
