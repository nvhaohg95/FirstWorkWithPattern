using Application.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.UnitOfWorks
{
    public class Linq2DbUnitOfWork : IUnitOfWork
    {
        private readonly DataConnection _dataConnection;
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly IServiceProvider _serviceProvider;

        public Linq2DbUnitOfWork(DataConnection dataConnection, IServiceProvider serviceProvider)
        {
            _dataConnection = dataConnection;
            _serviceProvider = serviceProvider;
        }

        public IRepositoryBase<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var strategy = _serviceProvider.GetRequiredService<IRepositoryStrategy<T>>();
                _repositories[type] = strategy.GetRepository();
            }

            return (IRepositoryBase<T>)_repositories[type];
        }

        public Task<int> SaveChangesAsync() => Task.FromResult(0); // Linq2Db có thể auto-commit từng truy vấn

        public Task BeginTransactionAsync()
        {
            _dataConnection.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task CommitAsync()
        {
            _dataConnection.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task RollbackAsync()
        {
            _dataConnection.RollbackTransaction();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _dataConnection.Dispose();
        }
    }
}
