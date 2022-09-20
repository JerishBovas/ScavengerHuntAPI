using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScavengerHunt.Models;
using Newtonsoft.Json;

namespace ScavengerHunt.Data
{
    public class ScavengerHuntContext : DbContext
    {
        public ScavengerHuntContext (DbContextOptions<ScavengerHuntContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>()
                .HasNoDiscriminator()
                .ToContainer("Accounts")
                .HasPartitionKey(nameof(Account.Id));

            builder.Entity<User>()
                .HasNoDiscriminator()
                .ToContainer("Users")
                .HasPartitionKey(nameof(User.Id));

            builder.Entity<Game>()
                .HasNoDiscriminator()
                .ToContainer(nameof(Games))
                .HasPartitionKey(nameof(Game.Id));

            builder.Entity<Item>()
                .HasNoDiscriminator()
                .ToContainer("Items")
                .HasPartitionKey(nameof(Item.GameId));

            builder.Entity<GamePlay>()
                .HasNoDiscriminator()
                .ToContainer("GamePlays")
                .HasPartitionKey(nameof(GamePlay.UserId));

            builder.Entity<Team>()
                .HasNoDiscriminator()
                .ToContainer(nameof(Teams))
                .HasPartitionKey(nameof(Team.UserId));

            builder.Entity<TeamMember>()
                .HasNoDiscriminator()
                .ToContainer("TeamUsers")
                .HasNoKey();
        }

        public DbSet<Account>? Accounts { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<Game>? Games { get; set; }
        public DbSet<Item>? Items { get; set; }
        public DbSet<GamePlay>? GamePlays { get; set; }
        public DbSet<Team>? Teams { get; set; }
        public DbSet<TeamMember>? TeamUsers { get; set; }
    }
}
