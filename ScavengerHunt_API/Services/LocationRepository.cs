using Microsoft.EntityFrameworkCore;
using ScavengerHunt_API.Data;
using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Services
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ScavengerHunt_APIContext context;
        private DbSet<Location> dbSet;
        public LocationRepository(ScavengerHunt_APIContext context)
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
