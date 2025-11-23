using GeoCidadao.FeedServiceAPI.Model;

namespace GeoCidadao.FeedServiceAPI.Contracts
{
    public interface IUserInterestsService
    {
        Task<UserInterestsDTO?> GetUserInterestsAsync(Guid userId);
    }
}
