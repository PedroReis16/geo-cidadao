
using GeoCidadao.EngagementServiceAPI.Models.DTOs;
using GeoCidadao.EngagementServiceAPI.Models.DTOs.Comments;

namespace GeoCidadao.EngagementServiceAPI.Contracts
{
    public interface IPostCommentsService
    {
        Task<CommentDTO> AddPostCommentAsync(Guid postId, Guid userId, NewCommentDTO newComment, CancellationToken cancellationToken);
        Task DeletePostCommentAsync(Guid postId, Guid commentId, CancellationToken cancellationToken);
        Task<List<CommentDTO>?> GetPostCommentsAsync(Guid postId, int? itemsCount, int? pageNumber, CancellationToken cancellationToken);
        Task LikePostCommentAsync(Guid postId, Guid commentId, Guid userId, CancellationToken cancellationToken);
        Task UnlikePostCommentAsync(Guid postId, Guid commentId, Guid userId, CancellationToken cancellationToken);
        Task<CommentDTO> UpdatePostCommentAsync(Guid postId, Guid commentId, UpdateCommentDTO updatedComment, CancellationToken cancellationToken);
    }
}