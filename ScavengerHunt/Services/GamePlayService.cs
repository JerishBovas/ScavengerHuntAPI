using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class GamePlayService : IGamePlayService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<GamePlay> dbSet;

        public GamePlayService(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<GamePlay>();
        }

        public async Task<List<GamePlay>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<GamePlay?> GetAsync(string id, string userId)
        {
            return await dbSet.FindAsync(id, userId);
        }

        public async Task<GamePlay?> GetByIdAsync(string id)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(GamePlay entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(GamePlay entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(GamePlay entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
