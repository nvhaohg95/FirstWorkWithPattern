using Application.Interfaces;
using Infrastructure.Helpers;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using System.Linq.Expressions;

public class Linq2DbRepository<T> : IRepositoryBase<T> where T : class
{
    private readonly DataConnection _db;

    public Linq2DbRepository(DataConnection db)
    {
        _db = db;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _db.GetTable<T>().FirstOrDefaultAsync(e => Microsoft.EntityFrameworkCore.EF.Property<Guid>(e, "Id") == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _db.GetTable<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAsync(Func<T, bool> predicate)
    {
        return _db.GetTable<T>().Where(predicate).ToList();
    }

    public async Task<T> GetOneAsync(Func<T, bool> predicate)
    {
        return _db.GetTable<T>().FirstOrDefault(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _db.InsertAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        await _db.UpdateAsync(entity);
    }

    public async Task<int> ExecuteUpdateAsync<TUpdate>(
        Expression<Func<T, bool>> predicate,
        TUpdate updateModel) where TUpdate : class
    {
        // 1. Lọc đối tượng cần cập nhật
        var updatable = _db.GetTable<T>().Where(predicate).AsUpdatable();

        // 2. Lấy danh sách các biểu thức cập nhật
        var setters = Linq2DbExpressionHelper<T, TUpdate>.GetUpdateExpressions(updateModel);

        // 3. Áp dụng từng biểu thức Set()
        foreach (var setter in setters)
        {
            updatable = updatable.Set(setter.Key, setter.Value);
        }

        // 4. Thực hiện cập nhật
        return await updatable.UpdateAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
            await _db.DeleteAsync(entity);
    }
}
