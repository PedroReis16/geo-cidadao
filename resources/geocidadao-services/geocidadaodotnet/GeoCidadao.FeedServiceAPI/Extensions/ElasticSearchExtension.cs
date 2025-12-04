using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using GeoCidadao.FeedServiceAPI.Config;
using GeoCidadao.FeedServiceAPI.Database.Documents;

namespace GeoCidadao.FeedServiceAPI.Models.Extensions
{
    internal static class ElasticSearchExtension
    {
        public static IServiceCollection AddElasticSearchService(this IServiceCollection services)
        {
            ElasticSearchSettings settings = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>()
                .GetSection(AppSettingsProperties.ElasticSearch)
                .Get<ElasticSearchSettings>()!;

            services.AddSingleton(settings);

            var clientSettings = new ElasticsearchClientSettings(new Uri(settings.Uri))
                .DefaultIndex(settings.DefaultIndex)
                .Authentication(new BasicAuthentication(settings.UserName, settings.Password));

            var client = new ElasticsearchClient(clientSettings);

            services.AddSingleton(client);
            services.AddSingleton<PostDocument>();

            return services;
        }

        public static async Task InitializeElasticSearchAsync(this IServiceProvider serviceProvider)
        {
            // var postIndex = serviceProvider.GetRequiredService<RelevanceIndex>();
            // await postIndex.CreateIndexAsync();
        }
    }
}