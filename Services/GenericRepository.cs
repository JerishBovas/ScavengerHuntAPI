using Microsoft.EntityFrameworkCore;
using ScavengerHunt_API.Data;
using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ScavengerHunt_APIContext context;
        private DbSet<T> dbSet;
        public GenericRepository(ScavengerHunt_APIContext context)
        {
            this.context = context;
            dbSet = this.context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetAsync(object id)
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

        public void DeleteAsync(object id)
        {
            T entity = dbSet.Find(id);
            if(entity != null)
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
