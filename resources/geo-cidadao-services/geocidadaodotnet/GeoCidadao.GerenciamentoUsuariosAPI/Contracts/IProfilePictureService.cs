namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IUserPictureService
    {
        Task DeleteUserPhotoAsync(Guid userId);
        Task<string?> GetUserPhotoUrlAsync(Guid userId);
        Task UpdateUserPhotoAsync(Guid userId, IFormFile photoBase64);
    }
}