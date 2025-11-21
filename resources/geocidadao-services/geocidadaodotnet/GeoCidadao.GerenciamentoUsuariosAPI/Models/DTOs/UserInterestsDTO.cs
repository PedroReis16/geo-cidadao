using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs
{
    public class UserInterestsDTO
    {
        public Guid UserId { get; set; }
        public List<PostCategory> Categories { get; set; } = new();
        public List<Guid>? Users { get; set; }
        public List<string>? Cities { get; set; }
        public List<string>? Districts { get; set; }

        public UserInterestsDTO()
        {

        }

        public UserInterestsDTO(UserInterests interests)
        {
            UserId = interests.User.Id;
            Categories = interests.FollowedCategories;
            Users = interests.FollowedUsers?.ToList();
            Cities = interests.FollowedCities?.ToList();
            Districts = interests.FollowedDistricts?.ToList();
        }
    }
}
