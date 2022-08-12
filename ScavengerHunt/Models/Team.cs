using System.Text.Json.Serialization;

namespace ScavengerHunt.Models
{
    public record Team
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public bool IsOpen { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string TeamIcon { get; set; } = "";
        public Guid CreatedUserId { get; set; }
        public ICollection<Guid> Members { get; set; } = new List<Guid>();
        public ICollection<GameScore> PastWinners { get; set; } = new List<GameScore>();
    }
}
