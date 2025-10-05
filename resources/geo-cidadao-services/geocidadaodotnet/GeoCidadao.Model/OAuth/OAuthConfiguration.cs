using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using GeoCidadao.Model.Helpers;
using GeoCidadao.Model.Middlewares;
using NetDevPack.Security.JwtExtensions;

namespace GeoCidadao.Model.OAuth;

public class OAuthConfiguration
{
    public const string SECRET_ALLOW_ANONYMOUS = "sirwyX-9cixze-vejgeg";
    public const string SECRET_ALLOW_ANONYMOUS_USER = "jkd72lm-q9vtxe4-81rmbz3";
    public string JwksEndpoint { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public List<string> Claims { get; set; } = null!;
    public string? ClientId { get; set; }
}

public static class OAuthConfigurationExtensions
{
    public static IServiceCollection ConfigureOAuth(this IServiceCollection services, List<OAuthConfiguration> oauthConfig)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.AddScheme<AllowAnonymousAuthenticationHandler>(AllowAnonymousDefaults.AllowAnonymousScheme, AllowAnonymousDefaults.AllowAnonymousScheme);
                options.AddScheme<AllowAnonymousUserAuthenticationHandler>(AllowAnonymousDefaults.AllowAnonymousUserScheme, AllowAnonymousDefaults.AllowAnonymousUserScheme);
            });

        foreach (var config in oauthConfig)
        {
            var schemeName = !string.IsNullOrWhiteSpace(config.ClientId) ? "ClientCredentials" : "OAuth";

            services.AddAuthentication().AddJwtBearer(schemeName, options =>
            {
                options.SetJwksOptions(new JwkOptions(
                    jwksUri: config.JwksEndpoint,
                    issuer: config.Issuer,
                    cacheTime: TimeSpan.FromMinutes(5)
                ));
            });
        }


        services.AddAuthentication()
            .AddPolicyScheme(JwtBearerDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();
                    string authHeader = context.Request.Headers.Authorization.ToString();
                    if (remoteIpAddress != null && Validators.IsPrivateIpAddress(remoteIpAddress) && authHeader.Contains($"Bearer {OAuthConfiguration.SECRET_ALLOW_ANONYMOUS}"))
                    {
                        return AllowAnonymousDefaults.AllowAnonymousScheme;
                    }
                    if (remoteIpAddress != null && Validators.IsPrivateIpAddress(remoteIpAddress) && authHeader.Contains($"Bearer {OAuthConfiguration.SECRET_ALLOW_ANONYMOUS_USER}"))
                    {
                        return AllowAnonymousDefaults.AllowAnonymousUserScheme;
                    }
                    var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : null;
                    if (!string.IsNullOrEmpty(token))
                    {
                        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                        if (jwt.Claims.Any(c => c.Type == "cognito:groups"))
                            return "OAuth";
                        string clientId = jwt.Claims.First(c => c.Type == "client_id").Value;
                        if (oauthConfig.Any(c => c.ClientId == clientId))
                            return "ClientCredentials";
                    }

                    return "OAuth";
                };
            });

        services.AddAuthorization(options =>
        {
            oauthConfig.ForEach(config =>
            {
                config.Claims.ForEach(claim =>
                {
                    options.AddPolicy(claim, policy =>
                    {
                        policy.RequireAssertion(context =>
                        {
                            var hasCognitoGroup = context.User.HasClaim(c =>
                                c.Type == "cognito:groups" && c.Value == claim);

                            var hasClientId = !string.IsNullOrEmpty(config.ClientId) &&
                                context.User.HasClaim(c =>
                                    c.Type == "client_id" && c.Value == config.ClientId);

                            return hasCognitoGroup || hasClientId;
                        });
                    });
                });
            });
        });
        return services;
    }
}