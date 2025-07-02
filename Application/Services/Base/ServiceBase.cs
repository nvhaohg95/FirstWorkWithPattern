using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Base
{
    public abstract class ServiceBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected IRepositoryBase<T> Repository<T>() where T : class
            => _unitOfWork.Repository<T>();

        protected Task<int> SaveChangesAsync()
            => _unitOfWork.SaveChangesAsync();
    }
}
