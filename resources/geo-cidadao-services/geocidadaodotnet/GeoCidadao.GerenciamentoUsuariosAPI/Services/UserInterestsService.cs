using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs;
using GeoCidadao.Models.Constants;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Extensions;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services
{
    internal class UserInterestsService : IUserInterestsService
    {
        private readonly ILogger<UserInterestsService> _logger;
        private readonly HttpContext? _httpContext;
        private readonly IUserInterestsDao _interestsDao;
        private readonly IServiceScopeFactory _serviceFactory;

        public UserInterestsService(
            ILogger<UserInterestsService> logger,
            IHttpContextAccessor? contextAccessor,
            IUserInterestsDao interestsDao,
            IServiceScopeFactory serviceFactory)
        {
            _logger = logger;
            _httpContext = contextAccessor?.HttpContext;
            _interestsDao = interestsDao;
            _serviceFactory = serviceFactory;
        }

        public async Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId)
        {
            try
            {
                UserInterests? interests = await _interestsDao.GetByUserIdAsync(userId);

                if (interests == null)
                    return null;

                return MapToDTO(interests);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao obter os interesses do usuário com Id '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    { LogConstants.User, userId }
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task<UserInterestsDTO> CreateUserInterestsAsync(Guid userId, UpdateUserInterestsDTO interestsDTO)
        {
            try
            {
                // Check if interests already exist
                UserInterests? existing = await _interestsDao.GetByUserIdAsync(userId);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Interesses já existem para o usuário '{userId}'");
                }

                UserInterests interests = new()
                {
                    UserId = userId,
                    Region = interestsDTO.Region,
                    City = interestsDTO.City,
                    State = interestsDTO.State,
                    Categories = interestsDTO.Categories
                };

                await _interestsDao.AddAsync(interests);

                NotifyUserChanged(userId);

                return MapToDTO(interests);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao criar os interesses do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    { LogConstants.User, userId }
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task<UserInterestsDTO> UpdateUserInterestsAsync(Guid userId, UpdateUserInterestsDTO interestsDTO)
        {
            try
            {
                UserInterests? interests = await _interestsDao.GetByUserIdAsync(userId);

                if (interests == null)
                {
                    throw new InvalidOperationException($"Interesses não encontrados para o usuário '{userId}'");
                }

                interests.Region = interestsDTO.Region;
                interests.City = interestsDTO.City;
                interests.State = interestsDTO.State;
                interests.Categories = interestsDTO.Categories;
                interests.UpdatedAt = DateTime.UtcNow;

                await _interestsDao.UpdateAsync(interests);

                NotifyUserChanged(userId);

                return MapToDTO(interests);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao atualizar os interesses do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    { LogConstants.User, userId }
                });
                throw new Exception(errorMessage);
            }
        }

        public async Task DeleteUserInterestsAsync(Guid userId)
        {
            try
            {
                UserInterests? interests = await _interestsDao.GetByUserIdAsync(userId);

                if (interests == null)
                {
                    throw new InvalidOperationException($"Interesses não encontrados para o usuário '{userId}'");
                }

                await _interestsDao.DeleteAsync(interests.Id);

                NotifyUserChanged(userId);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Houve um erro ao excluir os interesses do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(ex, errorMessage, context: _httpContext, additionalProperties: new Dictionary<string, object>
                {
                    { LogConstants.User, userId }
                });
                throw new Exception(errorMessage);
            }
        }

        private void NotifyUserChanged(Guid userId)
        {
            using IServiceScope scope = _serviceFactory.CreateScope();
            INotifyUserChangedService? notifyService = scope.ServiceProvider.GetService<INotifyUserChangedService>();

            if (notifyService != null)
                notifyService.NotifyUserChanged(userId);
        }

        private static UserInterestsDTO MapToDTO(UserInterests interests)
        {
            return new UserInterestsDTO
            {
                Id = interests.Id,
                UserId = interests.UserId,
                Region = interests.Region,
                City = interests.City,
                State = interests.State,
                Categories = interests.Categories
            };
        }
    }
}
