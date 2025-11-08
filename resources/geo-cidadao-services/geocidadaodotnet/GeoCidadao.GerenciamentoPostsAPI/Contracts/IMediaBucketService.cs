
namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IMediaBucketService
    {
        Task DeleteMediaAsync(string mediaKey);
        Task<List<string>> GetPostMediaKeysAsync(Guid postId);
        Task UploadMediaAsync(Guid postId, Guid mediaId, Stream fileContent, out string fileExtension);
    }
}