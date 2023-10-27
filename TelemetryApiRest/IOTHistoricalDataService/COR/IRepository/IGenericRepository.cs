using System.Linq.Expressions;

namespace IOTHistoricalDataService.COR.IRepository
{
    public interface IGenericRepository<T>where T : class
    {
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);

        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);
        Task<bool>Delete(T entity);
        Task<T> Update(T entity);
        Task<T> CreateAsync(T entity);

    }
}
