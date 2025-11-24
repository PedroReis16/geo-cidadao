using GeoCidadao.FeedMapAPI.Contracts;

namespace GeoCidadao.FeedMapAPI.Services
{
    public class MapService(ILogger<MapService> logger) : IMapService
    {
        private readonly ILogger<MapService> _logger = logger;  
    }
}