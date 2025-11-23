namespace GeoCidadao.FeedServiceAPI.Contracts
{
    public interface ISeenPostsService
    {
        Task MarkPostAsSeenAsync(Guid userId, Guid postId);
        Task<List<Guid>> GetSeenPostIdsAsync(Guid userId);
    }
}
