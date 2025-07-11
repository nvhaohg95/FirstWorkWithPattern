using System;

namespace Application.Interfaces.Interfaces
{
    public interface IRepositoryStrategy<T> where T : class
    {
        IRepositoryBase<T> GetRepository();
    }
} 