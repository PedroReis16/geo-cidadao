using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Model;

namespace GeoCidadao.FeedServiceAPI.Services
{
    public class FeedService(ElasticsearchClient client, ILogger<FeedService> logger, IUserInterestsService userInterestsService, ISeenPostsService seenPostsService, IEngagementService engagementService)
    {
        private readonly ElasticsearchClient _client = client;
        private readonly ILogger<FeedService> _logger = logger;
        private readonly IUserInterestsService _userInterestsService = userInterestsService;
        private readonly ISeenPostsService _seenPostsService = seenPostsService;
        private readonly IEngagementService _engagementService = engagementService;
        private const string IndexName = "posts";

        public async Task<List<PostDocument>> GetFeedAsync(Guid userId, double? lat, double? lon, int page = 1, int pageSize = 20)
        {
            var interestsTask = _userInterestsService.GetUserInterestsAsync(userId);
            // var seenPostsTask = _seenPostsService.GetSeenPostIdsAsync(userId);

            await Task.WhenAll(interestsTask);

            var interests = await interestsTask;
            // var seenPosts = await seenPostsTask;

            var response = await _client.SearchAsync<PostDocument>(s => s
                .Indices(IndexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .FunctionScore(fs => fs
                        .Query(bq => bq
                            .Bool(b =>
                            {
                                // if (seenPosts != null && seenPosts.Any())
                                // {
                                //     b.MustNot(mn => mn.Ids(i => i.Values(new Ids(seenPosts.Select(id => new Id(id.ToString()))))));
                                // }

                                if (interests != null)
                                {
                                    if (interests.Cities.Any())
                                    {
                                        b.Should(sh => sh.Terms(t => t.Field(f => f.City).Terms(new TermsQueryField(interests.Cities.Select(FieldValue.String).ToList()))));
                                    }
                                    if (interests.Tags.Any())
                                    {
                                        b.Should(sh => sh.Terms(t => t.Field(f => f.Tags).Terms(new TermsQueryField(interests.Tags.Select(FieldValue.String).ToList()))));
                                    }
                                }

                                if (lat.HasValue && lon.HasValue)
                                {
                                    // Filter by distance if needed, or just boost
                                    // For now, let's just boost closer posts
                                }
                            })
                        )
                        .Functions(fun => fun
                            .FieldValueFactor(fv => fv
                                .Field(f => f.RelevanceScore)
                                .Factor(1.2)
                                .Modifier(FieldValueFactorModifier.Sqrt)
                                .Missing(0)
                            )
                        // Decay function for location could be added here
                        )
                    )
                )
                .Sort(sort => sort
                    .Score(sc => sc.Order(SortOrder.Desc))
                    .Field(f => f.CreatedAt, fs => fs.Order(SortOrder.Desc))
                )
            );

            if (!response.IsValidResponse)
            {
                _logger.LogError($"Failed to search posts: {response.DebugInformation}");
                return new List<PostDocument>();
            }

            var posts = response.Documents.ToList();

            // Hydrate with Liked status
            if (posts.Any())
            {
                var likedPostIds = await _engagementService.GetLikedPostIdsAsync(userId, posts.Select(p => p.Id).ToList());
                foreach (var post in posts)
                {
                    post.LikedByCurrentUser = likedPostIds.Contains(post.Id);
                    // Mark as seen asynchronously
                    // _ = _seenPostsService.MarkPostAsSeenAsync(userId, post.Id);
                }
            }

            return posts;
        }
    }
}
