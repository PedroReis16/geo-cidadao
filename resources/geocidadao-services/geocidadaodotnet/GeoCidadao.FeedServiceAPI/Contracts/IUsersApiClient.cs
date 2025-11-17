using GeoCidadao.FeedServiceAPI.Models.External;

namespace GeoCidadao.FeedServiceAPI.Contracts
{
    /// <summary>
    /// Cliente HTTP para comunicação com a API de Usuários
    /// </summary>
    public interface IUsersApiClient
    {
        /// <summary>
        /// Busca informações de um usuário específico
        /// </summary>
        Task<UserDTO?> GetUserProfileAsync(Guid userId);

        /// <summary>
        /// Busca informações de múltiplos usuários
        /// </summary>
        Task<Dictionary<Guid, UserDTO>> GetUserProfilesAsync(List<Guid> userIds);
    }
}
