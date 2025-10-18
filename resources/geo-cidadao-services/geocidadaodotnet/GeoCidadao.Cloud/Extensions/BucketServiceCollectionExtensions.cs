using Microsoft.Extensions.DependencyInjection;

namespace GeoCidadao.Cloud.Extensions
{
    public static class BucketServiceCollectionExtensions
    {
        public static IServiceCollection AddBucketServices(this IServiceCollection services)
        {
            // services.AddTransient<IAmazonBucketService, AmazonBucketService>();
            // services.AddTransient<IOracleBucketService, OracleBucketService>();
            // services.AddTransient<IBucketService, BucketService>();

            // services.AddTransient<BucketServiceFactory>();

            return services;
        }
    }
}