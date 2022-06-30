using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGroupService
    {
        Task<Group?> GetAsync(Guid id);
        Task<List<Group>> GetAllAsync();
        Task CreateAsync(Group entity);
        void UpdateAsync(Group entity);
        void DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
