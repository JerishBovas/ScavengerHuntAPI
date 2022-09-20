using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGameService
    {
        Task<Game?> GetAsync(Guid id, Guid gameId);
        Task<Game?> GetByIdAsync(Guid id);
        Task<List<Game>> GetAllAsync();
        Task CreateAsync(Game entity);
        void UpdateAsync(Game entity);
        void DeleteAsync(Game entity);
        Task SaveChangesAsync();
    }
}
