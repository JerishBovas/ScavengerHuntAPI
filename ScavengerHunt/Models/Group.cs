namespace ScavengerHunt.Models
{
    public record Group
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; } = Guid.NewGuid();
        public bool IsOpen { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string CreatedUser { get; set; } = "";
        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<ScoreLog> PastWinners { get; set; } = new List<ScoreLog>();
    }
}
