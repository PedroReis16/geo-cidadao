namespace GeoCidadao.Model.Entities
{
    public class UserPicture : BaseEntity
    {
        public string FileHash { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public UserProfile User { get; set; } = null!;
    }
}