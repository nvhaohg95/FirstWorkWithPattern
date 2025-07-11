using Application.Interfaces;
using Application.Interfaces.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace Infrastructure.Persistence.Repositories
{
    public class RepositoryStrategy<T> : IRepositoryStrategy<T> where T : class
    {
        private readonly IRepositoryBase<T> _efRepo;
        private readonly IRepositoryBase<T> _dapperRepo;
        //private readonly IRepositoryBase<T> _mongoRepo;
        private readonly string _provider;

        public RepositoryStrategy(
            IServiceProvider serviceProvider,
            IConfiguration config)
        {
            _provider = config["Provider"];
            _efRepo = (IRepositoryBase<T>)serviceProvider.GetService(typeof(EFRepository<T>));
            _dapperRepo = (IRepositoryBase<T>)serviceProvider.GetService(typeof(DapperRepository<T>));
            //_mongoRepo = (IRepositoryBase<T>)serviceProvider.GetService(typeof(MongoRepository<T>));
        }

        public IRepositoryBase<T> GetRepository()
        {
            return _provider switch
            {
                "EF" => _efRepo,
                "Dapper" => _dapperRepo,
                //"Mongo" => _mongoRepo,
                _ => throw new NotSupportedException($"Provider {_provider} is not supported.")
            };
        }
    }
} 