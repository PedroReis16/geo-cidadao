using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Contracts
{
    public interface IPostCommentDao : IRepository<PostComment>
    {
        Task<List<PostComment>> GetPostCommentsAsync(Guid postId, int? itemsCount = null, int? pageNumber = null);
        Task<int> GetPostCommentsCountAsync(Guid postId);
    }
}
