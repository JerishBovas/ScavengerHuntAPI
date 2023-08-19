using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGamePlayService
    {
        Task<GamePlay?> GetAsync(string id, string userId);
        Task<GamePlay?> GetByIdAsync(string id);
        Task<List<GamePlay>> GetAllAsync();
        Task CreateAsync(GamePlay entity);
        void UpdateAsync(GamePlay entity);
        void DeleteAsync(GamePlay entity);
        Task SaveChangesAsync();
    }
}
