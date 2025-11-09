namespace GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI
{
    public class UserPicture : BaseEntity
    {
        public string FileHash { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public UserProfile User { get; set; } = null!;
    }
}