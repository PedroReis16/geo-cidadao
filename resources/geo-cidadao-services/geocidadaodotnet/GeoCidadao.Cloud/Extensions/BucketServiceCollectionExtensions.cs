using GeoCidadao.Cloud.Config;
using GeoCidadao.Cloud.Contracts;
using GeoCidadao.Cloud.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GeoCidadao.Cloud.Extensions
{
    public static class BucketServiceCollectionExtensions
    {
        public static IServiceCollection AddBucketServices(this IServiceCollection services)
        {
            services.AddTransient<ICloudBucketService, CloudBucketService>();

            services.AddTransient<IStartupFilter, BucketStartupFilter>();

            return services;
        }
    }
}