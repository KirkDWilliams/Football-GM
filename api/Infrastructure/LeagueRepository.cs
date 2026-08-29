using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class LeagueRepository(AppDbContext context) : ILeagueRepository
{
    public async Task<League> AddAsync(League league, CancellationToken cancellationToken = default)
    {
        context.Leagues.Add(league);
        await context.SaveChangesAsync(cancellationToken);
        return league;
    }

    public async Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return await context.Leagues
            .Include(l => l.Settings)
                .ThenInclude(s => s.Rules)
            .FirstOrDefaultAsync(l => l.LeagueId == leagueId, cancellationToken);
    }
}
