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
            var sp = context.HttpContext.RequestServices;
            var auth = sp.GetRequiredService<IAuthorizationService>();
            var cfg = sp.GetRequiredService<IConfiguration>();
            var fetcher = sp.GetRequiredService<IResourceFetcher<TResource>>();

            var resource = await fetcher.GetAsync(context);
            if (resource is null) { context.Result = new NotFoundResult(); return; }

            // monta requirement genérico reaproveitando seu helper
            var req = new OwnerOrPermissionRequirement<TResource>(
                ownerSelector: r => r.GetType().GetProperty(_ownerPropertyName)?.GetValue(r)?.ToString(),
                permissionsOrKeys: _permissions,
                resolveFromConfig: true);

            var result = await auth.AuthorizeAsync(context.HttpContext.User, resource, req);
            if (!result.Succeeded) context.Result = new ForbidResult();
        }
    }
}