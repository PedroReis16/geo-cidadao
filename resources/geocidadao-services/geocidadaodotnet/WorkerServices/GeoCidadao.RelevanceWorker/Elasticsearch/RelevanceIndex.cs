using Elastic.Clients.Elasticsearch;
using GeoCidadao.Models.Extensions;
using GeoCidadao.RelevanceWorker.Config;

namespace GeoCidadao.RelevanceWorker.Elasticsearch
{
    public class RelevanceIndex(ILogger<RelevanceIndex> logger, ElasticsearchClient client, IConfiguration configuration)
    {
        private readonly ILogger<RelevanceIndex> _logger = logger;
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
                              .FloatNumber("relevanceScore")
                              .IntegerNumber("likesCount")
                              .IntegerNumber("commentsCount")
                          )
                      )
                  );

                if (!response.IsValidResponse)
                    throw new Exception($"Erro ao criar o índice no ElasticSearch. Response Debug Information: {response.DebugInformation}");

                _logger.LogInformation($"Índice de relevância '{_indexName}' atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar o índice de relevância no ElasticSearch: {ex.GetFullMessage()}");
            }
        }

    }
}