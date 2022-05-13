namespace ScavengerHunt_API.Services
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(object id);
        Task<List<T>> GetAllAsync();
        Task CreateAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(object id);
        Task SaveChangesAsync();
    }
}
