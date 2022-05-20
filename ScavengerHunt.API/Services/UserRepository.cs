using Microsoft.EntityFrameworkCore;
using ScavengerHunt.API.Data;
using ScavengerHunt.API.Models;

namespace ScavengerHunt.API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<User> dbSet;

        public UserRepository(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<User>();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<User?> GetAsync(string email)
        {
            return await dbSet
                .Where(s => s.Email == email)
                .Include(u => u.UserLog)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(User entity)
        {
            dbSet.Update(entity);
        }

        public async void DeleteAsync(string email)
        {
            User? entity = await GetAsync(email);
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
