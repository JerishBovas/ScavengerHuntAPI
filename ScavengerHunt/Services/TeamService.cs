using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class TeamService : ITeamService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<Team> dbSet;

        public TeamService(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Team>();
        }

        public async Task<List<Team>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Team?> GetAsync(string id, string adminId)
        {
            return await dbSet.FindAsync(id, adminId);
        }

        public async Task<Team?> GetByIdAsync(string id)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Team entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Team entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(Team entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
