using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Models;

namespace ScavengerHunt.Data
{
    public class ScavengerHuntContext : DbContext
    {
        public ScavengerHuntContext (DbContextOptions<ScavengerHuntContext> options)
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
