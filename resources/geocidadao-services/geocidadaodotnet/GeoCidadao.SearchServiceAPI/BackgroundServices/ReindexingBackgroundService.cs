using GeoCidadao.SearchServiceAPI.Services.Contracts;

namespace GeoCidadao.SearchServiceAPI.BackgroundServices;

/// <summary>
/// Background service to periodically reindex all posts and users
/// </summary>
public class ReindexingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReindexingBackgroundService> _logger;
    private readonly TimeSpan _reindexInterval;

    public ReindexingBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ReindexingBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        
        // Default to reindex every 24 hours
        var intervalHours = configuration.GetValue<int?>("ReindexIntervalHours") ?? 24;
        _reindexInterval = TimeSpan.FromHours(intervalHours);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reindexing background service started. Interval: {Interval} hours", _reindexInterval.TotalHours);

        // Wait for 5 minutes before first reindex to allow system to stabilize
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting periodic reindex");
                
                using var scope = _scopeFactory.CreateScope();
                var postDataService = scope.ServiceProvider.GetRequiredService<IPostDataService>();
                var userDataService = scope.ServiceProvider.GetRequiredService<IUserDataService>();
                var indexService = scope.ServiceProvider.GetRequiredService<IElasticsearchIndexService>();

                // Reindex posts
                try
                {
                    var posts = await postDataService.GetAllPostsAsync();
                    var postsCount = posts.Count();
                    
                    if (postsCount > 0)
                    {
                        await indexService.BulkIndexPostsAsync(posts);
                        _logger.LogInformation("Successfully reindexed {Count} posts", postsCount);
                    }
                    else
                    {
                        _logger.LogInformation("No posts to reindex");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reindexing posts");
                }

                // Reindex users
                try
                {
                    var users = await userDataService.GetAllUsersAsync();
                    var usersCount = users.Count();
                    
                    if (usersCount > 0)
                    {
                        await indexService.BulkIndexUsersAsync(users);
                        _logger.LogInformation("Successfully reindexed {Count} users", usersCount);
                    }
                    else
                    {
                        _logger.LogInformation("No users to reindex");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reindexing users");
                }

                _logger.LogInformation("Periodic reindex completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during periodic reindex");
            }

            // Wait for the next reindex interval
            await Task.Delay(_reindexInterval, stoppingToken);
        }
    }
}
