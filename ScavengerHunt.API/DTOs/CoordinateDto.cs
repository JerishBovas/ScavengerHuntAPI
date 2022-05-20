﻿namespace ScavengerHunt.API.DTOs
{
    public record struct CoordinateDto
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int LocationId { get; set; }
    }
}
