using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.EngagementServiceAPI;

namespace GeoCidadao.EngagementServiceAPI.Database.Contracts
{
    public interface IPostCommentsDao : IRepository<PostComment>
    {
        Task DeletePostCommentsAsync(Guid postId);
        Task<PostComment?> FindAsync(Guid postId, Guid commentId, bool v);
        Task<List<PostComment>> GetPostCommentsAsync(Guid postId, int? itemsCount, int? pageNumber);
        Task RemoveUserCommentsAsync(Guid userId);
    }
}