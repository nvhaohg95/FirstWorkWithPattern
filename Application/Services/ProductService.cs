
using Application.Base;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class ProductService : ServiceBase
    {
        public ProductService(IUnitOfWorkStrategy strategy)
        {
            _unitOfWork = strategy.GetUnitOfWork();
        }

        public async Task<Products> Add(Products product)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GetRepository<Products>().AddAsync(product);
                await _unitOfWork.CommitAsync();
                return product;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int>Update(Guid id)
        {
            return await _unitOfWork.GetRepository<Products>().ExecuteUpdateAsync(x=>x.Id == id, new
            {
                Name = "Updated Name",
            });
        }

        public async Task<List<Products>> Get(string name)
        {
            var all = await _unitOfWork.GetRepository<Products>().GetAllAsync();
            var lstData = all.Where(x => x.Name.Contains(name)).ToList();
            return lstData;
        }
    }
}
