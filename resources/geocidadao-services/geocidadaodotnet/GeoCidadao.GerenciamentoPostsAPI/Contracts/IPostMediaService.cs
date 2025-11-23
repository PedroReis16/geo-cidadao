
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;

namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostMediaService
    {
        Task DeleteMediaPostAsync(Guid postId, Guid mediaId);
        Task DeletePostMediasAsync(Guid postId);
        Task<string> GetPostMediaUrlAsync(Guid postId, Guid mediaId);
        Task ReorderPostMediasAsync(Guid postId, List<Guid> mediaIdsInOrder);
        Task<List<PostMedia>> UploadPostMediasAsync(Guid postId, List<IFormFile> mediaFiles);
    }
}