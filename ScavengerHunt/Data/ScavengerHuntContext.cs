﻿using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Models;

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
            builder.Entity<User>()
                .HasNoDiscriminator()
                .ToContainer("Users")
                .HasPartitionKey(nameof(User.Id))
                .HasKey(u => u.Id);

            builder.Entity<Game>()
                .ToContainer(nameof(Games))
                .HasPartitionKey(nameof(Game.UserId))
                .HasKey(da => new{da.Id, da.UserId});

            builder.Entity<GamePlay>()
                .HasNoDiscriminator()
                .ToContainer("GamePlays")
                .HasPartitionKey(nameof(GamePlay.UserId))
                .HasKey(da => new{da.Id, da.UserId});

            builder.Entity<Team>()
                .HasNoDiscriminator()
                .ToContainer(nameof(Teams))
                .HasPartitionKey(nameof(Team.AdminId))
                .HasKey(da => new{da.Id, da.AdminId});
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Game>? Games { get; set; }
        public DbSet<GamePlay>? GamePlays { get; set; }
        public DbSet<Team>? Teams { get; set; }
    }
}
