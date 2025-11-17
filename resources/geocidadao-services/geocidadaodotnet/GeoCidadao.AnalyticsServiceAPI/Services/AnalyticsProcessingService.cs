using GeoCidadao.AMQP.Messages;
using GeoCidadao.AnalyticsServiceAPI.Contracts;
using GeoCidadao.AnalyticsServiceAPI.Database.Contracts;
using GeoCidadao.Models.Entities.AnalyticsServiceAPI;

namespace GeoCidadao.AnalyticsServiceAPI.Services
{
    public class AnalyticsProcessingService : IAnalyticsProcessingService
    {
        private readonly IProblemEventDao _problemEventDao;
        private readonly ILogger<AnalyticsProcessingService> _logger;

        public AnalyticsProcessingService(
            IProblemEventDao problemEventDao,
            ILogger<AnalyticsProcessingService> logger)
        {
            _problemEventDao = problemEventDao;
            _logger = logger;
        }

        public async Task ProcessPostAnalyticsEventAsync(PostAnalyticsMessage message)
        {
            try
            {
                _logger.LogInformation($"Processing analytics event for post {message.PostId}");

                // Check if event already exists (to handle duplicates/updates)
                var existingEvent = await _problemEventDao.GetByPostIdAsync(message.PostId);

                if (existingEvent != null)
                {
                    // Update existing event with latest metrics
                    existingEvent.LikesCount = message.LikesCount;
                    existingEvent.CommentsCount = message.CommentsCount;
                    existingEvent.RelevanceScore = message.RelevanceScore;
                    existingEvent.UpdatedAt = DateTime.UtcNow;

                    await _problemEventDao.UpdateAsync(existingEvent);
                    _logger.LogInformation($"Updated analytics event for post {message.PostId}");
                }
                else
                {
                    // Create new event
                    var problemEvent = new ProblemEvent
                    {
                        Id = Guid.NewGuid(),
                        PostId = message.PostId,
                        Title = message.Title,
                        Description = message.Description,
                        Category = message.Category,
                        Region = message.Region,
                        City = message.City,
                        State = message.State,
                        Latitude = message.Latitude,
                        Longitude = message.Longitude,
                        EventTimestamp = message.EventTimestamp,
                        LikesCount = message.LikesCount,
                        CommentsCount = message.CommentsCount,
                        RelevanceScore = message.RelevanceScore,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _problemEventDao.AddAsync(problemEvent);
                    _logger.LogInformation($"Created new analytics event for post {message.PostId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing analytics event for post {message.PostId}: {ex.Message}");
                throw;
            }
        }
    }
}
