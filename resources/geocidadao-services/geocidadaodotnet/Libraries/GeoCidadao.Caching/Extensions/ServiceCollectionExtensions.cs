using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Contracts;
using GeoCidadao.Caching.Services;

namespace GeoCidadao.Caching.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryCache(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddMemoryCache()
                .AddSingleton(GetCacheConfig(configuration))
                .AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
        }

        private static InMemoryCacheConfig GetCacheConfig(IConfiguration configuration)
        {
            _ = int.TryParse(configuration
                .GetSection(InMemoryCacheConfig.IN_MEMORY_CONFIG_SECTION)
                .GetSection(InMemoryCacheConfig.IN_MEMORY_MAX_AGE)
                .Value, out int maxAge);
            return new InMemoryCacheConfig(maxAge);
        }
    }
}
