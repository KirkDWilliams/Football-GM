using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class MatchupRepository(AppDbContext context) : IMatchupRepository
{
    public async Task<List<Matchup>> GetWeeklyMatchupsByLeagueId(int leagueId, int week, CancellationToken cancellationToken)
    {
        var query = context.Matchups
            .AsNoTracking()
            .Where(m => m.LeagueId == leagueId &&
                        m.Week == week)
            .AsQueryable();

        return await query.ToListAsync(cancellationToken);
    }
}
