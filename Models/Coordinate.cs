﻿namespace ScavengerHunt_API.Models
{
    public class Coordinate
    {
        public int Id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}