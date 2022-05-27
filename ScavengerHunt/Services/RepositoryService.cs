using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;

namespace ScavengerHunt.Services
{
    public class RepositoryService<T> : IRepositoryService<T> where T : class
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<T> dbSet;

        public RepositoryService(ScavengerHuntContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreatedAsync();
            dbSet = this.context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T?> GetAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(T entity)
        {
            dbSet.Update(entity);
        }

        public async void DeleteAsync(Guid id)
        {
            T? entity = await GetAsync(id);
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
