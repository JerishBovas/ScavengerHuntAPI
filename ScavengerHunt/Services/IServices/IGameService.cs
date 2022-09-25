using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGameService
    {
        Task<Game?> GetAsync(Guid id, Guid userId);
        Task<List<Game>> GetAllAsync();
        Task CreateAsync(Game entity);
        void UpdateAsync(Game entity);
        void DeleteAsync(Game entity);
        Task SaveChangesAsync();
    }
}
