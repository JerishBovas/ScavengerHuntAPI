using System;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
	public interface IScoreLogRepository
	{
		Task<List<ScoreLog>> GetScoreLogsByUser(User user);
		Task<List<ScoreLog>> GetScoreLogsByGroup(Group group);
	}
}

