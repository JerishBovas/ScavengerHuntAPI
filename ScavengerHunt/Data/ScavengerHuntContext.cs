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
                .HasPartitionKey(nameof(User.Id))
                .OwnsOne(u => u.UserLog)
                .OwnsMany(u => u.ScoreLog);

            userModel.Property(u => u.Locations)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<Guid>>(v) ?? new List<Guid>(){Guid.Empty}));

            userModel.Property(u => u.Groups)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<Guid>>(v) ?? new List<Guid>(){Guid.Empty}));

            builder.Entity<Location>()
                .ToContainer(nameof(Locations))
                .HasPartitionKey(nameof(Location.UserId))
                .OwnsMany(l => l.Rooms)
                .OwnsMany(r => r.Items);

            builder.Entity<Group>()
                .ToContainer(nameof(Groups))
                .HasPartitionKey(nameof(Group.CreatedUserId))
                .OwnsMany(g => g.PastWinners);

            builder.Entity<Group>().Property(u => u.Members)
                .HasConversion(new ValueConverter<ICollection<Guid>, string>(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<Guid>>(v) ?? new List<Guid>(){Guid.Empty}));
        }

        public DbSet<Location>? Locations { get; set; }
        public DbSet<Group>? Groups { get; set; }
        public DbSet<User>? Users { get; set; }
    }
}
