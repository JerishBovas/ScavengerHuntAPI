﻿namespace ScavengerHunt_API.Models
{
    public record struct CoordinateDto
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
