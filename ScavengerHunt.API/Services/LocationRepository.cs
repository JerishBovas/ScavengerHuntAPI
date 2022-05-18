using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ScavengerHuntContext context;
        private DbSet<Location> dbSet;
        public LocationRepository(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Location>();
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await dbSet
                .Include(c => c.Coordinate)
                .ToListAsync();
        }

        public async Task<Location?> GetAsync(int id)
        {
            Location? loc = await dbSet
                .Where(p => p.Id == id)
                .Include(x => x.Coordinate)
                .Include(x => x.Rooms)
                .FirstOrDefaultAsync();
            return loc;
        }

        public async Task CreateAsync(Location entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Location entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(Location entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
