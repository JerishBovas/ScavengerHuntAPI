using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface ITeamService
    {
        Task<Team?> GetAsync(string id, string adminId);
        Task<Team?> GetByIdAsync(string id);
        Task<List<Team>> GetAllAsync();
        Task CreateAsync(Team entity);
        void UpdateAsync(Team entity);
        void DeleteAsync(Team entity);
        Task SaveChangesAsync();
    }
}
