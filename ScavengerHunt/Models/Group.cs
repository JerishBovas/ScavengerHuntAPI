namespace ScavengerHunt.Models
{
    public record Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsOpen { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public Guid CreatedUserId { get; set; }
        public ICollection<Guid> Members { get; set; } = new List<Guid>();
        public ICollection<ScoreLog> PastWinners { get; set; } = new List<ScoreLog>();
    }
}
