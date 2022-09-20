using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IAccountService
    {
        Task<Account?> GetAsync(Guid id);
        Task<List<Account>> GetAllAsync();
        Task CreateAsync(Account entity);
        void UpdateAsync(Account entity);
        void DeleteAsync(Account entity);
        Task SaveChangesAsync();
    }
}
