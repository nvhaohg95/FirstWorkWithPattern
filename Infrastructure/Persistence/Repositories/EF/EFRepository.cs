using Application.Interfaces;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class EFRepository<T> : IRepositoryBase<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> GetAsync(Func<T, bool> predicate)
        {
            return await Task.Run(() => _dbSet.AsEnumerable().Where(predicate));
        }

        public async Task<T> GetOneAsync(Func<T, bool> predicate)
        {
            return await Task.Run(() => _dbSet.AsEnumerable().FirstOrDefault(predicate));
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<int> ExecuteUpdateAsync<TUpdate>(Expression<Func<T, bool>> predicate, TUpdate updateModel) where TUpdate : class
        {
            var updateExpr = EFExpressionHelper<T, TUpdate>.GetUpdateExpression(updateModel);
            return await _dbSet.Where(predicate).ExecuteUpdateAsync(updateExpr);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

    }
}