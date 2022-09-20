using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGamePlayService
    {
        Task<GamePlay?> GetAsync(Guid id, Guid gamePlayId);
        Task<GamePlay?> GetByIdAsync(Guid id);
        Task<List<GamePlay>> GetAllAsync();
        Task CreateAsync(GamePlay entity);
        void UpdateAsync(GamePlay entity);
        void DeleteAsync(GamePlay entity);
        Task SaveChangesAsync();
    }
}
