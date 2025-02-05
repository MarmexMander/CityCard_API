using CityCard_API.Models.DB;

namespace CityCard_API.Services;
public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TokenCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CityCardDBContext>();

                var now = DateTime.UtcNow;
                var expiredTokens = dbContext.TerminalTokens.Where(t => t.ValidUntil <= now);
                dbContext.TerminalTokens.RemoveRange(expiredTokens);
                await dbContext.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}