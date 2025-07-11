using Application.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories
{
    public class MongoRepository<T> : IRepositoryBase<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
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
            await _collection.InsertOneAsync(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) throw new Exception("No Id property");
            var id = (Guid)idProp.GetValue(entity);
            var filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
} 