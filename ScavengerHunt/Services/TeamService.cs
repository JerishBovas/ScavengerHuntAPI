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
            this.context.Database.EnsureCreatedAsync();
            dbSet = this.context.Set<Team>();
        }

        public async Task<List<Team>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Team?> GetAsync(Guid id, Guid teamId)
        {
            return await dbSet.FindAsync(id, teamId);
        }

        public async Task<Team?> GetByIdAsync(Guid id)
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

        public async void DeleteAsync(Guid id, Guid teamId)
        {
            Team? entity = await GetAsync(id, teamId);
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
