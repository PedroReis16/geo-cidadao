using GeoCidadao.Caching.Config;
using GeoCidadao.Caching.Services;
using GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices;
using Microsoft.Extensions.Caching.Memory;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Services.CacheServices
{
    internal class UserPictureCacheService(InMemoryCacheConfig cacheConfig, IMemoryCache memoryCache) : InMemoryCacheService(cacheConfig, memoryCache), IUserPictureCacheService
    {
        public void AddPictureUrl(Guid userId, string presignedUrl) => base.Add(userId.ToString(), presignedUrl);
        public string? GetPictureUrl(Guid userId) => base.Get(userId.ToString()) as string;
        public void RemovePictureUrl(Guid userId) => base.Remove(userId.ToString());
    }
}