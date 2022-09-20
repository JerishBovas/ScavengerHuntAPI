using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface ITeamMemberService
    {
        Task<List<TeamMember>> GetByTeamIdAsync(Guid id);
        Task<List<TeamMember>> GetByUserIdAsync(Guid id);
        Task CreateAsync(TeamMember entity);
        void DeleteAsync(TeamMember entity);
        Task SaveChangesAsync();
    }
}
