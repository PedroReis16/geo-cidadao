using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using GeoCidadao.FeedMapAPI.Config;
using GeoCidadao.FeedMapAPI.Contracts;
using GeoCidadao.FeedMapAPI.Database.Documents;
using GeoCidadao.FeedMapAPI.Models.DTOs;
using GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedMapAPI.Services
{
    internal class PostsDaoService(ElasticsearchClient client, ElasticSearchSettings settings) : IPostsDaoService
    {
        private readonly ElasticsearchClient _client = client;
        private readonly ElasticSearchSettings _settings = settings;

        public async Task<List<PostDocument>> GetPostsAsync(UserInterestsDTO interests, List<Guid> viewedPosts, int page, int pageSize)
        {
            var results = new List<PostDocument>();
            var offset = page <= 1 ? 0 : (page - 1) * pageSize;
            var indexName = _settings.DefaultIndex;

            if (interests != null && (interests.Users?.Any() == true || interests.Cities?.Any() == true))
            {
                var interestedPostsResponse = await _client.SearchAsync<PostDocument>(s => s
                    .Indices(indexName)
                    .From(offset)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                sh => sh.Terms(t => t.Field(f => f.PostOwnerId)
                                    .Terms(new TermsQueryField(interests.Users?
                                        .Select(u => (FieldValue)u.ToString())
                                        .ToArray() ?? Array.Empty<FieldValue>()))),
                                sh => sh.Terms(t => t.Field(f => f.City)
                                    .Terms(new TermsQueryField(interests.Cities?
                                        .Select(c => (FieldValue)c)
                                        .ToArray() ?? Array.Empty<FieldValue>())))
                            )
                            .MinimumShouldMatch(1)
                            .MustNot(mn => mn
                                .Ids(new IdsQuery
                                {
                                    Values = new Ids(viewedPosts
                                        .Select(id => new Id(id.ToString()))
                                        .ToArray())
                                })
                            )
                        )
                    )
                );

                if (interestedPostsResponse.Hits != null && interestedPostsResponse.Hits.Count > 0)
                {
                    foreach (var hit in interestedPostsResponse.Hits)
                    {
                        var doc = hit.Source;
                        if (doc != null)
                        {
                            doc.Id = Guid.Parse(hit.Id);
                            results.Add(doc);
                        }
                    }
                }
            }

            int remaining = pageSize - results.Count;
            if (remaining > 0)
            {
                var excludeIds = results.Select(r => r.Id).Distinct().ToList();

                var generalPostsResponse = await _client.SearchAsync<PostDocument>(s => s
                    .Indices(indexName)
                    .Size(remaining)
                    .Query(q => q
                        .Bool(b => b
                            .Must(mu => mu.MatchAll(new MatchAllQuery()))
                            .MustNot(mn => mn
                                .Ids(new IdsQuery
                                {
                                    Values = new Ids(excludeIds
                                        .Select(id => new Id(id.ToString()))
                                        .ToArray())
                                })
                            )
                        )
                    )
                );

                if (generalPostsResponse.Hits != null && generalPostsResponse.Hits.Count > 0)
                {
                    foreach (var hit in generalPostsResponse.Hits)
                    {
                        var doc = hit.Source;
                        if (doc != null)
                        {
                            doc.Id = Guid.Parse(hit.Id);
                            results.Add(doc);
                        }
                    }
                }
            }

            return results;
        }

        public async Task<List<PostDocument>> GetPostsInBoundsAsync(
            double topLeftLat,
            double topLeftLon,
            double bottomRightLat,
            double bottomRightLon,
            double minRelevanceScore,
            int limit)
        {
            var results = new List<PostDocument>();
            var indexName = _settings.DefaultIndex;

            // Query geoespacial com bounding box e filtro de relevância
            var response = await _client.SearchAsync<PostDocument>(s => s
                .Indices(indexName)
                .Size(limit)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            // Filtro geográfico - postagens dentro do bounding box
                            m => m.Bool(bb => bb
                                .Must(
                                    // Latitude entre bottomRight e topLeft
                                    lat => lat.Range(r => r
                                        .Number(nr => nr
                                            .Field(f => f.Latitude)
                                            .Gte(bottomRightLat)
                                            .Lte(topLeftLat)
                                        )
                                    ),
                                    // Longitude entre topLeft e bottomRight
                                    lon => lon.Range(r => r
                                        .Number(nr => nr
                                            .Field(f => f.Longitude)
                                            .Gte(topLeftLon)
                                            .Lte(bottomRightLon)
                                        )
                                    )
                                )
                            ),
                            // Filtro de relevância mínima
                            m => m.Range(r => r
                                .Number(nr => nr
                                    .Field(f => f.RelevanceScore)
                                    .Gte(minRelevanceScore)
                                )
                            )
                        )
                    )
                )
                .Sort(s => s.Field(f => f.RelevanceScore, order: Elastic.Clients.Elasticsearch.SortOrder.Desc))
            );

            if (response.Hits != null && response.Hits.Count > 0)
            {
                foreach (var hit in response.Hits)
                {
                    var doc = hit.Source;
                    if (doc != null && doc.Latitude.HasValue && doc.Longitude.HasValue)
                    {
                        doc.Id = Guid.Parse(hit.Id);
                        results.Add(doc);
                    }
                }
            }

            return results;
        }
    }
}