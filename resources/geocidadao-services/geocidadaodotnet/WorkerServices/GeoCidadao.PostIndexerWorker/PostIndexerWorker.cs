using GeoCidadao.PostIndexerWorker.Contracts.QueueServices;

namespace GeoCidadao.PostIndexerWorker
{
    public class PostIndexerWorker(ILogger<PostIndexerWorker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly ILogger<PostIndexerWorker> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("PostIndexerWorker is running at: {time}", DateTimeOffset.Now);

                    using IServiceScope scope = _scopeFactory.CreateScope();

                    INewPostQueueService newPostQueueService = scope.ServiceProvider.GetRequiredService<INewPostQueueService>();
                    IPostDeletedQueueService postDeletedQueueService = scope.ServiceProvider.GetRequiredService<IPostDeletedQueueService>();
                    IPostInterectedQueueService postInterectedQueueService = scope.ServiceProvider.GetRequiredService<IPostInterectedQueueService>();

                    newPostQueueService.ConsumeQueue();
                    postDeletedQueueService.ConsumeQueue();
                    postInterectedQueueService.ConsumeQueue();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in PostIndexerWorker: {message}", ex.Message);
                }
                finally
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}