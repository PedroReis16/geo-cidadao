using GeoCidadao.Caching.Contracts;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts.CacheServices
{
    public interface IUserPictureCacheService : IInMemoryCacheService
    {
        void AddPictureUrl(Guid userId, string presignedUrl);
        string? GetPictureUrl(Guid userId);
        void RemovePictureUrl(Guid userId);
    }
}