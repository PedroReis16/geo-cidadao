using GeoCidadao.Models.Enums;
using GeoCidadao.RelevanceWorker.Contracts;
using GeoCidadao.RelevanceWorker.Models.DTOs;

namespace GeoCidadao.RelevanceWorker.Services
{
    public class InteractionService(ILogger<InteractionService> logger, IElasticSearchService elasticSearchService) : IInteractionService
    {
        private readonly ILogger<InteractionService> _logger = logger;
        private readonly IElasticSearchService _elasticSearchService = elasticSearchService;

        public async Task UpdatePostInteractionAsync(Guid postId, InteractionType interactionType)
        {
            double relevanceChange = interactionType switch
            {
                InteractionType.PostLike => 1.0,
                InteractionType.PostComment => 0.5,
                InteractionType.PostCommentLiked => 0.2,
                InteractionType.PostUnlike => -1.0,
                InteractionType.PostReported => -2.0,
                _ => 0.0
            };

            RelevanceDocument? postDetails = await _elasticSearchService.FindPostDetailsAsync(postId);

            if (postDetails == null)
            {
                postDetails = new RelevanceDocument
                {
                    RelevanceScore = Math.Max(0.0, relevanceChange),
                    LikesCount = interactionType == InteractionType.PostLike ? 1 : 0,
                    CommentsCount = interactionType == InteractionType.PostComment ? 1 : 0,
                };
            }
            else
            {
                postDetails.RelevanceScore = Math.Max(0.0, postDetails.RelevanceScore + relevanceChange);

                switch (interactionType)
                {
                    case InteractionType.PostLike:
                        postDetails.LikesCount += 1;
                        break;
                    case InteractionType.PostUnlike:
                        postDetails.LikesCount = Math.Max(0, postDetails.LikesCount - 1);
                        break;
                    case InteractionType.PostComment:
                        postDetails.CommentsCount += 1;
                        break;
                    case InteractionType.PostCommentDeleted:
                        postDetails.CommentsCount = Math.Max(0, postDetails.CommentsCount - 1);
                        break;
                }
            }

            await _elasticSearchService.UpdatePostAsync(postId, postDetails);
        }
    }
}