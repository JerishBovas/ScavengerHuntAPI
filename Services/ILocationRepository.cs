using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Services
{
    public interface ILocationRepository
    {
        Task<Location?> GetAsync(int id);
        Task<List<Location>> GetAllAsync();
        Task CreateAsync(Location entity);
        void UpdateAsync(Location entity);
        void DeleteAsync(Location entity);
        Task SaveChangesAsync();
    }
}
