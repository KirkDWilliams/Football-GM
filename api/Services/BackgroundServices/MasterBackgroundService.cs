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

        REngine.SetEnvironmentVariables(
            @"C:\Program Files\R\R-4.6.1\bin\x64",
            @"C:\Program Files\R\R-4.6.1");

        using var engine = REngine.GetInstance();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentDay = DateTime.Now;

                var nflWeek = Helpers.WeekHelper.CurrentWeek;
                if (nflWeek == 0 || nflWeek > 18)
                {
                    IngestNFLGames(engine);
                    await FinishDay(stoppingToken);
                    continue;
                }

                switch (currentDay.DayOfWeek)
                {
                    case DayOfWeek.Tuesday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async k =>
                            {
                                IngestGameStats(engine);
                                IngestInjuries(engine);
                                UpdatePlayerSeasonStats(engine);
                            }, stoppingToken);

                        await MinutesUntil(desiredHour: 8, desiredMinute: 0,
                            async k =>
                                await CloseMatches(nflWeek),
                            stoppingToken);

                        await MinutesUntil(desiredHour: 9, desiredMinute: 0,
                            async k =>
                            {
                                await TerminateContracts();
                                IngestPlayerInfo(engine);
                                await OpenAuctionedFreeAgency();
                                await OpenTrading();
                            }, stoppingToken);

                        await FinishDay(stoppingToken);
                        break;

                    case DayOfWeek.Wednesday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async k =>
                            {
                                IngestGameStats(engine);
                                IngestInjuries(engine);
                            }, stoppingToken);
                            
                        if (nflWeek == 12)
                        {
                            /* thanksgiving slide */
                            await MinutesUntil(desiredHour: 2, desiredMinute: 0,
                                async k =>
                                {
                                    await CloseAuctionedFreeAgency();
                                    await OpenUnrestrictedFreeAgency();
                                    
                                }, stoppingToken);

                            await MinutesUntil(desiredHour: 12, desiredMinute: 45,
                                async k =>
                                {
                                    await CloseTrading();
                                    await CloseUnrestrictedFreeAgency();
                                });
                            
                            await FinishDay(stoppingToken);
                        }

                        await MinutesUntil(desiredHour: 12, desiredMinute: 0,
                            async k => await CloseAuctionedFreeAgency(), stoppingToken);

                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async k => await OpenUnrestrictedFreeAgency(), stoppingToken);

                        await FinishDay(stoppingToken);
                        break;

                    case DayOfWeek.Thursday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async k =>
                            {
                                IngestGameStats(engine);
                                IngestInjuries(engine);
                            }, stoppingToken);

                        if (nflWeek == 12)
                            await FinishDay(stoppingToken);

                        await MinutesUntil(desiredHour: 19, desiredMinute: 0,
                            async k =>
                            {
                                await CloseUnrestrictedFreeAgency();
                                await CloseTrading();
                            }, stoppingToken);

                        await FinishDay(stoppingToken);
                        break;

                    case DayOfWeek.Friday:
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                    case DayOfWeek.Monday:
                        await MinutesUntil(desiredHour: 1, desiredMinute: 0,
                            async k =>
                            {
                                IngestGameStats(engine);
                                IngestInjuries(engine);
                            }, stoppingToken);

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

        _logger.LogInformation("Master Background Service has stopped");
    }

    private static void UpdatePlayerSeasonStats(REngine engine)
    {
        // PlayerSeasonIngestion.R
        var script = "PlayerSeasonIngestion.R";
        engine.Evaluate(
            ScriptPath(script)
        );
    }

    private static void IngestGameStats(REngine engine)
    {
        // PlayerGameIngestion.R
        var script = "PlayerGameIngestion.R";
        engine.Evaluate(
            ScriptPath(script)
        );
    }

    private static void IngestPlayerInfo(REngine engine)
    {
        // PlayerInfoIngestion.R
        var script = "PlayerInfoIngestion.R";
        engine.Evaluate(
            ScriptPath(script)
        );
    }

    private static void IngestNFLGames(REngine engine)
    {
        // GameIngestion.R
        var script = "GameIngestion.R";
        engine.Evaluate(
            ScriptPath(script)
        );
    }

    private static void IngestInjuries(REngine engine)
    {
        // InjuryStatusIngestion.R
        var script = "InjuryStatusIngestion.R";
        engine.Evaluate(
            ScriptPath(script)
        );
    }

    private static string ScriptPath(string path)
    {
        return $@"source('..\..\Scripts\{path}.R')";
    }

    private async Task TerminateContracts()
    {
        throw new NotImplementedException();
    }

    private async Task CloseMatches(int nflWeek)
    {
        throw new NotImplementedException();
    }

    private async Task OpenTrading()
    {
        throw new NotImplementedException();
    }

    private async Task CloseTrading()
    {
        throw new NotImplementedException();
    }

    private async Task OpenUnrestrictedFreeAgency()
    {
        throw new NotImplementedException();
    }

    private async Task CloseUnrestrictedFreeAgency()
    {
        throw new NotImplementedException();
    }

    private async Task OpenAuctionedFreeAgency()
    {
        throw new NotImplementedException();
    }

    private async Task CloseAuctionedFreeAgency()
    {
        throw new NotImplementedException();
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
