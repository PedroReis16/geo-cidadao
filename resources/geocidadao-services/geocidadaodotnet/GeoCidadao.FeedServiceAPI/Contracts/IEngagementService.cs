namespace GeoCidadao.FeedServiceAPI.Contracts
{
    public interface IEngagementService
    {
        Task<List<Guid>> GetLikedPostIdsAsync(Guid userId, List<Guid> postIds);
    }
}
