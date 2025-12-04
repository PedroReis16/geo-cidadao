using System.Collections.Concurrent;
using GeoCidadao.FeedMapAPI.Contracts;
using GeoCidadao.FeedMapAPI.Database.Documents;
using GeoCidadao.FeedMapAPI.Models.DTOs;

namespace GeoCidadao.FeedMapAPI.Services
{
    public class MapService(ILogger<MapService> logger, IServiceScopeFactory scopeFactory) : IMapService
    {
        private readonly ILogger<MapService> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task<List<MapPostDTO>> GetPostsInBoundsAsync(
            double topLeftLat, 
            double topLeftLon, 
            double bottomRightLat, 
            double bottomRightLon, 
            int zoomLevel,
            int limit = 100)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                IPostsDaoService postsDaoService = scope.ServiceProvider.GetRequiredService<IPostsDaoService>();

                // Calcula o threshold de relevância baseado no zoom
                // Quanto menor o zoom (mais afastado), maior o threshold
                double relevanceThreshold = CalculateRelevanceThreshold(zoomLevel);
                
                // Calcula o limite de posts baseado no zoom
                // Quanto menor o zoom, menos posts (apenas os mais relevantes)
                int adjustedLimit = CalculatePostLimit(zoomLevel, limit);

                List<PostDocument> posts = await postsDaoService.GetPostsInBoundsAsync(
                    topLeftLat, 
                    topLeftLon, 
                    bottomRightLat, 
                    bottomRightLon,
                    relevanceThreshold,
                    adjustedLimit);

                ConcurrentBag<MapPostDTO> results = new();

                Parallel.ForEach(posts, post =>
                {
                    MapPostDTO result = new()
                    {
                        Id = post.Id,
                        Content = post.Content,
                        Author = new AuthorDTO
                        {
                            Id = post.PostOwnerId,
                            Name = post.AuthorName!,
                            Username = post.AuthorUsername!,
                            ProfilePictureUrl = post.AuthorProfilePicture
                        },
                        Location = new LocationDTO
                        {
                            Latitude = post.Latitude!.Value,
                            Longitude = post.Longitude!.Value,
                            City = post.City ?? string.Empty
                        },
                        LikesCount = post.LikesCount,
                        CommentsCount = post.CommentsCount,
                        FirstMediaUrl = post.MediaUrls?.FirstOrDefault(),
                        RelevanceScore = post.RelevanceScore
                    };

                    results.Add(result);
                });

                // Ordena por relevância (maior primeiro)
                return results.OrderByDescending(p => p.RelevanceScore).ToList();
            }
            catch (Exception ex)
            {
                string errorMessage = "Ocorreu um erro ao obter os dados para o feed do mapa";
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }

        /// <summary>
        /// Calcula o threshold de relevância baseado no nível de zoom
        /// Zoom baixo (0-5): apenas posts muito relevantes
        /// Zoom médio (6-12): posts moderadamente relevantes
        /// Zoom alto (13+): todos os posts acima do mínimo
        /// </summary>
        private static double CalculateRelevanceThreshold(int zoomLevel)
        {
            return zoomLevel switch
            {
                <= 5 => 0.8,    // Muito zoomed out - apenas os mais relevantes
                <= 8 => 0.6,    // Zoomed out - posts bem relevantes
                <= 12 => 0.4,   // Zoom médio - posts moderadamente relevantes
                <= 15 => 0.2,   // Zoom in - maioria dos posts
                _ => 0.0        // Muito zoomed in - todos os posts
            };
        }

        /// <summary>
        /// Calcula o limite de posts baseado no nível de zoom
        /// Quanto menor o zoom, menos posts para evitar poluição visual
        /// </summary>
        private static int CalculatePostLimit(int zoomLevel, int maxLimit)
        {
            int calculatedLimit = zoomLevel switch
            {
                <= 5 => 20,     // Muito zoomed out - poucos pontos
                <= 8 => 50,     // Zoomed out - quantidade moderada
                <= 12 => 100,   // Zoom médio - boa quantidade
                <= 15 => 200,   // Zoom in - muitos pontos
                _ => 300        // Muito zoomed in - máximo de pontos
            };

            return Math.Min(calculatedLimit, maxLimit);
        }
    }
}