using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IUserRepository
    {
        Task<User?> GetAsync(string email);
        Task<List<User>> GetAllAsync();
        Task CreateAsync(User entity);
        void UpdateAsync(User entity);
        void DeleteAsync(string email);
        Task SaveChangesAsync();
    }
}
