using GeoCidadao.Caching.Contracts;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.CacheServices
{
    public interface IPostMediasCacheService : IInMemoryCacheService
    {
        void AddPostMedia(Guid postId, Guid mediaId, string mediaUrl);
        string? GetPostMediaUrl(Guid postId, Guid mediaId);
        void RemovePostMedia(Guid postId, Guid mediaId);
    }
}