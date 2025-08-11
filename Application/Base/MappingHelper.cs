using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Base
{
    public static class MapperHelper
    {
        private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
           {
               builder.Equals(LogLevel.Error);
               builder.SetMinimumLevel(LogLevel.Information);
           });
        public static T MapTo<T>(object source)
        {

            var config = new MapperConfiguration(cfg =>
            {
                // Tự động tạo mapping giữa source type và TTarget
                cfg.CreateMap(source.GetType(), typeof(T));
            }, _loggerFactory);

            var mapper = config.CreateMapper();

            return (T)mapper.Map(source, source.GetType(), typeof(T));
        }
    }
}
