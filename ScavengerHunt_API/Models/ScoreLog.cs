namespace ScavengerHunt_API.Models
{
    public record class ScoreLog
    {
        public int Id { get; set; }
        public DateTime DatePlayed { get; set; }
        public string LocationName { get; set; }
        public int Score { get; set; }
        public UserLog? UserLog { get; set; }
        public Group? Group { get; set; }
    }
}