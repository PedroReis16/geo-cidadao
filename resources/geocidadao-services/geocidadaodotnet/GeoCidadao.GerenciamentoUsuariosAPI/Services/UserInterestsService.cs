using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.Models.Extensions;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services
{
    internal class UserInterestsService(ILogger<UserInterestsService> logger, IHttpContextAccessor contextAccessor, IServiceScopeFactory scopeFactory) : IUserInterestsService
    {
        private readonly ILogger<UserInterestsService> _logger = logger;
        private readonly HttpContext? _httpContext = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        private IUserInterestsDao _userInterestsDao
        {
            get
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                return scope.ServiceProvider.GetRequiredService<IUserInterestsDao>();
            }
        }

        public async Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId)
        {
            try
            {
                UserInterests? interests = await _userInterestsDao.FindAsync(userId);

                if (interests == null)
                    return null;

                return new UserInterestsDTO(interests);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao consultar as preferências de postagem do usuário com Id '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    { "User", userId }
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task UpdateUserFollowedCategoriesAsync(Guid userId, List<PostCategory> categories)
        {
            try
            {
                await _userInterestsDao.UpdateFollowedCategoriesAsync(userId, categories);

                NotifyInterestChanged(userId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao atualizar as categorias seguidas pelo usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.UserId, userId },
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task UpdateUserFollowedCitiesAsync(Guid userId, string city)
        {
            try
            {
                await _userInterestsDao.UpdateFollowedCitiesAsync(userId, city);

                NotifyInterestChanged(userId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao atualizar as cidades seguidas pelo usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.UserId, userId },
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task UpdateUserFollowedDistrictsAsync(Guid userId, string district)
        {
            try
            {
                await _userInterestsDao.UpdateFollowedDistrictsAsync(userId, district);

                NotifyInterestChanged(userId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao atualizar os bairros de interesse do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.UserId, userId },
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task UpdateUserFollowedUsersAsync(Guid userId, Guid followedUserId)
        {
            try
            {
                await _userInterestsDao.UpdateFollowedUsersAsync(userId, followedUserId);

                NotifyInterestChanged(userId);
            }
            catch (EntityValidationException) { throw; }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao atualizar os usuários seguidos pelo usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.UserId, userId },
                });
                throw new Exception(errorMessage);
            }
        }

        private void NotifyInterestChanged(Guid userId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            INotifyUserChangedService userChangedNotification = scope.ServiceProvider.GetRequiredService<INotifyUserChangedService>();
            userChangedNotification.NotifyUserChanged(userId);
        }
    }
}
