using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using GeoCidadao.FeedServiceAPI.Config;
using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Database.Documents;
using GeoCidadao.FeedServiceAPI.Models.DTOs;
using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedServiceAPI.Services
{
    internal class PostsDaoService(ElasticsearchClient client, ElasticSearchSettings settings) : IPostsDaoService
    {
        private readonly ElasticsearchClient _client = client;
        private readonly ElasticSearchSettings _settings = settings;

        public async Task<List<PostDocument>> GetPostsAsync(UserInterestsDTO interests, List<Guid> viewedPosts, int page, int pageSize)
        {
            var results = new List<PostDocument>();
            var offset = page * pageSize;

            // Primeira consulta: Posts baseados nos interesses do usuário
            if (interests != null && (interests.Users?.Any() == true || interests.Cities?.Any() == true))
            {
                var interestedPostsResponse = await _client.SearchAsync<PostDocument>(s => s
                    .Indices(_settings.DefaultIndex)
                    .From(offset)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                sh => sh.Terms(t => t.Field(f => f.PostOwnerId).Terms(new TermsQueryField(interests.Users?.Select(u => (FieldValue)u.ToString())?.ToArray() ?? Array.Empty<FieldValue>()))),
                                sh => sh.Terms(t => t.Field(f => f.City).Terms(new TermsQueryField(interests.Cities?.Select(c => (FieldValue)c)?.ToArray() ?? Array.Empty<FieldValue>())))
                            )
                            .MinimumShouldMatch(1)
                            .MustNot(mn => mn
                                .Ids(new IdsQuery
                                {
                                    Values = new Ids(viewedPosts.Select(id => new Id(id.ToString())).ToArray())
                                })
                            )
                        )
                    )
                );

                if (interestedPostsResponse.Documents != null)
                {
                    results.AddRange(interestedPostsResponse.Documents);
                }
            }

            // Segunda consulta: Completar com posts gerais/sugeridos se necessário
            int remaining = pageSize - results.Count;
            if (remaining > 0)
            {
                // Permite repetição removendo apenas os já retornados na primeira query
                List<Guid> excludeIds = results.Select(p => p.Id).ToList();

                var generalPostsResponse = await _client.SearchAsync<PostDocument>(s => s
                    .Indices(_settings.DefaultIndex)
                    .From(0)
                    .Size(remaining)
                    .Query(q => q
                        .Bool(b => b
                            .MustNot(mn => mn
                                .Ids(new IdsQuery
                                {
                                    Values = new Ids(excludeIds.Select(id => new Id(id.ToString())).ToArray())
                                })
                            )
                        )
                    )
                );

                if (generalPostsResponse.Documents != null)
                {
                    results.AddRange(generalPostsResponse.Documents);
                }
            }

            return results;
        }
    }
}