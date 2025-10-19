using GeoCidadao.Cloud.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Config;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Model.Constants;
using GeoCidadao.Model.Entities;
using GeoCidadao.Model.Enums;
using GeoCidadao.Model.Exceptions;
using GeoCidadao.Model.Extensions;
using GeoCidadao.Model.Helpers;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services
{
    public class UserPictureService(
        ILogger<UserPictureService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory serviceFactory,
        IUserProfileDao userDao
        ) : IUserPictureService
    {
        private readonly ILogger<UserPictureService> _logger = logger;
        private readonly HttpContext? _contextAccessor = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _serviceFactory = serviceFactory;
        private readonly IUserProfileDao _userDao = userDao;


        public async Task UpdateUserPhotoAsync(Guid userId, IFormFile photoFile)
        {
            try
            {
                using Stream fileContent = photoFile.OpenReadStream();

                if (photoFile.Length == 0)
                    throw new UserException(500, "A foto enviada está vazia", ErrorCodes.INVALID_MEDIA);

                UserProfile? userProfile = await _userDao.FindAsync(userId);

                if (userProfile == null)
                    throw new UserException(404, "Perfil de usuário não encontrado", ErrorCodes.USER_NOT_FOUND);

                byte[] buffer = new byte[8];
                fileContent.Read(buffer, 0, 8);
                fileContent.Position = 0;

                if (!buffer.Take(3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }) && !buffer.Take(8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                    throw new UserException("Formato inválido de foto de perfil", ErrorCodes.INVALID_MEDIA);

                string fileHash = FileHelpers.GetFileHash(fileContent);

                // if (userProfile.ProfilePictureHash == null)
                // {
                //     string objectKey = $"{AppSettingsProperties.ProfileMediaFolder}/{userId}.{Path.GetExtension(photoFile.FileName).TrimStart('.')}";
                //     _ = Task.WhenAll(
                //         _cloudService.PutObjectAsync(new()
                //         {
                //             ObjectKey = objectKey,
                //             FileContent = fileContent
                //         }),
                //         UpdatePhotoAsync(userId, objectKey, fileHash)
                //     );
                // }

            }
            catch (UserException) { throw; }
            catch (Exception ex)
            {
                string errorMsg = $"Houve um erro ao tentar atualizar a foto de perfil do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(message: errorMsg, exception: ex, context: _contextAccessor, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.EntityId, userId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        private async Task UpdatePhotoAsync(Guid userId, string objectKey, string fileHash)
        {
            using IServiceScope scope = _serviceFactory.CreateScope();
            INotifyUserChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyUserChangedService>();
            IUserPictureCacheService pictureCacheService = scope.ServiceProvider.GetRequiredService<IUserPictureCacheService>();

            await _userDao.UpdateUserPictureAsync(userId, objectKey, fileHash);

            pictureCacheService.RemovePictureUrl(userId);

            notifyService.NotifyUserPhotoChanged(userId);
        }
    }
}