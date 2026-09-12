using RDotNet;

namespace FootballGm.Api.Services.BackgroundServices;

public class MasterBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MasterBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);

    public MasterBackgroundService(IServiceScopeFactory scopeFactory, ILogger<MasterBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background services running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentDay = DateTime.Now;

                var nflWeek = Helpers.WeekHelper.CurrentWeek;

                switch (currentDay.DayOfWeek)
                {
                    case DayOfWeek.Tuesday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async game => await UpdateGameStats(), stoppingToken);

                        await MinutesUntil(desiredHour: 6, desiredMinute: 0,
                            async match => await DetermineMatches(nflWeek), stoppingToken);

                        await MinutesUntil(desiredHour: 9, desiredMinute: 0,
                            async agency =>
                            {
                                await OpenAuctionedFreeAgency();
                                await OpenTrading();
                            }, stoppingToken);

                        await FinishDay(stoppingToken);
                        break;

                    case DayOfWeek.Wednesday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async game => await UpdateGameStats(), stoppingToken);

                        if (nflWeek == 12)
                        {
                            /* thanksgiving slide */
                            await MinutesUntil(desiredHour: 10, desiredMinute: 0,
                                async k =>
                                {
                                    CloseAuctions();
                                    CloseUnrestrictedFreeAgency();
                                    CloseTrading();
                                }, stoppingToken);
                            
                            await FinishDay(stoppingToken);
                        }

                        await MinutesUntil(desiredHour: 12, desiredMinute: 0,
                            async k => CloseAuctions(), stoppingToken);

                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async k => OpenUnrestrictedFreeAgency(), stoppingToken);

                        await FinishDay(stoppingToken);
                        break;

                    case DayOfWeek.Thursday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async game => await UpdateGameStats(), stoppingToken);

                        if (nflWeek == 12)
                            await FinishDay(stoppingToken);

                        await MinutesUntil(desiredHour: 19, desiredMinute: 0,
                            async k =>
                            {
                                CloseAuctions();
                                CloseUnrestrictedFreeAgency();
                                CloseTrading();
                            }, stoppingToken);

                        await FinishDay(stoppingToken);
                        break;

                    case DayOfWeek.Friday:
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                    case DayOfWeek.Monday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async game => await UpdateGameStats(), stoppingToken);

                        await FinishDay(stoppingToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while closing expired auctions");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("AuctionCloserBackgroundService stopped");
    }

    private async Task OpenTrading()
    {
        throw new NotImplementedException();
    }

    private void CloseTrading()
    {
        throw new NotImplementedException();
    }

    private void CloseUnrestrictedFreeAgency()
    {
        throw new NotImplementedException();
    }

    private void OpenUnrestrictedFreeAgency()
    {
        throw new NotImplementedException();
    }

    private void CloseAuctions()
    {
        throw new NotImplementedException();
    }

    private async Task OpenAuctionedFreeAgency()
    {
        throw new NotImplementedException();
    }

    private async Task DetermineMatches(int nflWeek)
    {
        throw new NotImplementedException();
    }

    private async Task UpdateGameStats()
    {
        ;
    }

    private async static Task MinutesUntil(int desiredHour, int desiredMinute, Func<CancellationToken, Task>? onComplete = null, CancellationToken stoppingToken = default)
    {
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        if (currentTime > new TimeOnly(desiredHour, desiredMinute))
            return;

        var minutes = 60 * (desiredHour - currentTime.Hour)
            - currentTime.Minute
            + desiredMinute;

        await Task.Delay(TimeSpan.FromMinutes(minutes), stoppingToken);

        if (onComplete != null)
            await onComplete(stoppingToken);
    }

    private async static Task FinishDay(CancellationToken stoppingToken)
    {
        await MinutesUntil(desiredHour: 23, desiredMinute: 59, null, stoppingToken);
    }
}
