using ScavengerHunt_API.Models;

namespace ScavengerHunt_API.Services
{
    public interface IUserRepository
    {
        Task<User?> GetAsync(int id);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<UserLog?> GetUserLogAsync(string email);
        Task<IEnumerable<Location>> GetLocationsAsync(string email);
        Task<IEnumerable<Group>> GetGroupsAsync(string email);
        Task<string> CheckEmailAsync(string email);
        Task CreateAsync(User entity);
        void UpdateAsync(User entity);
        void DeleteAsync(string email);
        Task SaveChangesAsync();
    }
}
