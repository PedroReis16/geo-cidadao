using GeoCidadao.FeedServiceAPI.Models.DTOs;

namespace GeoCidadao.FeedServiceAPI.Contracts
{
    /// <summary>
    /// Serviço de Feed de Usuários
    /// </summary>
    public interface IFeedService
    {
        /// <summary>
        /// Obtém o feed geral de posts
        /// </summary>
        Task<List<FeedPostDTO>> GetFeedAsync(int pageSize = 20, int page = 1);

        /// <summary>
        /// Obtém o feed de posts de um usuário específico
        /// </summary>
        Task<List<FeedPostDTO>> GetUserFeedAsync(Guid userId, int pageSize = 20, int page = 1);

        /// <summary>
        /// Invalida o cache do feed
        /// </summary>
        Task InvalidateFeedCacheAsync();
    }
}
