﻿namespace ScavengerHunt.API.Models
{
    public record Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemLocation { get; set; }
        public string ImageName { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}