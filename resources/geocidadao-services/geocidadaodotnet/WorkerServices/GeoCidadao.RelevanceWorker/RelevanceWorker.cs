using GeoCidadao.RelevanceWorker.Contracts.QueueServices;

namespace GeoCidadao.RelevanceWorker;

public class RelevanceWorker(ILogger<RelevanceWorker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly ILogger<RelevanceWorker> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Relevance is running at: {time}", DateTimeOffset.Now);

                using IServiceScope scope = _scopeFactory.CreateScope();

                IPostDeletedQueueService postDeletedQueueService = scope.ServiceProvider.GetRequiredService<IPostDeletedQueueService>();
                IPostInteractQueueService postInteractQueueService = scope.ServiceProvider.GetRequiredService<IPostInteractQueueService>();

                postDeletedQueueService.ConsumeQueue();
                postInteractQueueService.ConsumeQueue();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in RelevanceWorker: {message}", ex.Message);
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}