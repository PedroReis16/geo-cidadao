using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.Contracts
{
    public interface IPostLikesDao : IRepository<PostLike>
    {
        Task RemovePostLikeAsync(Guid postId, Guid userId);
    }
}