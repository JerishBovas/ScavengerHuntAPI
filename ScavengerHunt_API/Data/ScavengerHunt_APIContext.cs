using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Data
{
    public class ScavengerHunt_APIContext : DbContext
    {
        public ScavengerHunt_APIContext (DbContextOptions<ScavengerHunt_APIContext> options)
            : base(options)
        {
        }

        public DbSet<Location>? Locations { get; set; }
        public DbSet<Group>? Groups { get; set; }
        public DbSet<Coordinate>? Coordinates { get; set; }
        public DbSet<Item>? Items { get; set; }
        public DbSet<Room>? Rooms { get; set; }
        public DbSet<ScoreLog>? ScoreLogs { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<UserLog>? UserLogs { get; set; }
    }
}
