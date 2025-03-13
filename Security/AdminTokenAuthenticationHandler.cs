using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CityCard_API.Models.DB;

namespace CityCard_API.Security;

public class AdminTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AdminTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IServiceScopeFactory serviceScopeFactory) 
        : base(options, logger, encoder)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Retrieve token (e.g., from headers)
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("No token provided");
        }

        // Create a new DI scope to access DbContext
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CityCardDBContext>();
        var tokenHash = CityCard_API.Security.Tools.ComputeHash(token);
        // Query the database asynchronously inside the scope
        var adminToken = await dbContext.AdminTokens
            .Where(t => t.TokenHash == tokenHash && t.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (adminToken == null)
        {
            return AuthenticateResult.Fail("Invalid or expired admin token");
        }

        // Create identity with Admin role
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, adminToken.AdminId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
