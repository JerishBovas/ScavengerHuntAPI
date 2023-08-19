using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGameService
    {
        Task<Game?> GetByIdAsync(string id);
        Task<Game?> GetAsync(string id, string userId);
        Task<List<Game>> GetAllAsync();
        Task CreateAsync(Game entity);
        void Update(Game entity);
        void Delete(Game entity);
        Task SaveChangesAsync();
    }
}
