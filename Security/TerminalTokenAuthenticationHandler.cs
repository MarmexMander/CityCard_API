using Microsoft.AspNetCore.Authentication;
using CityCard_API.Models.DB;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Data.Entity;

namespace CityCard_API.Security;

public class TerminalTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly CityCardDBContext _dbContext;

    public TerminalTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        CityCardDBContext dbContext)
        : base(options, logger, encoder, clock)
    {
        _dbContext = dbContext;
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
        var tokenHash = ComputeHash(token); // Hashing logic here
        var terminalToken = await _dbContext.TerminalTokens
        .Where(t =>
             t.Terminal.Id.ToString() == TerminalId 
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
            new Claim(type: ClaimTypes.NameIdentifier, terminalToken.Terminal.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, nameof(TerminalTokenAuthenticationHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private string ComputeHash(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashed = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashed);
    }
}