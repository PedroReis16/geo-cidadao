using Elastic.Clients.Elasticsearch;
using GeoCidadao.Models.Extensions;
using GeoCidadao.RelevanceWorker.Config;
using GeoCidadao.RelevanceWorker.Contracts;
using GeoCidadao.RelevanceWorker.Models.DTOs;

namespace GeoCidadao.RelevanceWorker.Services
{
    internal class ElasticSearchService(ILogger<ElasticSearchService> logger, ElasticsearchClient client, IConfiguration configuration) : IElasticSearchService
    {
        private readonly ILogger<ElasticSearchService> _logger = logger;
        private readonly ElasticsearchClient _client = client;
        private readonly ElasticSearchSettings _settings = configuration.GetSection(AppSettingsProperties.ElasticSearch).Get<ElasticSearchSettings>()!;

        public async Task IndexPostAsync(Guid postId, RelevanceDocument postDocument, CancellationToken cancellationToken = default)
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
                var response = await _client.DeleteAsync<RelevanceDocument>(postId, cancellationToken);

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

        public async Task<RelevanceDocument?> FindPostDetailsAsync(Guid postId)
        {
            try
            {
                GetResponse<RelevanceDocument> response = await _client.GetAsync<RelevanceDocument>(postId, idx => idx.Index(_settings.DefaultIndex));

                if (response.Found)
                {
                    return response.Source;
                }

                return null;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve uma falha ao tentar buscar os detalhes do post '{postId}' no Elastic Search: {ex.GetFullMessage()}.";
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }

        public async Task UpdatePostAsync(Guid postId,  RelevanceDocument postDocument, CancellationToken cancellationToken = default)
        {
            try
            {
                UpdateResponse<RelevanceDocument> response = await _client.UpdateAsync<RelevanceDocument, RelevanceDocument>(
                   _settings.DefaultIndex,
                   postId,
                   u => u
                       .Doc(new()
                       {
                           RelevanceScore = postDocument.RelevanceScore,
                           LikesCount = postDocument.LikesCount,
                           CommentsCount = postDocument.CommentsCount
                       })
                       .DocAsUpsert(true),
                   cancellationToken
                   );

                if (!response.IsValidResponse)
                {
                    string errorMessage = $"Houve uma falha ao tentar atualizar o post '{postId}' no Elastic Search. " +
                        $"Response Debug Information: {response.DebugInformation}";

                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve uma falha ao tentar atualizar o post '{postId}' no Elastic Search: {ex.GetFullMessage()}.";
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }

    }
}