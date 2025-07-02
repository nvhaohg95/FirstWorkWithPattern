using Application.Interfaces.Repositories;
using Application.Services;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // đăng ký thủ công từng repository
            //services.AddScoped<IProductRepository, ProductRepository>();
            // Đăng ký repository tự động
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

            // Đăng ký UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // ... các service khác
            services.AddScoped<ProductService>();

            return services;
        }
    }
}
