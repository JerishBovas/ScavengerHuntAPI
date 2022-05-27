namespace ScavengerHunt.Services
{
    public interface IRepositoryService<T> where T : class
    {
        Task<T?> GetAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task CreateAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
