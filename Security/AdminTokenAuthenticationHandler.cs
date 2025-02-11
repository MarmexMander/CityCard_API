using Microsoft.AspNetCore.Authentication;
using CityCard_API.Models.DB;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Data.Entity;
using CityCard_API.Security;

public class AdminTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly CityCardDBContext _dbContext;

    public AdminTokenAuthenticationHandler(
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
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return AuthenticateResult.NoResult();
        }

        var token = authorizationHeader.ToString().Replace("Bearer ", "");

        var tokenHashBase64 = Tools.ComputeHash(token);

        var adminToken = await _dbContext.AdminTokens
            .Include(t => t.Admin)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHashBase64);

        if (adminToken == null)
        {
            return AuthenticateResult.Fail("Invalid token.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, adminToken.Admin.UserName),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.NameIdentifier, adminToken.AdminId)
        };

        var identity = new ClaimsIdentity(claims, "AdminToken");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "AdminToken");

        return AuthenticateResult.Success(ticket);
    }
}
