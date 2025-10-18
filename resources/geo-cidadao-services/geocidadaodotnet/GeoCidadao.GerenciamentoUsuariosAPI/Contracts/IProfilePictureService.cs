namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IProfilePictureService
    {
        Task UpdateUserPhotoAsync(Guid userId, IFormFile photoBase64);
    }
}