using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGameService
    {
        Task<Game?> GetByIdAsync(Guid id);
        Task<Game?> GetAsync(Guid id, Guid userId);
        Task<List<Game>> GetAllAsync();
        Task CreateAsync(Game entity);
        void Update(Game entity);
        void Delete(Game entity);
        Task SaveChangesAsync();
    }
}
