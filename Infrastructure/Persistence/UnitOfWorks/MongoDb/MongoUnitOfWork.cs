using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Persistence.UnitOfWorks
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly IServiceProvider _serviceProvider;

        public MongoUnitOfWork(IMongoDatabase database, IServiceProvider serviceProvider)
        {
            _database = database;
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

        public Task<int> SaveChangesAsync()
        {
            // MongoDB auto-save per operation
            return Task.FromResult(0);
        }

        public Task BeginTransactionAsync()
        {
            // Optional: Use session if needed
            return Task.CompletedTask;
        }

        public Task CommitAsync() => Task.CompletedTask;

        public Task RollbackAsync() => Task.CompletedTask;

        public void Dispose()
        {
            // No explicit dispose needed for IMongoDatabase
        }
    }
}
