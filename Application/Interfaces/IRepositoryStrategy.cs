using System;

namespace Application.Interfaces
{
    public interface IRepositoryStrategy<T> where T : class
    {
        IRepositoryBase<T> GetRepository();
    }
} 