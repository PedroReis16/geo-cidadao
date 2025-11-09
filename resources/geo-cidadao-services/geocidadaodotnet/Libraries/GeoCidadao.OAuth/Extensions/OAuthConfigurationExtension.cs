using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GeoCidadao.OAuth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GeoCidadao.OAuth.Extensions
{
    public static class OAuthConfigurationExtension
    {
        public static IServiceCollection ConfigureOAuth(this IServiceCollection services, OAuthConfiguration oauthConfig)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = oauthConfig.Authority;                 // ex.: http://localhost:8082/realms/geocidadao
                    options.RequireHttpsMetadata = false;                       // TODO: true em produção
                    options.Audience = oauthConfig.Audience;                   // ex.: geocidadao-posts-api

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = oauthConfig.Authority,
                        ValidateAudience = !string.IsNullOrWhiteSpace(oauthConfig.Audience),
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine($"JWT auth failed: {ctx.Exception.GetType().Name} - {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            var principal = ctx.Principal!;
                            var id = (ClaimsIdentity)principal.Identity!;

                            // 1) Garantir NameIdentifier a partir do "sub"
                            var sub = principal.FindFirst("sub")?.Value;
                            if (!string.IsNullOrEmpty(sub) && id.FindFirst(ClaimTypes.NameIdentifier) is null)
                                id.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));

                            // 2) Mapear grupos e roles a partir do payload bruto (funciona para JsonWebToken e JwtSecurityToken)
                            if (ctx.SecurityToken is Microsoft.IdentityModel.JsonWebTokens.JsonWebToken jtok)
                            {
                                // groups
                                if (jtok.TryGetPayloadValue<object>("groups", out var groupsObj) &&
                                    groupsObj is IEnumerable<object> groups)
                                {
                                    foreach (var g in groups)
                                        id.AddClaim(new Claim("group", g.ToString()!));
                                }

                                // realm_access.roles
                                if (jtok.TryGetPayloadValue<IDictionary<string, object>>("realm_access", out var ra) &&
                                    ra.TryGetValue("roles", out var rrObj) &&
                                    rrObj is IEnumerable<object> rroles)
                                {
                                    foreach (var r in rroles)
                                        id.AddClaim(new Claim(ClaimTypes.Role, $"realm:{r}"));
                                }

                                // resource_access.<clientId>.roles
                                if (jtok.TryGetPayloadValue<IDictionary<string, object>>("resource_access", out var resources))
                                {
                                    foreach (var (clientId, entryObj) in resources)
                                    {
                                        if (entryObj is IDictionary<string, object> entry &&
                                            entry.TryGetValue("roles", out var crObj) &&
                                            crObj is IEnumerable<object> croles)
                                        {
                                            foreach (var r in croles)
                                                id.AddClaim(new Claim(ClaimTypes.Role, $"res:{clientId}:{r}"));
                                        }
                                    }
                                }
                            }
                            else if (ctx.SecurityToken is JwtSecurityToken jwt) // fallback (se o handler estiver usando JwtSecurityToken)
                            {
                                var payload = (IDictionary<string, object>)jwt.Payload;

                                // groups
                                if (payload.TryGetValue("groups", out var groupsObj) &&
                                    groupsObj is IEnumerable<object> groups)
                                {
                                    foreach (var g in groups)
                                        id.AddClaim(new Claim("group", g.ToString()!));
                                }

                                // realm_access.roles
                                if (payload.TryGetValue("realm_access", out var raObj) &&
                                    raObj is IDictionary<string, object> ra &&
                                    ra.TryGetValue("roles", out var rrObj) &&
                                    rrObj is IEnumerable<object> rroles)
                                {
                                    foreach (var r in rroles)
                                        id.AddClaim(new Claim(ClaimTypes.Role, $"realm:{r}"));
                                }

                                // resource_access.<clientId>.roles
                                if (payload.TryGetValue("resource_access", out var resObj) &&
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
                            }

                            return Task.CompletedTask;
                        }
                    };

                    // Se sua API roda em Docker e o Keycloak está no host, habilite via config:
                    // options.MetadataAddress = "http://host.docker.internal:8082/realms/geocidadao/.well-known/openid-configuration";
                });

            services.AddAuthorization(options =>
            {
                foreach (var kv in oauthConfig.ClaimRoles)
                    options.AddPolicy(kv.Key, p => p.RequireRole(kv.Value));

                foreach (var kv in oauthConfig.GroupClaims)
                    options.AddPolicy(kv.Key, p => p.RequireClaim("group", kv.Value));
            });

            return services;
        }
    }
}