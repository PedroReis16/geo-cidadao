using System.Security.Claims;

namespace GeoCidadao.Model.OAuth
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var v = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            return Guid.Parse(v ?? throw new UnauthorizedAccessException("sub não encontrado"));
        }
    }
}