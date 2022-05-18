namespace ScavengerHunt.Models
{
    public record class Group
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public bool IsOpen { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedUser { get; set; }
        public ICollection<User> Members { get; set; }
        public ICollection<ScoreLog> PastWinners { get; set; }
    }
}
