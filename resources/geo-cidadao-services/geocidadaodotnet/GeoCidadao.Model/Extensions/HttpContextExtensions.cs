using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using GeoCidadao.Model.OAuth;

namespace GeoCidadao.Model.Extensions
{
    public static class RequestLogsExtensions
    {
        public static string? GetUser(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                string header = authHeader.ToString().Replace("Bearer ", "");

                string? userId =
                    header == OAuthConfiguration.SECRET_ALLOW_ANONYMOUS ? "Anonymous Admin User" :
                    header == OAuthConfiguration.SECRET_ALLOW_ANONYMOUS_USER ? "Anonymous User" :
                    new JwtSecurityTokenHandler()
                        .ReadJwtToken(header)
                        .Claims
                        .FirstOrDefault(c => c.Type == "username")?.Value;
                return userId;
            }

            return null;
        }
        public static string? GetClientIpAddress(this HttpContext httpContext)
        {
            return httpContext?.Connection?.RemoteIpAddress?.ToString() ;
        }
        public static string GetRequestId(this HttpContext context) => context.TraceIdentifier;
        public static string GetConnectionId(this HttpContext context) => context.Connection.Id;
        public static string GetScheme(this HttpContext context) => context.Request.Scheme;
        public static string GetHost(this HttpContext context) => context.Request.Host.ToString();
        public static string GetTimestamp(this HttpContext context) => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
        public static string GetRequestMethod(this HttpContext context) => context.Request.Method;
    }
}