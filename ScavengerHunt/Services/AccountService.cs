using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class AccountService : IAccountService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<Account> dbSet;

        public AccountService(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Account>();
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Account?> GetAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task CreateAsync(Account entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Account entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(Account entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
