
using GeoCidadao.EngagementServiceAPI.Models.DTOs;

namespace GeoCidadao.EngagementServiceAPI.Contracts
{
    public interface IPostInteractionService
    {
        Task LikePostAsync(Guid postId, Guid userId);
        Task ReportPostAsync(Guid postId, Guid userId, DelationDTO delationDetails);
        Task UnlikePostAsync(Guid postId, Guid userId);
        Task<List<Guid>> GetLikedPostIdsAsync(Guid userId, List<Guid> postIds);
    }
}