﻿namespace ScavengerHunt.Models
{
    public record Location
    {
        public Guid Id { get; set; }
        public bool IsPrivate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public Guid UserId { get; set; }
        public Coordinate Coordinate { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public string ImageName { get; set; }
        public int Difficulty { get; set; }
        public List<int> Ratings { get; set; }
        public List<string> Tags { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public Location()
        {
            Id = Guid.NewGuid();
            Name = "";
            Description = "";
            Address = "";
            Country = "";
            Coordinate = new Coordinate();
            Rooms = new List<Room>();
            ImageName = "";
            Difficulty = 0;
            Ratings = new List<int>();
            Tags = new List<string>();
            CreatedDate = DateTimeOffset.UtcNow;
            LastUpdated = DateTimeOffset.UtcNow;
        }
        
        public Location(bool isPrivate, string name, string description, string address, string country, Guid userId, Coordinate coordinate,  string imageName, int difficulty, List<string> tags)
        {
            Id = Guid.NewGuid();
            IsPrivate = isPrivate;
            Name = name;
            Description = description;
            Address = address;
            Country = country;
            UserId = userId;
            Coordinate = coordinate;
            Rooms = new List<Room>();
            ImageName = imageName;
            Difficulty = difficulty;
            Ratings = new List<int>();
            Tags = tags;
            CreatedDate = DateTimeOffset.UtcNow;
            LastUpdated = DateTimeOffset.UtcNow;
        }
    }
}
