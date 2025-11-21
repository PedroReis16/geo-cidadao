using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Contracts
{
    public interface IPostDao : IRepository<Post>
    {
        Task<List<Post>> GetUserContentAsync(Guid userId);
        Task<List<Post>> GetUserPostsAsync(Guid userId, int? itemsCount, int? pageNumber);
        Task DeleteUserPostsAsync(Guid userId);
    }
}