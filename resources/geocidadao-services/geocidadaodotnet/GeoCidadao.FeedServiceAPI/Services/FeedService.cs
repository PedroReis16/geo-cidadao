using GeoCidadao.Caching.Contracts;
using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Models.DTOs;
using GeoCidadao.FeedServiceAPI.Models.External;

namespace GeoCidadao.FeedServiceAPI.Services
{
    /// <summary>
    /// Serviço de Feed de Usuários com suporte a cache
    /// </summary>
    public class FeedService : IFeedService
    {
        private readonly IPostsApiClient _postsApiClient;
        private readonly IUsersApiClient _usersApiClient;
        private readonly IInMemoryCacheService _cacheService;
        private readonly ILogger<FeedService> _logger;
        private readonly int _cacheExpirationMinutes;

        public FeedService(
            IPostsApiClient postsApiClient,
            IUsersApiClient usersApiClient,
            IInMemoryCacheService cacheService,
            ILogger<FeedService> logger,
            IConfiguration configuration)
        {
            _postsApiClient = postsApiClient;
            _usersApiClient = usersApiClient;
            _cacheService = cacheService;
            _logger = logger;
            
            // Obtém o tempo de expiração do cache da configuração (padrão: 10 minutos)
            _cacheExpirationMinutes = configuration.GetValue<int>("FeedCacheExpirationMinutes", 10);
        }

        public async Task<List<FeedPostDTO>> GetFeedAsync(int pageSize = 20, int page = 1)
        {
            var cacheKey = $"feed_general_p{page}_s{pageSize}";
            
            // Tenta buscar do cache primeiro
            var cachedFeed = _cacheService.Get(cacheKey) as List<FeedPostDTO>;
            if (cachedFeed != null)
            {
                _logger.LogInformation("Feed retornado do cache: {CacheKey}", cacheKey);
                return cachedFeed;
            }

            _logger.LogInformation("Cache miss para {CacheKey}, reconstruindo feed", cacheKey);

            // Se não houver no cache, retorna lista vazia
            // (pois não temos endpoint para buscar todos os posts)
            var feed = new List<FeedPostDTO>();
            
            // Armazena no cache
            _cacheService.Add(cacheKey, feed);
            
            return feed;
        }

        public async Task<List<FeedPostDTO>> GetUserFeedAsync(Guid userId, int pageSize = 20, int page = 1)
        {
            var cacheKey = $"feed_user_{userId}_p{page}_s{pageSize}";
            
            // Tenta buscar do cache primeiro
            var cachedFeed = _cacheService.Get(cacheKey) as List<FeedPostDTO>;
            if (cachedFeed != null)
            {
                _logger.LogInformation("Feed do usuário {UserId} retornado do cache", userId);
                return cachedFeed;
            }

            _logger.LogInformation("Cache miss para feed do usuário {UserId}, reconstruindo", userId);

            try
            {
                // Busca posts do usuário
                var posts = await _postsApiClient.GetUserPostsAsync(userId, pageSize, page);

                if (posts.Count == 0)
                {
                    _logger.LogInformation("Nenhum post encontrado para o usuário {UserId}", userId);
                    var emptyFeed = new List<FeedPostDTO>();
                    _cacheService.Add(cacheKey, emptyFeed);
                    return emptyFeed;
                }

                // Busca informações do usuário (autor)
                var user = await _usersApiClient.GetUserProfileAsync(userId);

                // Monta o feed
                var feed = posts.Select(post => new FeedPostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    Author = user != null ? new AuthorDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    } : new AuthorDTO
                    {
                        Id = userId,
                        Username = "Usuário não encontrado",
                        FirstName = "",
                        LastName = "",
                        Email = ""
                    },
                    Interactions = new InteractionStatsDTO
                    {
                        LikesCount = 0,  // TODO: Integrar com serviço de interações
                        CommentsCount = 0 // TODO: Integrar com serviço de interações
                    }
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

                // Armazena no cache
                _cacheService.Add(cacheKey, feed);

                _logger.LogInformation("Feed do usuário {UserId} reconstruído com {Count} posts", userId, feed.Count);
                return feed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao construir feed do usuário {UserId}", userId);
                
                // Em caso de erro, tenta retornar o cache antigo se existir
                var fallbackCache = _cacheService.Get(cacheKey) as List<FeedPostDTO>;
                if (fallbackCache != null)
                {
                    _logger.LogWarning("Retornando cache antigo devido a erro");
                    return fallbackCache;
                }

                // Se não houver cache, retorna lista vazia
                return new List<FeedPostDTO>();
            }
        }

        public Task InvalidateFeedCacheAsync()
        {
            _logger.LogInformation("Invalidando cache de feed");
            
            // Limpa todo o cache
            // Em uma implementação mais sofisticada, poderíamos invalidar apenas chaves específicas
            _cacheService.Clear();
            
            return Task.CompletedTask;
        }
    }
}
