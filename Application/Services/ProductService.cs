
using Application.Base;
using Application.Interfaces;
using Application.Interfaces;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : ServiceBase
    {
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> Add(Product product)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GetRepository<Product>().AddAsync(product);
                await _unitOfWork.CommitAsync();
                return product;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Product>> Get(string name)
        {
            var all = await _unitOfWork.GetRepository<Product>().GetAllAsync();
            var lstData = all.Where(x => x.Name.Contains(name)).ToList();
            return lstData;
        }
    }
}
