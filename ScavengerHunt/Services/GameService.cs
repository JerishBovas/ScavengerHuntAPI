using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class GameService : IGameService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<Game> dbSet;

        public GameService(ScavengerHuntContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreatedAsync();
            dbSet = this.context.Set<Game>();
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Game?> GetAsync(Guid id)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Game entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Game entity)
        {
            dbSet.Update(entity);
        }

        public async void DeleteAsync(Guid id)
        {
            Game? entity = await GetAsync(id);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
