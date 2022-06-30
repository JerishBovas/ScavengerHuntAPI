using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IUserService
    {
        Task<User?> GetAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task CreateAsync(User entity);
        void UpdateAsync(User entity);
        void DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
