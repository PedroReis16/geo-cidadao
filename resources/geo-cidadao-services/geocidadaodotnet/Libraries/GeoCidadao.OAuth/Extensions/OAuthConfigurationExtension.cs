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
                        ClockSkew = TimeSpan.FromMinutes(2),

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
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
                            var id = (ClaimsIdentity)ctx.Principal!.Identity!;

                            // Garante que é autenticado e usa RoleClaimType correto
                            if (!id.IsAuthenticated)
                                id = new ClaimsIdentity(id.Claims, "Bearer", ClaimTypes.NameIdentifier, ClaimTypes.Role);

                            // Força o NameIdentifier pelo "sub"
                            var sub = id.FindFirst("sub")?.Value;
                            if (!string.IsNullOrEmpty(sub) && id.FindFirst(ClaimTypes.NameIdentifier) is null)
                                id.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));

                            // � 1) Mapear groups - direto das claims já parseadas
                            var groupClaims = id.Claims.Where(c => c.Type == "groups").ToList();
                            Console.WriteLine($"[DEBUG] Found {groupClaims.Count} group claims in token");
                            foreach (var gc in groupClaims)
                            {
                                Console.WriteLine($"[DEBUG] Adding group: {gc.Value}");
                                id.AddClaim(new Claim("group", gc.Value));
                            }

                            // 🔹 2) Mapear realm_access.roles - procurar nas claims
                            var realmRoleClaims = id.Claims.Where(c => c.Type == "realm_access.roles" || c.Type.EndsWith("/roles")).ToList();
                            Console.WriteLine($"[DEBUG] Found {realmRoleClaims.Count} realm role claims");
                            
                            // Tentar pegar do claim realm_access
                            var realmAccessClaim = id.FindFirst("realm_access");
                            if (realmAccessClaim != null)
                            {
                                try
                                {
                                    var realmAccess = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(realmAccessClaim.Value);
                                    if (realmAccess != null && realmAccess.TryGetValue("roles", out var rolesElement))
                                    {
                                        foreach (var role in rolesElement.EnumerateArray())
                                        {
                                            var roleValue = role.GetString();
                                            id.AddClaim(new Claim(id.RoleClaimType, $"realm:{roleValue}"));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"[ERROR] Failed to parse realm_access: {ex.Message}");
                                }
                            }

                            // 🔹 3) Mapear resource_access.<client>.roles - procurar nas claims
                            var resourceAccessClaim = id.FindFirst("resource_access");
                            if (resourceAccessClaim != null)
                            {
                                try
                                {
                                    var resourceAccess = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(resourceAccessClaim.Value);
                                    if (resourceAccess != null)
                                    {
                                        foreach (var client in resourceAccess)
                                        {
                                            var clientId = client.Key;
                                            
                                            if (client.Value.ValueKind == System.Text.Json.JsonValueKind.Object &&
                                                client.Value.TryGetProperty("roles", out var rolesElement) &&
                                                rolesElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                                            {
                                                foreach (var role in rolesElement.EnumerateArray())
                                                {
                                                    var roleValue = role.GetString();
                                                    var fullRole = $"res:{clientId}:{roleValue}";
                                                    id.AddClaim(new Claim(id.RoleClaimType, fullRole));
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"[ERROR] Failed to parse resource_access: {ex.Message}");
                                }
                            }

                            // 🔹 Atualiza o principal
                            ctx.Principal = new ClaimsPrincipal(id);


                            return Task.CompletedTask;
                        }
                    };

                    // Se sua API roda em Docker e o Keycloak está no host, habilite via config:
                    // options.MetadataAddress = "http://host.docker.internal:8082/realms/geocidadao/.well-known/openid-configuration";
                });

            services.AddAuthorization(options =>
            {
                foreach (var kv in oauthConfig.ClaimRoles)
                {
                    Console.WriteLine($"[POLICY CONFIG] Policy: {kv.Key} -> RequireRole: {kv.Value}");
                    options.AddPolicy(kv.Key, p => p.RequireRole(kv.Value));
                }

                foreach (var kv in oauthConfig.GroupClaims)
                {
                    Console.WriteLine($"[POLICY CONFIG] Policy: {kv.Key} -> RequireClaim group: {kv.Value}");
                    options.AddPolicy(kv.Key, p => p.RequireClaim("group", kv.Value));
                }
            });

            return services;
        }
    }
}