using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Contracts
{
    public interface IPostLikeDao : IRepository<PostLike>
    {
        Task<PostLike?> GetLikeByUserAndPostAsync(Guid userId, Guid postId);
        Task<List<PostLike>> GetPostLikesAsync(Guid postId, int? itemsCount = null, int? pageNumber = null);
        Task<int> GetPostLikesCountAsync(Guid postId);
    }
}
