using Microsoft.EntityFrameworkCore;
using ScavengerHunt_API.Data;
using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Services
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ScavengerHunt_APIContext context;
        private DbSet<Group> dbSet;
        public GroupRepository(ScavengerHunt_APIContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Group>();
        }

        public async Task<List<Group>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Group?> GetAsync(int id)
        {
            Group? group = await dbSet
                .Where(p => p.Id == id)
                .Include(x => x.Members)
                .Include(x => x.PastWinners)
                .FirstOrDefaultAsync();
            return group;
        }

        public async Task CreateAsync(Group entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Group entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(Group entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
