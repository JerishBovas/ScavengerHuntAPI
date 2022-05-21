using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
	public class ScoreLogRepository : IScoreLogRepository
	{
		private readonly ScavengerHuntContext context;
		private readonly DbSet<ScoreLog> dbSet;

		public ScoreLogRepository(ScavengerHuntContext context)
		{
			this.context = context;
			dbSet = this.context.Set<ScoreLog>();
		}

        public async Task<List<ScoreLog>> GetScoreLogsByGroup(Group group)
        {
            return await dbSet
                .Include(u => u.Group)
                .Where(u => u.Group.Id == group.Id)
                .ToListAsync();
        }

        public async Task<List<ScoreLog>> GetScoreLogsByUser(User user)
        {
            return await dbSet
                .Include(u => u.UserLog)
                .Where(u => u.UserLog.UserId == user.Id)
                .ToListAsync();
        }
    }
}

