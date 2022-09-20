using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<TeamMember> dbSet;

        public TeamMemberService(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<TeamMember>();
        }

        public async Task<List<TeamMember>> GetByUserIdAsync(Guid id)
        {
            List<TeamMember> teamUsers = await dbSet.ToListAsync();
            return teamUsers.Where(x => x.UserId == id).ToList();
        }

        public async Task<List<TeamMember>> GetByTeamIdAsync(Guid id)
        {
            List<TeamMember> teamUsers = await dbSet.ToListAsync();
            return teamUsers.Where(x => x.TeamId == id).ToList();
        }

        public async Task CreateAsync(TeamMember entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void DeleteAsync(TeamMember entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
