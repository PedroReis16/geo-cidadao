using Elastic.Clients.Elasticsearch;
using GeoCidadao.PostIndexerWorker.Contracts;
using GeoCidadao.PostIndexerWorker.Models.DTOs;

namespace GeoCidadao.PostIndexerWorker.Services
{
    internal class ElasticSearchService(ILogger<ElasticSearchService> logger, ElasticsearchClient client) : IElasticSearchService
    {
        private readonly ILogger<ElasticSearchService> _logger = logger;
        private readonly ElasticsearchClient _client = client;

        public async Task IndexPostAsync(Guid postId, PostDocument postDocument, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.IndexAsync(postDocument, i => i.Id(postId), cancellationToken);

                if (!response.IsValidResponse)
                {
                    string errorMessage = $"Houve uma falha ao tentar indexar o post '{postId}' no Elastic Search. " +
                        $"Response Debug Information: {response.DebugInformation}";

                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve uma falha ao tentar indexar o post '{postId}' no Elastic Search.";
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }

        public async Task DeletePostIndexAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.DeleteAsync<PostDocument>(postId, cancellationToken);

                if (!response.IsValidResponse)
                {
                    string errorMessage = $"Houve uma falha ao tentar deletar o índico do post '{postId}' no Elastic Search. " +
                        $"Response Debug Information: {response.DebugInformation}";

                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve uma falha ao tentar deletar o índico do post '{postId}' no Elastic Search.";
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }

    }
}