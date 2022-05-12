namespace ScavengerHunt_API.Models
{
    public class Group
    {
        public int Id { get; set; }
        public Guid uniqueId { get; set; }
        public bool IsOpen { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<Location> Locations { get; set; }
        public string CreatedUser { get; set; }
        public int NoOfUsers {
            get
            {
                return this.Members.Count;
            }
        }
        public ICollection<User> Members { get; set; }
        public ICollection<ScoreLog> PastWinners { get; set; }
    }
}
