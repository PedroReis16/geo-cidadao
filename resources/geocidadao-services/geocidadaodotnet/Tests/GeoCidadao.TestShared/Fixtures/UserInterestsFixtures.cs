using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;

namespace GeoCidadao.TestShared.Fixtures
{
    public class UserInterestsFixtures
    {
        public static UserInterests CreateUserInterests(Guid? userId = null)
        {
            userId ??= Guid.NewGuid();

            return new()
            {
                Id = userId.Value,
                User = new UserProfile(){ Id = userId.Value },
            };
        }
    }
}