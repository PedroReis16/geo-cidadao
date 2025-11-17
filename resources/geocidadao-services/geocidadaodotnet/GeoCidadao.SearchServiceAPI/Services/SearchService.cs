using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using GeoCidadao.Models.Enums;
using GeoCidadao.SearchServiceAPI.Config;
using GeoCidadao.SearchServiceAPI.Models;
using GeoCidadao.SearchServiceAPI.Models.DTOs;
using GeoCidadao.SearchServiceAPI.Services.Contracts;

namespace GeoCidadao.SearchServiceAPI.Services;

/// <summary>
/// Implementation of search operations
/// </summary>
public class SearchService : ISearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<SearchService> _logger;
    private readonly string _postsIndex;
    private readonly string _usersIndex;

    public SearchService(
        ElasticsearchClient client,
        IConfiguration configuration,
        ILogger<SearchService> logger)
    {
        _client = client;
        _logger = logger;
        
        var esConfig = configuration.GetSection("Elasticsearch").Get<ElasticsearchConfiguration>();
        var defaultIndex = esConfig?.DefaultIndex ?? "geocidadao";
        _postsIndex = $"{defaultIndex}-posts";
        _usersIndex = $"{defaultIndex}-users";
    }

    public async Task<SearchResponseDTO<PostDocument>> SearchPostsAsync(SearchRequestDTO request)
    {
        try
        {
            var mustQueries = new List<Query>();
            
            // Always filter out deleted and non-public posts
            mustQueries.Add(new TermQuery(new Field("isDeleted")) { Value = false });
            mustQueries.Add(new TermQuery(new Field("isPublic")) { Value = true });

            var shouldQueries = new List<Query>();

            // Text search
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                shouldQueries.Add(new MatchQuery(new Field("content")) { Query = request.Query, Boost = 2.0f });
                shouldQueries.Add(new MatchQuery(new Field("authorName")) { Query = request.Query, Boost = 1.5f });
                shouldQueries.Add(new MatchQuery(new Field("tags")) { Query = request.Query, Boost = 1.8f });
            }

            // Location filter
            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var locationShould = new List<Query>
                {
                    new MatchQuery(new Field("locationCity")) { Query = request.Location },
                    new MatchQuery(new Field("locationNeighborhood")) { Query = request.Location }
                };
                
                mustQueries.Add(new BoolQuery
                {
                    Should = locationShould,
                    MinimumShouldMatch = 1
                });
            }

            // Author filter
            if (request.AuthorId.HasValue)
            {
                mustQueries.Add(new TermQuery(new Field("authorId")) { Value = request.AuthorId.Value.ToString() });
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                if (Enum.TryParse<PostCategory>(request.Category, true, out var category))
                {
                    mustQueries.Add(new TermQuery(new Field("category")) { Value = category.ToString() });
                }
            }

            // Date range filter
            if (request.DateFrom.HasValue || request.DateTo.HasValue)
            {
                var dateRange = new DateRangeQuery(new Field("createdAt"));
                if (request.DateFrom.HasValue)
                    dateRange.Gte = request.DateFrom.Value;
                if (request.DateTo.HasValue)
                    dateRange.Lte = request.DateTo.Value;
                
                mustQueries.Add(dateRange);
            }

            var boolQuery = new BoolQuery
            {
                Must = mustQueries,
                Should = shouldQueries.Count > 0 ? shouldQueries : null,
                MinimumShouldMatch = shouldQueries.Count > 0 ? 1 : null
            };

            var from = (request.Page - 1) * request.PageSize;
            
            var searchResponse = await _client.SearchAsync<PostDocument>(s => s
                .Index(_postsIndex)
                .From(from)
                .Size(request.PageSize)
                .Query(boolQuery)
            );

            if (!searchResponse.IsValidResponse)
            {
                _logger.LogError("Search failed: {Error}", searchResponse.DebugInformation);
                return new SearchResponseDTO<PostDocument>
                {
                    Results = new List<PostDocument>(),
                    TotalResults = 0,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = 0,
                    SearchTimeMs = searchResponse.Took
                };
            }

            var totalResults = searchResponse.Total;
            var totalPages = (int)Math.Ceiling((double)totalResults / request.PageSize);

            return new SearchResponseDTO<PostDocument>
            {
                Results = searchResponse.Documents.ToList(),
                TotalResults = totalResults,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                SearchTimeMs = searchResponse.Took
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching posts");
            return new SearchResponseDTO<PostDocument>
            {
                Results = new List<PostDocument>(),
                TotalResults = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = 0,
                SearchTimeMs = 0
            };
        }
    }

    public async Task<SearchResponseDTO<UserDocument>> SearchUsersAsync(SearchRequestDTO request)
    {
        try
        {
            var mustQueries = new List<Query>();
            
            // Always filter out deleted and inactive users
            mustQueries.Add(new TermQuery(new Field("isDeleted")) { Value = false });
            mustQueries.Add(new TermQuery(new Field("isActive")) { Value = true });

            var shouldQueries = new List<Query>();

            // Text search
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                shouldQueries.Add(new MatchQuery(new Field("username")) { Query = request.Query, Boost = 2.0f });
                shouldQueries.Add(new MatchQuery(new Field("fullName")) { Query = request.Query, Boost = 1.8f });
                shouldQueries.Add(new MatchQuery(new Field("firstName")) { Query = request.Query, Boost = 1.5f });
                shouldQueries.Add(new MatchQuery(new Field("lastName")) { Query = request.Query, Boost = 1.5f });
            }

            var boolQuery = new BoolQuery
            {
                Must = mustQueries,
                Should = shouldQueries.Count > 0 ? shouldQueries : null,
                MinimumShouldMatch = shouldQueries.Count > 0 ? 1 : null
            };

            var from = (request.Page - 1) * request.PageSize;
            
            var searchResponse = await _client.SearchAsync<UserDocument>(s => s
                .Index(_usersIndex)
                .From(from)
                .Size(request.PageSize)
                .Query(boolQuery)
            );

            if (!searchResponse.IsValidResponse)
            {
                _logger.LogError("User search failed: {Error}", searchResponse.DebugInformation);
                return new SearchResponseDTO<UserDocument>
                {
                    Results = new List<UserDocument>(),
                    TotalResults = 0,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = 0,
                    SearchTimeMs = searchResponse.Took
                };
            }

            var totalResults = searchResponse.Total;
            var totalPages = (int)Math.Ceiling((double)totalResults / request.PageSize);

            return new SearchResponseDTO<UserDocument>
            {
                Results = searchResponse.Documents.ToList(),
                TotalResults = totalResults,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                SearchTimeMs = searchResponse.Took
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return new SearchResponseDTO<UserDocument>
            {
                Results = new List<UserDocument>(),
                TotalResults = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = 0,
                SearchTimeMs = 0
            };
        }
    }
}
