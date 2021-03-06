using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface ITeamService
    {
        Task<Team?> GetAsync(Guid id, Guid teamId);
        Task<Team?> GetByIdAsync(Guid id);
        Task<List<Team>> GetAllAsync();
        Task CreateAsync(Team entity);
        void UpdateAsync(Team entity);
        void DeleteAsync(Guid id, Guid teamId);
        Task SaveChangesAsync();
    }
}
