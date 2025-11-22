using GeoCidadao.Caching.Contracts;
using GeoCidadao.Database.Cache;
using GeoCidadao.EngagementServiceAPI.Database.CacheContracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.Cache
{
    public class PostCommentsDaoCache(IInMemoryCacheService cacheService) : RepositoryCache<PostComment>(cacheService), IPostCommentsDaoCache
    {
        private static string GetCacheKey(PostComment post) => $"{nameof(PostComment)}-{post.Id}-{post.PostId}";

        public override void AddEntity(PostComment entity)
        {
            base.AddEntity(GetCacheKey(entity), entity);
        }

        public override void RemoveEntity(params PostComment[] entities)
        {
            foreach (PostComment post in entities)
            {
                base.RemoveEntity(GetCacheKey(post));
            }
        }

        public PostComment? GetPostComment(Guid commentId, Guid postId)
        {
            return GetEntity($"{nameof(PostComment)}-{commentId}-{postId}");
        }
    }
}