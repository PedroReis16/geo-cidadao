
namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IMediaService
    {
        Task<List<string>> GetPostMediaKeysAsync(Guid postId);
        Task UploadMediaAsync(Guid postId, Guid mediaId, Stream fileContent, out string fileExtension);
    }
}