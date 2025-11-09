using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GeoCidadao.Model.Middlewares;

public class AllowAnonymousAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "anonymous"), new Claim("cognito:groups", "Admin"), new Claim("cognito:groups", "User") };
        var identity = new ClaimsIdentity(claims, nameof(AllowAnonymousAuthenticationHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(),
            nameof(AllowAnonymousAuthenticationHandler));

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class AllowAnonymousUserAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "anonymousUser"), new Claim("cognito:groups", "User") };
        var identity = new ClaimsIdentity(claims, nameof(AllowAnonymousUserAuthenticationHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(),
            nameof(AllowAnonymousUserAuthenticationHandler));

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class AllowAnonymousDefaults
{
    public const string AllowAnonymousScheme = "AllowAnonymous";
    public const string AllowAnonymousUserScheme = "AllowAnonymousUser";
}
