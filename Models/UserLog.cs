using System.ComponentModel.DataAnnotations.Schema;

namespace ScavengerHunt_API.Models
{
    public class UserLog
    {
        public int Id { get; set; }
        public int UserScore { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public ICollection<ScoreLog> ScoreLog { get; set; } = new List<ScoreLog>();
    }
}
