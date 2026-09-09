namespace FootballGm.Api.Services.BackgroundServices
{
    public class MasterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MasterBackgroundService> _logger;
        private readonly TimeSpan _matchCheck = TimeSpan.FromDays(1);
        private readonly TimeSpan _freeAgencyCheck = TimeSpan.FromDays(1);
        private readonly TimeSpan _playerStatsCheck = TimeSpan.FromDays(1);
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);
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
                    var currTime = DateTime.UtcNow;
                    var nflWeek = Helpers.WeekHelper.CurrentWeek;

                    switch (currTime.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            break;
                        case DayOfWeek.Monday:
                            break;
                        case DayOfWeek.Tuesday:
                            break;
                        case DayOfWeek.Wednesday:
                            break;
                        case DayOfWeek.Thursday:
                            break;
                        case DayOfWeek.Friday:
                            break;
                        case DayOfWeek.Saturday:
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
    }
}
