using GeoCidadao.Database.Contracts;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Database.Contracts
{
    public interface IPostMediaDao : IRepository<PostMedia>
    {
        Task<List<PostMedia>> GetPostMediasAsync(Guid postId);
        Task UpdateMediaOrderAsync(Guid id, int order);
    }
}