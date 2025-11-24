using GeoCidadao.FeedServiceAPI.Database.Documents;
using GeoCidadao.FeedServiceAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedServiceAPI.Contracts
{
    public interface IPostsDaoService
    {
        Task<List<PostDocument>> GetPostsAsync(UserInterestsDTO interests, List<Guid> viewedPosts, int page, int pageSize);
    }
}