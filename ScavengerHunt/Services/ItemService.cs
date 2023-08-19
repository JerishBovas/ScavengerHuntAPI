using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class ItemService : IItemService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<Item> dbSet;

        public ItemService(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Item>();
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Item?> GetAsync(string gameId, string itemId)
        {
            return await dbSet.FindAsync(gameId, itemId);
        }

        public async Task<Item?> GetByIdAsync(string id)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Item entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Item entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(Item entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
