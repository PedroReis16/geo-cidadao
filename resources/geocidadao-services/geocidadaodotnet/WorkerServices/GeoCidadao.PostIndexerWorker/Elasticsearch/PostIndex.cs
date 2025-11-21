using System.Diagnostics;
using Elastic.Clients.Elasticsearch;
using GeoCidadao.Models.Extensions;
using GeoCidadao.PostIndexerWorker.Config;

namespace GeoCidadao.PostIndexerWorker.Elasticsearch
{
    public class PostIndex(ILogger<PostIndex> logger, ElasticsearchClient client, IConfiguration configuration)
    {
        private readonly ILogger<PostIndex> _logger = logger;
        private readonly ElasticsearchClient _client = client;
        private readonly string _indexName = configuration.GetSection(AppSettingsProperties.ElasticSearch)
            .Get<ElasticSearchSettings>()!.DefaultIndex;

        public async Task CreateIndexAsync()
        {
            try
            {
                var exists = await _client.Indices.ExistsAsync(_indexName);

                if (exists.Exists)
                    return;

                var response = await _client.Indices.CreateAsync(_indexName, c => c
                      .Mappings(m => m
                          .Properties(p => p
                              .Text("content")
                              .Keyword("postId")
                              .Keyword("postOwnerId")
                              .Keyword("city")
                              .GeoPoint("location")
                              .Keyword("tags")
                              .FloatNumber("relevanceScore")
                          )
                      )
                  );

                if (!response.IsValidResponse)
                    throw new Exception($"Erro ao criar o índice no ElasticSearch. Response Debug Information: {response.DebugInformation}");

                _logger.LogInformation($"Índice de postagens '{_indexName}' atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar o índice de posts no ElasticSearch: {ex.GetFullMessage()}");
            }
        }

    }
}