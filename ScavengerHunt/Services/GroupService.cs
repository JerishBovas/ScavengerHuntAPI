using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class GroupService : IGroupService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<Group> dbSet;

        public GroupService(ScavengerHuntContext context)
        {
            this.context = context;
            this.context.Database.EnsureCreatedAsync();
            dbSet = this.context.Set<Group>();
        }

        public async Task<List<Group>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Group?> GetAsync(Guid id)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Group entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Group entity)
        {
            dbSet.Update(entity);
        }

        public async void DeleteAsync(Guid id)
        {
            Group? entity = await GetAsync(id);
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
