using GeoCidadao.Cloud.Contracts;
using GeoCidadao.Cloud.Models.BucketRequests;
using GeoCidadao.GerenciamentoUsuariosAPI.Config;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.QueueServices;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.Contracts;
using GeoCidadao.Model.Constants;
using GeoCidadao.Model.Entities;
using GeoCidadao.Model.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Model.Enums;
using GeoCidadao.Model.Exceptions;
using GeoCidadao.Model.Extensions;
using GeoCidadao.Model.Helpers;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services
{
    internal class UserPictureService(
        ILogger<UserPictureService> logger,
        IHttpContextAccessor? contextAccessor,
        IServiceScopeFactory serviceFactory,
        IUserPictureDao pictureDao,
        ICloudBucketService bucketService
        ) : IUserPictureService
    {
        private readonly ILogger<UserPictureService> _logger = logger;
        private readonly HttpContext? _contextAccessor = contextAccessor?.HttpContext;
        private readonly IServiceScopeFactory _serviceFactory = serviceFactory;
        private readonly IUserPictureDao _pictureDao = pictureDao;
        private readonly ICloudBucketService _bucketService = bucketService;
        private readonly string _pictureFolder = serviceFactory.CreateScope().ServiceProvider
            .GetRequiredService<IConfiguration>()
            .GetValue<string>(AppSettingsProperties.ProfileMediaFolder)!;

        public async Task<string?> GetUserPhotoUrlAsync(Guid userId)
        {
            try
            {
                using IServiceScope scope = _serviceFactory.CreateScope();
                IUserPictureCacheService pictureCacheService = scope.ServiceProvider.GetRequiredService<IUserPictureCacheService>();

                string? cachedUrl = pictureCacheService.GetPictureUrl(userId);

                if (!string.IsNullOrWhiteSpace(cachedUrl))
                    return cachedUrl;

                UserPicture? picture = await _pictureDao.FindAsync(userId);

                if (picture == null)
                    return null;

                string objectKey = $"{_pictureFolder}/{userId}{picture.FileExtension}";
                string url = await _bucketService.GetPreSignedUrlAsync(new GetPreSignedUrlRequest
                {
                    ObjectKey = objectKey
                });

                pictureCacheService.AddPictureUrl(userId, url);
                return url;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Houve um erro ao tentar obter a URL da foto de perfil do usuário: {ex.GetFullMessage()}";
                _logger.LogError(message: errorMsg, exception: ex, context: _contextAccessor, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.EntityId, userId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        public async Task UpdateUserPhotoAsync(Guid userId, IFormFile photoFile)
        {
            try
            {
                using IServiceScope scope = _serviceFactory.CreateScope();
                IUserProfileDao userProfileDao = scope.ServiceProvider.GetRequiredService<IUserProfileDao>();
                string objectPrefix = $"{_pictureFolder}/{userId}";

                using Stream fileContent = photoFile.OpenReadStream();

                if (photoFile.Length == 0)
                    throw new UserException(500, "A foto enviada está vazia", ErrorCodes.INVALID_MEDIA);

                UserProfile? userProfile = await userProfileDao.FindAsync(userId);

                if (userProfile == null)
                    throw new UserException(404, "Perfil de usuário não encontrado", ErrorCodes.USER_NOT_FOUND);

                UserPicture? userPicture = await _pictureDao.FindAsync(userId);

                byte[] buffer = new byte[8];
                fileContent.Read(buffer, 0, 8);
                fileContent.Position = 0;

                if (!buffer.Take(3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }) && !buffer.Take(8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                    throw new UserException("Formato inválido de foto de perfil", ErrorCodes.INVALID_MEDIA);

                string fileHash = FileHelpers.GetFileHash(fileContent);

                if (userPicture == null)
                {
                    await _bucketService.PutObjectAsync(new PutObjectRequest
                    {
                        ObjectKey = $"{objectPrefix}{Path.GetExtension(photoFile.FileName).ToLowerInvariant()}",
                        FileContent = fileContent,
                    });
                }
                else
                {
                    if (userPicture.FileHash == fileHash)
                        return;

                    await Task.WhenAll(
                        _bucketService.DeleteObjectAsync(new DeleteObjectRequest
                        {
                            ObjectKey = $"{objectPrefix}{userPicture.FileExtension}"
                        }),
                        _bucketService.PutObjectAsync(new PutObjectRequest
                        {
                            ObjectKey = $"{objectPrefix}{Path.GetExtension(photoFile.FileName).ToLowerInvariant()}",
                            FileContent = fileContent,
                        })
                    );
                }

                await UpdatePhotoAsync(userId, Path.GetExtension(photoFile.FileName).ToLowerInvariant(), fileHash);
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

        private async Task UpdatePhotoAsync(Guid userId, string pictureExtension, string fileHash)
        {
            await _pictureDao.AddOrUpdatePictureAsync(userId, pictureExtension, fileHash);
            NotifyUserPhotoChanged(userId);
        }

        public async Task DeleteUserPhotoAsync(Guid userId)
        {
            try
            {
                using IServiceScope scope = _serviceFactory.CreateScope();
                string objectPrefix = $"{_pictureFolder}/{userId}";

                UserPicture? userPicture = await _pictureDao.FindAsync(userId);

                if (userPicture == null)
                    return;

                await _bucketService.DeleteObjectAsync(new DeleteObjectRequest
                {
                    ObjectKey = $"{objectPrefix}{userPicture.FileExtension}"
                });

                await _pictureDao.DeleteAsync(userPicture);

                NotifyUserPhotoChanged(userId);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Houve um erro ao tentar deletar a foto de perfil do usuário '{userId}': {ex.GetFullMessage()}";
                _logger.LogError(message: errorMsg, exception: ex, context: _contextAccessor, additionalProperties: new Dictionary<string, object>
                {
                    {LogConstants.EntityId, userId },
                });
                throw new Exception(errorMsg, ex);
            }
        }

        private void NotifyUserPhotoChanged(Guid userId)
        {
            using IServiceScope scope = _serviceFactory.CreateScope();
            INotifyUserChangedService notifyService = scope.ServiceProvider.GetRequiredService<INotifyUserChangedService>();
            IUserPictureCacheService pictureCacheService = scope.ServiceProvider.GetRequiredService<IUserPictureCacheService>();

            pictureCacheService.RemovePictureUrl(userId);

            notifyService.NotifyUserPhotoChanged(userId);
        }

    }
}