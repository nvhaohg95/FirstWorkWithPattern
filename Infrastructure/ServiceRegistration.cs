using Application.Services;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Application.Interfaces;
using MongoDB.Driver;
using Infrastructure.Persistence.Authentication;
using Infrastructure.Persistence.Log;
using System.Reflection;
using Application.Base;
using LinqToDB.Data;
using Infrastructure.Persistence.Repositories.Mongo;
using Infrastructure.Persistence.Repositories.Base;
using Infrastructure.Persistence.UnitOfWorks.Base;
using Infrastructure.Persistence.UnitOfWorks.EF;
using Infrastructure.Persistence.UnitOfWorks;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        /// <summary>
        /// Add các server ko phải nghiệp vụ
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDatabaseProviders(configuration);

            // Đăng ký Strategy Pattern
            services.AddScoped(typeof(IRepositoryStrategy<>), typeof(RepositoryStrategy<>));

            // Đăng ký IJwtService
            services.AddScoped<IJwtService, JwtService>();

            // Đăng ký ILogService cho từng loại controller
            services.AddScoped(typeof(ILogService<>), typeof(Logs<>));

            //Thêm 1 service duy nhất để tự động add các service khác theo mẫu
            services.RegisterAppServices(typeof(ProductService).Assembly);

            return services;
        }

        public static IServiceCollection RegisterAppServices(this IServiceCollection services, Assembly assemblyToScan)
        {
            var serviceTypes = assemblyToScan.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    (typeof(ServiceBase).IsAssignableFrom(t))
                );

            foreach (var implementationType in serviceTypes)
            {
                var interfaceType = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i != typeof(ServiceBase)); // Loại bỏ IAppService nếu có

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                }
                else
                {
                    services.AddScoped(implementationType);
                }
            }

            return services;
        }

        public static IServiceCollection RegisterDatabaseProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var dbType = configuration["DbType"];
            var provider = configuration["Provider"];
            string sConnectionString = configuration.GetConnectionString(dbType);
            
            services.AddScoped<IUnitOfWorkStrategy, UnitOfWorkStrategy>();
            if (provider.ToLower() == "ef")
            {
                if (dbType.ToLower().Contains("sqlserver"))
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(sConnectionString));
                }
                else if (dbType.ToLower().Contains("postgresql"))
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(sConnectionString));
                }
                services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                services.AddScoped<EfUnitOfWork>();
                services.AddScoped(typeof(EFRepository<>));
            }
            else if (provider.ToLower() == "linq2db")
            {
                services.AddScoped<DataConnection>(sp =>
                {
                    var providerName = dbType.ToLower().StartsWith("sql")
                        ? LinqToDB.ProviderName.SqlServer
                        : LinqToDB.ProviderName.PostgreSQL;

                    return new DataConnection(providerName, sConnectionString);
                });
                services.AddScoped<IUnitOfWork, Linq2DbUnitOfWork>();
                services.AddScoped<Linq2DbUnitOfWork>();
                services.AddScoped(typeof(Linq2DbRepository<>));
            }
            else if (provider.ToLower() == "mongodb")
            {
                services.AddSingleton<IMongoClient>(sp => new MongoClient(sConnectionString));
                services.AddScoped(sp =>
                {
                    var client = sp.GetRequiredService<IMongoClient>();
                    return client.GetDatabase("DefaultDb");
                });

                services.AddScoped(typeof(MongoRepository<>));
                services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
            }
            return services;
        }
    }
}
