﻿namespace ScavengerHunt_API.Models
{
    public class ScoreLog
    {
        public int Id { get; set; }
        public DateTime DatePlayed { get; set; }
        public string LocationName { get; set; }
        public int Score { get; set; }
        public UserSection? UserSection { get; set; }
        public Group? Group { get; set; }
    }
}