using Application.Interfaces;
using Infrastructure.Persistence.UnitOfWorks.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.UnitOfWorks.Base
{
    public class UnitOfWorkStrategy : IUnitOfWorkStrategy
    {
        private readonly IServiceProvider _sp;
        private readonly IConfiguration _config;

        public UnitOfWorkStrategy(IServiceProvider sp, IConfiguration config)
        {
            _sp = sp;
            _config = config;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            var provider = _config["Provider"]?.ToLower();

            return provider switch
            {
                "ef" => _sp.GetRequiredService<EfUnitOfWork>(),
                "linq2db" => _sp.GetRequiredService<Linq2DbUnitOfWork>(),
                "mongodb" => _sp.GetRequiredService<MongoUnitOfWork>(),
                _ => throw new NotSupportedException($"Provider {provider} is not supported.")
            };
        }
    }

}
