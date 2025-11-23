using GeoCidadao.Models.Enums;

namespace GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI
{
    public class UserInterests : BaseEntity
    {
        public List<PostCategory> FollowedCategories { get; set; } = Enum.GetValues<PostCategory>().ToList();

        public List<Guid> FollowedUsers { get; set; } = new(); // ID of other users
        public List<string> FollowedCities { get; set; } = new(); // Cidades de interesse
        public List<string> FollowedDistricts { get; set; } = new();// Bairros de interesse
        public int InterestRange { get; set; } = 5; // Raio de interesse em km

        public UserProfile User { get; set; } = null!;
    }
}
