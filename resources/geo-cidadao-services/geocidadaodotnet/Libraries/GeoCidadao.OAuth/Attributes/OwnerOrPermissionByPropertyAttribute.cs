using System.Security.Claims;
using GeoCidadao.Models.Entities;
using GeoCidadao.OAuth.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoCidadao.OAuth.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class OwnerOrPermissionByPropertyAttribute<TResource>(string ownerPropertyName, params string[] permissions) : Attribute, IAsyncAuthorizationFilter
    where TResource : BaseEntity
    {
        private readonly string _ownerPropertyName = ownerPropertyName;
        private readonly string[] _permissions = permissions;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            IServiceProvider sp = context.HttpContext.RequestServices;
            IConfiguration cfg = sp.GetRequiredService<IConfiguration>();
            IResourceFetcher<TResource> fetcher = sp.GetRequiredService<IResourceFetcher<TResource>>();

            TResource? resource = await fetcher.GetAsync(context);
            if (resource is null) { context.Result = new NotFoundResult(); return; }

            var prop = typeof(TResource).GetProperty(_ownerPropertyName);
            if (prop is null) { context.Result = new ForbidResult(); return; }

            var user = context.HttpContext.User;

            // 1. Verifica se é o proprietário do recurso
            var ownerId = prop.GetValue(resource)?.ToString();
            var userId = user.Identity?.Name ?? user.FindFirst("sub")?.Value ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            bool isOwner = !string.IsNullOrEmpty(ownerId) && !string.IsNullOrEmpty(userId) && ownerId.Equals(userId, StringComparison.OrdinalIgnoreCase);

            if (isOwner)
                return;

            // 2. Verifica se tem permissões administrativas

            foreach (var permissionOrKey in _permissions)
            {
                if (HasPermission(user, isOwner, permissionOrKey, cfg))
                    return;
            }

            // Se chegou até aqui, não tem autorização
            context.Result = new ForbidResult();
        }

        private bool HasPermission(ClaimsPrincipal user, bool isOwner, string permissionOrKey, IConfiguration cfg)
        {
            // Tenta resolver da configuração primeiro
            string actualPermission = cfg[$"OAuth:ClaimRoles:{permissionOrKey}"]
                                   ?? cfg[$"OAuth:GroupClaims:{permissionOrKey}"]
                                   ?? permissionOrKey;

            if (actualPermission.Contains("Self", StringComparison.OrdinalIgnoreCase) && !isOwner)
                return false;

            // Verifica se tem o role/grupo necessário
            var hasPermission = user.IsInRole(actualPermission) ||
                               user.HasClaim("group", actualPermission) ||
                               user.HasClaim(System.Security.Claims.ClaimTypes.Role, actualPermission) ||
                               user.HasClaim("role", actualPermission);


            return hasPermission;
        }
    }
}