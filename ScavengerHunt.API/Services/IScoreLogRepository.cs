using System;
using ScavengerHunt.API.Models;

namespace ScavengerHunt.API.Services
{
	public interface IScoreLogRepository
	{
		Task<List<ScoreLog>> GetScoreLogsByUser(User user);
		Task<List<ScoreLog>> GetScoreLogsByGroup(Group group);
	}
}

