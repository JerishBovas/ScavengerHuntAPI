using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface ILocationRepository
    {
        Task<Location?> GetAsync(Guid id);
        Task<List<Location>> GetAllAsync();
        Task CreateAsync(Location entity);
        void UpdateAsync(Location entity);
        void DeleteAsync(Location entity);
        Task SaveChangesAsync();
    }
}
