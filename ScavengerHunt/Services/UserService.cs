using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class UserService : IUserService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<User> dbSet;

        public UserService(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<User>();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<User?> GetAsync(string id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task CreateAsync(User entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(User entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(User entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
