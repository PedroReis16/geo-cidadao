using GeoCidadao.EngagementServiceAPI.Contracts;

namespace GeoCidadao.EngagementServiceAPI.Services
{
    public class PostInteractionService(ILogger<PostInteractionService> logger) : IPostInteractionService
    {
        private readonly ILogger<PostInteractionService> _logger = logger;
    }
}