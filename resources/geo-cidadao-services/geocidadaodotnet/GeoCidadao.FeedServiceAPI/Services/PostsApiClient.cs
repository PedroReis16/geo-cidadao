using GeoCidadao.FeedServiceAPI.Contracts;
using GeoCidadao.FeedServiceAPI.Models.External;
using System.Net.Http.Headers;

namespace GeoCidadao.FeedServiceAPI.Services
{
    /// <summary>
    /// Cliente HTTP para comunicação com a API de Posts
    /// </summary>
    public class PostsApiClient : IPostsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PostsApiClient> _logger;

        public PostsApiClient(HttpClient httpClient, ILogger<PostsApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<PostDTO>> GetUserPostsAsync(Guid userId, int? itemsCount = null, int? pageNumber = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (itemsCount.HasValue)
                    queryParams.Add($"itemsCount={itemsCount.Value}");
                if (pageNumber.HasValue)
                    queryParams.Add($"pageNumber={pageNumber.Value}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"posts/{userId}/posts{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var posts = await response.Content.ReadFromJsonAsync<List<PostDTO>>();
                    return posts ?? new List<PostDTO>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return new List<PostDTO>();
                }

                _logger.LogWarning("Failed to fetch posts for user {UserId}. Status: {StatusCode}", 
                    userId, response.StatusCode);
                return new List<PostDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching posts for user {UserId}", userId);
                return new List<PostDTO>();
            }
        }

        public async Task<List<PostDTO>> GetAllPostsAsync(int? itemsCount = null, int? pageNumber = null)
        {
            // Como não temos um endpoint específico para buscar todos os posts,
            // vamos retornar uma lista vazia por enquanto e isso pode ser expandido futuramente
            _logger.LogInformation("GetAllPostsAsync não implementado - endpoint não disponível na API de Posts");
            return new List<PostDTO>();
        }
    }
}
