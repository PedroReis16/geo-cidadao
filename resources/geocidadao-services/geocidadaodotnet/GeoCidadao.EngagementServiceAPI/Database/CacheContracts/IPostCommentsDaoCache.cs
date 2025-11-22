using GeoCidadao.Database.CacheContracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.CacheContracts
{
    public interface IPostCommentsDaoCache : IRepositoryCache<PostComment>
    {
        PostComment? GetPostComment(Guid commentId, Guid postId);
    }
}