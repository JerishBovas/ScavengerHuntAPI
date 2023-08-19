using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IItemService
    {
        Task<Item?> GetAsync(string gameId, string itemId);
        Task<List<Item>> GetAllAsync();
        Task CreateAsync(Item entity);
        void UpdateAsync(Item entity);
        void DeleteAsync(Item entity);
        Task SaveChangesAsync();
    }
}
