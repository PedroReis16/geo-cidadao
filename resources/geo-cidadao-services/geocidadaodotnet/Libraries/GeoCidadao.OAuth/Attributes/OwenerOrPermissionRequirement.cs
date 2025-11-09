using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GeoCidadao.OAuth.Attributes
{
    public sealed class OwnerOrPermissionRequirement<TResource> : IAuthorizationRequirement
    {
        // Como extrair o OwnerId do recurso (se preferir, use IOwnedResource e um helper abaixo)
        public Func<TResource, string?> OwnerSelector { get; }

        // Lista de permissões/grupos. Podem ser chaves do appsettings (ex.: "Posts.Delete.Any")
        // ou valores diretos (ex.: "res:geocidadao-posts-api:post:delete:any")
        public IReadOnlyList<string> PermissionsOrKeys { get; }

        // Se true, tenta resolver cada item em ClaimRoles do appsettings; se não existir, usa o valor como está
        public bool ResolveFromConfig { get; }

        public OwnerOrPermissionRequirement(
            Func<TResource, string?> ownerSelector,
            IEnumerable<string> permissionsOrKeys,
            bool resolveFromConfig = true)
        {
            OwnerSelector      = ownerSelector ?? throw new ArgumentNullException(nameof(ownerSelector));
            PermissionsOrKeys  = permissionsOrKeys?.ToArray() ?? Array.Empty<string>();
            ResolveFromConfig  = resolveFromConfig;
        }
    }
}