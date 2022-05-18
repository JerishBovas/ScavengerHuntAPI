using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Services
{
    public interface IGroupRepository
    {
        Task<Group?> GetAsync(int id);
        Task<List<Group>> GetAllAsync();
        Task CreateAsync(Group entity);
        void UpdateAsync(Group entity);
        void DeleteAsync(Group entity);
        Task SaveChangesAsync();
    }
}
