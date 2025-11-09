using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GeoCidadao.MongoDatabase.Extensions
{
    public static class MongoExtensions
    {
        public static IServiceCollection AddMongoDbContext(this IServiceCollection services, string connectionString)
        {
            var regex = new Regex(@"^(mongodb(?:\+srv)?://[^/]+(?:/(?<database>[^?]+))?(?:\?.*)?|(?:mongodb(?:\+srv)?://)?[^/]+/(?<database>[^?]+)(?:\?.*)?)$");
            var match = regex.Match(connectionString);

            if (!match.Success)
                throw new ArgumentException("Formato da connection string do MongoDB inválido", nameof(connectionString));

            string actualDatabaseName =
             match.Groups["database"].Success ?
              match.Groups["database"].Value :
              throw new ArgumentException("Database name não encontrado na connection string", nameof(connectionString));

            string actualConnectionString = connectionString;

            services.AddSingleton<IMongoClient>(sp => new MongoClient(actualConnectionString));
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(actualDatabaseName);
            });

            return services;
        }
    }
}