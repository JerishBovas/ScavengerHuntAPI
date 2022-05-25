using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IGroupRepository
    {
        Task<Group?> GetAsync(Guid id);
        Task<List<Group>> GetAllAsync();
        Task CreateAsync(Group entity);
        void UpdateAsync(Group entity);
        void DeleteAsync(Group entity);
        Task SaveChangesAsync();
    }
}
