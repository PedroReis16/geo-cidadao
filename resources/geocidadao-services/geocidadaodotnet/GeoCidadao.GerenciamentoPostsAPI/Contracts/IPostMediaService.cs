
namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostMediaService
    {
        Task DeleteMediaPostAsync(Guid postId, Guid mediaId);
        Task DeletePostMediasAsync(Guid postId);
        Task ReorderPostMediasAsync(Guid postId, List<Guid> mediaIdsInOrder);
        Task UploadPostMediaAsync(Guid postId, int order, IFormFile mediaFile);
    }
}