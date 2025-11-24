using System.Text.Json;
using GeoCidadao.FeedMapAPI.Config;
using GeoCidadao.FeedMapAPI.Contracts.CacheServices;
using Microsoft.Extensions.Options;

namespace GeoCidadao.FeedMapAPI.Middlewares
{
    public interface IKeycloakTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken ct);
        Task InvalidateTokenAsync();
    }

    internal class KeycloakTokenProvider(HttpClient client, IKeycloakAdminCacheService cacheService, IOptions<KeycloakAdminOptions> options) : IKeycloakTokenProvider
    {
        private readonly HttpClient _httpClient = client;
        private readonly IKeycloakAdminCacheService _cacheService = cacheService;
        private readonly KeycloakAdminOptions _options = options.Value;
        private readonly SemaphoreSlim _lock = new(1, 1);


        public Task InvalidateTokenAsync()
        {
            _cacheService.RemoveToken();
            return Task.CompletedTask;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            if (_cacheService.GetToken() is { } token)
                return token;

            await _lock.WaitAsync(ct);
            try
            {
                if (_cacheService.GetToken() is { } cachedToken)
                    return cachedToken;

                var form = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret
                });

                var resp = await _httpClient.PostAsync("", form, ct);
                resp.EnsureSuccessStatusCode();

                using var json = await resp.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(json, cancellationToken: ct);

                var accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
                var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

                var ttl = TimeSpan.FromSeconds(Math.Max(5, expiresIn - _options.TokenSkewSeconds));
                _cacheService.AddToken(accessToken);

                return accessToken;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}