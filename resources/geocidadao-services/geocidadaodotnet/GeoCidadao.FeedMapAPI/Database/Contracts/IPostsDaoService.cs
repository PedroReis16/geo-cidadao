using GeoCidadao.FeedMapAPI.Database.Documents;
using GeoCidadao.FeedMapAPI.Models.DTOs.UserManagement;

namespace GeoCidadao.FeedMapAPI.Contracts
{
    public interface IPostsDaoService
    {
        Task<List<PostDocument>> GetPostsAsync(UserInterestsDTO interests, List<Guid> viewedPosts, int page, int pageSize);
    }
}