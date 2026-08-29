using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class PlayerSeasonRepository
{
    private AppDbContext _context;

    public PlayerSeasonRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<PlayerSeason?> GetPlayerSeasonStatsAsync(string playerId)
    {
        var season = WeekHelper.CurrentSeason;

        return await _context.PlayerSeason
            .Where(ps => ps.PlayerId == playerId && ps.Season == season)
            .FirstOrDefaultAsync();
    }
}
