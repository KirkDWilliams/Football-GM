namespace FootballGm.Api.Services.BackgroundServices
{
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
                        case DayOfWeek.Sunday:
                        case DayOfWeek.Monday:
                            await MinutesUntil(desiredHour: 1, desiredMinute: 0, stoppingToken);
                            // pull game stats
                            await FinishDay(stoppingToken);
                            break;

                        case DayOfWeek.Tuesday:
                            await MinutesUntil(desiredHour: 1, desiredMinute: 0, stoppingToken);
                            // pull game stats

                            await MinutesUntil(desiredHour: 6, desiredMinute: 0, stoppingToken);
                            // Determine who won each game

                            await MinutesUntil(desiredHour: 9, desiredMinute: 0, stoppingToken);
                            // Open Auction Free Agency
                            // Open Trading

                            await FinishDay(stoppingToken);
                            break;

                        case DayOfWeek.Wednesday:
                            await MinutesUntil(desiredHour: 9, desiredMinute: 0, stoppingToken);
                            // Close free agency auction
                            // Award players to teams and update budgets
                            // Open Free Agency Contracts

                            if (nflWeek == 12)
                            {
                                /* thanksgiving slide */
                                await MinutesUntil(desiredHour: 19, desiredMinute: 0, stoppingToken);
                                // close free agency
                                // close trading
                                // award free agency
                            }

                            await FinishDay(stoppingToken);
                            break;

                        case DayOfWeek.Thursday:
                            await MinutesUntil(desiredHour: 1, desiredMinute: 0, stoppingToken);
                            // pull game stats

                            if (nflWeek == 12)
                                await FinishDay(stoppingToken);

                            await MinutesUntil(desiredHour: 19, desiredMinute: 0, stoppingToken);
                            // close free agency
                            // close trading
                            // award free agency

                            await FinishDay(stoppingToken);
                            break;

                        case DayOfWeek.Friday:
                        case DayOfWeek.Saturday:
                            await MinutesUntil(desiredHour: 1, desiredMinute: 0, stoppingToken);
                            // pull game stats
                            await FinishDay(stoppingToken);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while closing expired auctions");
                }

                // Wait before the next check
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("AuctionCloserBackgroundService stopped");
        }

        private async static Task MinutesUntil(int desiredHour, int desiredMinute, CancellationToken stoppingToken = default)
        {
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            if (currentTime > new TimeOnly(desiredHour, desiredMinute))
                return;

            var minutes = 60 * (desiredHour - currentTime.Hour)
                - currentTime.Minute
                + desiredMinute;

            await Task.Delay(TimeSpan.FromMinutes(minutes), stoppingToken);
        }

        private async static Task FinishDay(CancellationToken stoppingToken)
        {
            await MinutesUntil(desiredHour: 23, desiredMinute: 55, stoppingToken);
        }
    }
}
