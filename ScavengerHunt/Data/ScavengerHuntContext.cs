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
            var userModel = builder.Entity<User>();
            
            userModel.ToContainer(nameof(Users))
                .HasPartitionKey(nameof(User.id))
                .OwnsOne(u => u.UserLog)
                .OwnsMany(u => u.ScoreLog);

            userModel.Property(u => u.Games)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<Guid>>(v) ?? new List<Guid>(){Guid.Empty}));

            userModel.Property(u => u.Teams)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<Guid>>(v) ?? new List<Guid>(){Guid.Empty}));

            builder.Entity<Game>()
                .ToContainer(nameof(Games))
                .HasPartitionKey(nameof(Game.UserId))
                .OwnsMany(r => r.Items);

            builder.Entity<Team>()
                .ToContainer(nameof(Teams))
                .HasPartitionKey(nameof(Team.CreatedUserId))
                .OwnsMany(g => g.PastWinners);

            builder.Entity<Team>().Property(u => u.Members)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<Guid>>(v) ?? new List<Guid>(){Guid.Empty}));
        }

        public DbSet<Game>? Games { get; set; }
        public DbSet<Team>? Teams { get; set; }
        public DbSet<User>? Users { get; set; }
    }
}
