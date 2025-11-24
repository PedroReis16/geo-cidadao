using System.Net;
using System.Net.Http.Headers;

namespace GeoCidadao.FeedServiceAPI.Middlewares
{
    internal class KeycloakAdminHandler(IKeycloakTokenProvider tokenProvider) : DelegatingHandler
    {
        private readonly IKeycloakTokenProvider _tokenProvider = tokenProvider;

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization is null)
            {
                var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                response.Dispose();
                await _tokenProvider.InvalidateTokenAsync();

                var retryRequest = await CloneAsync(request, cancellationToken);
                var newToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                return await base.SendAsync(retryRequest, cancellationToken);
            }
            return response;
        }

        private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage req, CancellationToken ct)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Content (precisa rebufferizar)
            if (req.Content != null)
            {
                var bytes = await req.Content.ReadAsByteArrayAsync(ct);
                var ms = new ByteArrayContent(bytes);
                foreach (var h in req.Content.Headers)
                    ms.Headers.TryAddWithoutValidation(h.Key, h.Value);
                clone.Content = ms;
            }

            // Headers
            foreach (var h in req.Headers)
                clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
            foreach (var p in req.Options)
                clone.Options.Set(new HttpRequestOptionsKey<object?>(p.Key), p.Value);

            clone.Version = req.Version;
            clone.VersionPolicy = req.VersionPolicy;
            return clone;
        }
    }
}