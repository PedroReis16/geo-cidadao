using GeoCidadao.FeedServiceAPI.Models.External;

namespace GeoCidadao.FeedServiceAPI.Contracts
{
    /// <summary>
    /// Cliente HTTP para comunicação com a API de Posts
    /// </summary>
    public interface IPostsApiClient
    {
        /// <summary>
        /// Busca posts de um usuário específico
        /// </summary>
        Task<List<PostDTO>> GetUserPostsAsync(Guid userId, int? itemsCount = null, int? pageNumber = null);

        /// <summary>
        /// Busca todos os posts (feed geral) - assumindo endpoint que retorna posts de todos
        /// </summary>
        Task<List<PostDTO>> GetAllPostsAsync(int? itemsCount = null, int? pageNumber = null);
    }
}
