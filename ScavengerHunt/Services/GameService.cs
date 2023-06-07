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
            dbSet = this.context.Set<Game>();
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<Game?> GetAsync(Guid id, Guid gameId)
        {
            return await dbSet.FindAsync(id, gameId);
        }

        public async Task CreateAsync(Game entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Update(Game entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(Game entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
