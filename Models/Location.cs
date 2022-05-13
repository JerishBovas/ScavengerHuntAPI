namespace ScavengerHunt_API.Models
{
    public class Location
    {
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public virtual Coordinate Coordinate { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public string ImageName { get; set; }
        public Difficult Difficulty { get; set; }
        public int Ratings { get; set; }
        public String Tags { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
