using Application.Interfaces;
using System.Data;

namespace Infrastructure.Persistence.Repositories
{
    public class DapperRepository<T> : IRepositoryBase<T> where T : class
    {
        protected readonly IDbConnection _connection;
        public DapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            // TODO: Implement query by id
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // TODO: Implement get all
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Func<T, bool> predicate)
        {
            var all = await GetAllAsync();
            return all.Where(predicate);
        }

        public virtual async Task<T> GetOneAsync(Func<T, bool> predicate)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            // TODO: Implement insert
            throw new NotImplementedException();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            // TODO: Implement update
            throw new NotImplementedException();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            // TODO: Implement delete by id
            throw new NotImplementedException();
        }
    }
} 