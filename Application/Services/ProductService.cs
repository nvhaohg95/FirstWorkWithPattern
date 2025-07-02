
using Application.Interfaces.Repositories;
using Application.Services.Base;
using Domain;
namespace Application.Services
{
    public class ProductService : ServiceBase
    {
        public ProductService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<Product> Add(Product product)
        {
            await Repository<Product>().AddAsync(product);
            await SaveChangesAsync();
            return product;
        }

        public List<Product> Get(string name)
        {
            var lstData = Repository<Product>().Queryable().Where(x => x.Name.Contains(name)).ToList();

            return lstData;
        }
    }
}
