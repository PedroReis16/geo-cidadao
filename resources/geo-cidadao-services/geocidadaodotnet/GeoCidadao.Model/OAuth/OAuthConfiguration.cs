using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace GeoCidadao.Model.OAuth;

public class OAuthConfiguration
{
    public string Authority { get; set; } = null!;
    public string Audience { get; set; } = null!;   
    public Dictionary<string, string> ClaimRoles { get; set; } = new();
    public Dictionary<string, string> GroupClaims { get; set; } = new();
}

public static class OAuthConfigurationExtensions
{
    public static IServiceCollection ConfigureOAuth(this IServiceCollection services, OAuthConfiguration oauthConfig)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = oauthConfig.Authority;
                options.RequireHttpsMetadata = false;   //TODO: Mudar para true em produção

                // Remove a validação da audiência (audience) do token
                var requiredAudience = oauthConfig.Audience; // ex: "geocidadao-posts-api"
                if (!string.IsNullOrWhiteSpace(requiredAudience))
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        AudienceValidator = (audiences, _, __) =>
                            audiences.Contains(requiredAudience) // aceita se o "aud" contém sua API
                    };
                }
                else
                {
                    // opcional: desabilitar validação de audience se você confia no gateway
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                }
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        JwtSecurityToken jwt = (JwtSecurityToken)context.SecurityToken;
                        ClaimsIdentity id = (ClaimsIdentity)context.Principal!.Identity!;

                        // Garantir NameIdentifier a partir de "sub"
                        var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                        if (!string.IsNullOrEmpty(sub) &&
                            id.FindFirst(ClaimTypes.NameIdentifier) is null)
                        {
                            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
                        }

                        // Mapear grupos
                        if (jwt.Payload.TryGetValue("groups", out var groupsObj) &&
                            groupsObj is IEnumerable<object> groups)
                        {
                            foreach (var g in groups)
                                id.AddClaim(new Claim("group", g.ToString()!));
                        }

                        // Mapear realm roles -> Role "realm:<role>"
                        if (jwt.Payload.TryGetValue("realm_access", out var raObj) &&
                            raObj is IDictionary<string, object> ra &&
                            ra.TryGetValue("roles", out var rrObj) &&
                            rrObj is IEnumerable<object> rroles)
                        {
                            foreach (var r in rroles)
                                id.AddClaim(new Claim(ClaimTypes.Role, $"realm:{r}"));
                        }

                        // Mapear client roles -> Role "res:<clientId>:<role>"
                        if (jwt.Payload.TryGetValue("resource_access", out var resObj) &&
                            resObj is IDictionary<string, object> resources)
                        {
                            foreach (var kv in resources)
                            {
                                var clientId = kv.Key;
                                if (kv.Value is IDictionary<string, object> entry &&
                                    entry.TryGetValue("roles", out var crObj) &&
                                    crObj is IEnumerable<object> croles)
                                {
                                    foreach (var r in croles)
                                        id.AddClaim(new Claim(ClaimTypes.Role, $"res:{clientId}:{r}"));
                                }
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var kv in oauthConfig.ClaimRoles)
            {
                options.AddPolicy(kv.Key,
                    p => p.RequireRole(kv.Value));
            }

            foreach (var kv in oauthConfig.GroupClaims)
            {
                options.AddPolicy(kv.Key,
                    p => p.RequireClaim("group", kv.Value));
            }
        });

        return services;
    }
}