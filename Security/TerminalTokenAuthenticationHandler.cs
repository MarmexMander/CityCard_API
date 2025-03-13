using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CityCard_API.Models.DB;

namespace CityCard_API.Security;

public class TerminalTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TerminalTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IServiceScopeFactory serviceScopeFactory)
        : base(options, logger, encoder, clock)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Get the token from the request header
        var authData = Request.Headers["Authorization"].FirstOrDefault()?.Split(":");
        if (authData == null || authData.Length != 2)
        {
            return AuthenticateResult.Fail("Authorization data is invalid");
        }
        string TerminalId = authData[0];
        string token = authData[1];

        // Validate the token
        var tokenHash = Tools.ComputeHash(token); // Hashing logic here
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CityCardDBContext>();
        var terminalId = Guid.Parse(TerminalId);
        var terminalToken = await dbContext.TerminalTokens
        .Where(t =>
             t.TerminalId == terminalId
             && t.TokenHash == tokenHash 
             && t.ValidUntil > DateTime.UtcNow)
        .FirstAsync();

        if (terminalToken == null)
        {
            return AuthenticateResult.Fail("Invalid or expired token");
        }

        // Create claims
        var claims = new[]
        {
            new Claim(type: ClaimTypes.NameIdentifier, terminalToken.TerminalId.ToString())
        };

        var identity = new ClaimsIdentity(claims, nameof(TerminalTokenAuthenticationHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}