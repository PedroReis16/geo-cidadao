namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IPostMediaService
    {
        Task DeleteMediaPostAsync(Guid postId, Guid mediaId);
        Task UploadPostMediaAsync(Guid postId, IFormFile mediaFile);
    }
}