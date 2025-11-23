using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.GerenciamentoPostsAPI.Contracts.CacheServices;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.GerenciamentoPostsAPI.Services.CacheServices
{
    public class PostMediasCacheService(InMemoryCacheConfig config, IMemoryCache memoryCache) : InMemoryCacheService(config, memoryCache), IPostMediasCacheService
    {
        private static string GetPostMediaKey(Guid postId, Guid mediaId) => $"PostMedia_{postId}_{mediaId}";

        public void AddPostMedia(Guid postId, Guid mediaId, string mediaUrl) => base.Add(GetPostMediaKey(postId, mediaId), mediaUrl);

        public string? GetPostMediaUrl(Guid postId, Guid mediaId) => base.Get(GetPostMediaKey(postId, mediaId)) as string;

        public void RemovePostMedia(Guid postId, Guid mediaId) => base.Remove(GetPostMediaKey(postId, mediaId));
    }
}