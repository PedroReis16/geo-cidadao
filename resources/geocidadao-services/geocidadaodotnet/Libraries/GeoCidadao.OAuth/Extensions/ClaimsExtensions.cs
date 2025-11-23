using System.Net.NetworkInformation;
using System.Security.Claims;
using GeoCidadao.OAuth.Models;

namespace GeoCidadao.Models.OAuth
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var v = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            return Guid.Parse(v ?? throw new UnauthorizedAccessException("sub não encontrado"));
        }

        public static RequestUser GetUserDetails(this ClaimsPrincipal user)
        {
            RequestUser requestUser = new()
            {
                Id = user.GetUserId(),
                Email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                FirstName = user.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                LastName = user.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                Picture = user.FindFirstValue("picture"),
            };
            return requestUser;
        }
    }
}