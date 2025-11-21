using GeoCidadao.EngagementServiceAPI.Contracts;

namespace GeoCidadao.EngagementServiceAPI.Services
{
    public class PostLikesService(ILogger<PostLikesService> logger) : IPostLikesService
    {
        private readonly ILogger<PostLikesService> _logger = logger;
    }
}