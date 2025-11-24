using GeoCidadao.FeedServiceAPI.Models.DTOs;

namespace GeoCidadao.FeedServiceAPI.Contracts
{
    public interface IFeedService
    {
        Task<List<PostDTO>> GetFeedAsync(Guid userId, int page, int pageSize);
    }
}