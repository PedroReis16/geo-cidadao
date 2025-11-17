namespace GeoCidadao.GerenciamentoPostsAPI.Contracts.QueueServices
{
    public interface INotifyPostAnalyticsService
    {
        Task NotifyPostAnalyticsAsync(Guid postId);
    }
}
