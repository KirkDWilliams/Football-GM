using FootballGm.Api.Auth;
using Microsoft.Extensions.Options;

namespace FootballGm.Api.Services;

/// <summary>
/// Periodically deletes expired and aged-revoked refresh tokens so the table stays small.
/// Also runs once shortly after startup.
/// </summary>
public class RefreshTokenCleanupHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<JwtOptions> jwtOptions,
    ILogger<RefreshTokenCleanupHostedService> logger) : BackgroundService
{
    private readonly JwtOptions _options = jwtOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Let the app finish startup (migrations, etc.) before the first pass.
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var maintenance = scope.ServiceProvider.GetRequiredService<IRefreshTokenMaintenance>();
                var removed = await maintenance.CleanupAsync(stoppingToken);

                if (removed > 0)
                {
                    logger.LogInformation(
                        "Refresh token cleanup removed {RemovedCount} dead row(s).",
                        removed);
                }
                else
                {
                    logger.LogDebug("Refresh token cleanup found nothing to remove.");
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Refresh token cleanup failed.");
            }

            var hours = Math.Max(1, _options.RefreshTokenCleanupIntervalHours);
            try
            {
                await Task.Delay(TimeSpan.FromHours(hours), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
