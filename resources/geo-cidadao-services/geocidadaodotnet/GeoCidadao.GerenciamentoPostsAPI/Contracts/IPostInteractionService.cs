using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostInteractionService
    {
        // Like operations
        Task<PostLikeDTO> LikePostAsync(Guid postId, Guid userId);
        Task UnlikePostAsync(Guid postId, Guid userId);

        // Comment operations
        Task<PostCommentDTO> CreateCommentAsync(Guid postId, Guid userId, CreateCommentDTO createCommentDto);
        Task<PostCommentDTO> UpdateCommentAsync(Guid postId, Guid commentId, Guid userId, UpdateCommentDTO updateCommentDto);
        Task DeleteCommentAsync(Guid postId, Guid commentId, Guid userId, bool isModerator = false);
        Task<List<PostCommentDTO>> GetPostCommentsAsync(Guid postId, int? itemsCount = null, int? pageNumber = null);
    }
}
