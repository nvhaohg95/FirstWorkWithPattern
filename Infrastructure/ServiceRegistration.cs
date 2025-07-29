using Application.Services;
using Infrastructure.Persistence.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Application.Interfaces;
using MongoDB.Driver;
using Infrastructure.Persistence.Authentication;
using Infrastructure.Persistence.Log;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext cho EF
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQL")));

            // Đăng ký IDbConnection cho Dapper
            services.AddScoped<IDbConnection>(sp =>
                new SqlConnection(configuration.GetConnectionString("Postgre")));

            // Đăng ký các repository cho từng provider
            services.AddScoped(typeof(EFRepository<>));
            services.AddScoped(typeof(DapperRepository<>));
            services.AddScoped(typeof(MongoRepository<>));

            // Đăng ký Strategy Pattern
            services.AddScoped(typeof(IRepositoryStrategy<>), typeof(RepositoryStrategy<>));

            // Đăng ký ProductService
            services.AddScoped<ProductService>();

            // Đăng ký IJwtService
            services.AddScoped<IJwtService, JwtService>();

            // Đăng ký ILogService cho từng loại controller
            services.AddScoped(typeof(ILogService<>), typeof(Logs<>));

            // Đăng ký IMongoClient và IMongoDatabase
            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(configuration.GetConnectionString("MongoDB")));
            services.AddScoped<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(configuration["MongoDB"]));

            // Đăng ký UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
