using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface ITeamService
    {
        Task<Team?> GetAsync(Guid id, Guid adminId);
        Task<Team?> GetByIdAsync(Guid id);
        Task<List<Team>> GetAllAsync();
        Task CreateAsync(Team entity);
        void UpdateAsync(Team entity);
        void DeleteAsync(Team entity);
        Task SaveChangesAsync();
    }
}
