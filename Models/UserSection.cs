namespace ScavengerHunt_API.Models
{
    public class UserSection
    {
        public int Id { get; set; }
        public int UserScore { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<ScoreLog> ScoreLog { get; set; }
    }
}
