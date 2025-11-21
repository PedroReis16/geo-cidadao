namespace GeoCidadao.GerenciamentoPostsAPI.Contracts
{
    public interface IUserPostService
    {
        Task RemoveAllUserContentAsync(Guid userId);
    }
}