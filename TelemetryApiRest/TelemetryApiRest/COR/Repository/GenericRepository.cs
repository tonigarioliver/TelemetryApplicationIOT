using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using TelemetryApiRest.COR.IRepository;
using TelemetryApiRest.Data;

namespace TelemetryApiRest.COR.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected TelemetryApiDbContext _apiDbContext;
        protected DbSet<T> _dbSet;
        private ILogger<IGenericRepository<T>> logger;
        public GenericRepository(TelemetryApiDbContext dbContext, ILogger<IGenericRepository<T>> logger)
        {
            this._apiDbContext = dbContext;
            this._dbSet = dbContext.Set<T>();
            this.logger = logger;
        }
        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return entity;
            }
            catch (Exception e)
            {
                logger.LogError("Error creating in GenericRepository", e);
                return null;
            }
        }

        public async Task<bool> Delete(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                return true;
            }
            catch (Exception e)
            {
                logger.LogError("Error deleting in GenericRepository", e);
                return false;
            }
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                return entity;
            }
            catch (Exception e)
            {
                logger.LogError("Error updating in GenericRepository", e);
                return null;
            }
        }
    }
}
