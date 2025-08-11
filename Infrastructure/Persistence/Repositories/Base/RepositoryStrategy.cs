using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Persistence.Repositories.Base
{
    public class RepositoryStrategy<T> : IRepositoryStrategy<T> where T : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _provider;

        public RepositoryStrategy(IServiceProvider serviceProvider, IConfiguration config)
        {
            _provider = config["Provider"]?.ToLower();
            _serviceProvider = serviceProvider;
        }

        public IRepositoryBase<T> GetRepository()
        {
            return _provider switch
            {
                "ef" => _serviceProvider.GetRequiredService<EFRepository<T>>(),
                "linq2db" => _serviceProvider.GetRequiredService<Linq2DbRepository<T>>(),
                _ => throw new NotSupportedException($"Provider {_provider} is not supported.")
            };
        }
    }
}