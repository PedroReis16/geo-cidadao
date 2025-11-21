using GeoCidadao.EngagementServiceAPI.Contracts;

namespace GeoCidadao.EngagementServiceAPI.Services
{
    public class PostCommentsService(ILogger<PostCommentsService> logger) : IPostCommentsService
    {
        private readonly ILogger<PostCommentsService> _logger = logger;
    }
}