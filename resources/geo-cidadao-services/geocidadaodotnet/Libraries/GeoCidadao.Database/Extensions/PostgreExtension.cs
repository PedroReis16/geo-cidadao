using GeoCidadao.Database.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoCidadao.Database.Extensions
{
    public static class PostgreExtension
    {
        public static IServiceCollection UsePostgreSql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GeoDbContext>(options =>
            {
                _ = options.UseNpgsql(configuration.GetConnectionString("GeoDb"),
                o => o.UseNetTopologySuite());
            });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddTransient<IStartupFilter, MigrationStartupFilter<GeoDbContext>>();

            return services;
        }
    }
}