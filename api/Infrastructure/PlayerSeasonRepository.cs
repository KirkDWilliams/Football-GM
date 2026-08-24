using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Ingested;
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
        return await _context.PlayerSeason
            .FirstOrDefaultAsync(pg => pg.PlayerId == playerId);
    }
}
