namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IUserPictureService
    {
        Task UpdateUserPhotoAsync(Guid userId, IFormFile photoBase64);
    }
}