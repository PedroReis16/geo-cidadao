using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.ConnectionServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using GeoCidadao.Model.Extensions;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services
{
    internal class UserProfileService(ILogger<UserProfileService> logger,
                                      IHttpContextAccessor? contextAccessor,
                                      IServiceScopeFactory serviceFactory,
                                      IKeycloakService keycloakService,
                                      IUserCacheService cacheService) : IUserProfileService
    {
        private readonly ILogger<UserProfileService> _logger = logger;
        private readonly IHttpContextAccessor? _contextAccessor = contextAccessor;
        private readonly IServiceScopeFactory _serviceFactory = serviceFactory;
        private readonly IKeycloakService _keycloakService = keycloakService;
        private readonly IUserCacheService _cacheService = cacheService;

        public async Task<UserDTO?> GetUserProfileAsync(Guid userId)
        {
            try
            {
                UserDTO? user = _cacheService.GetUser(userId);

                if (user != null)
                    return user;

                user = await _keycloakService.GetUserAsync(userId);

                if (user == null)
                    return null;

                _cacheService.AddUserCache(userId, user);
                return user;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao obter o perfil de usuário com Id '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex);
                throw new Exception(errorMessage);
            }
        }

        public async Task UpdateUserProfileAsync(Guid userId, UpdateUserDTO updatedProfile)
        {
            try
            {
                await _keycloakService.UpdateUserAsync(userId, updatedProfile);
                
                _cacheService.RemoveUser(userId);

            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao atualizar o perfil de usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex);
                throw new Exception(errorMessage);
            }
        }

        public async Task DeleteUserProfileAsync(Guid userId)
        {
            try
            {
                await _keycloakService.DeleteUserAsync(userId);
                _cacheService.RemoveUser(userId);

            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao excluir o perfil de usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(errorMessage, ex);
                throw new Exception(errorMessage);
            }
        }
    }
}