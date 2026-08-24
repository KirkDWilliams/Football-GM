using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Infrastructure;

public class LeagueRepository(AppDbContext context) : ILeagueRepository
{
    public async Task<League> AddAsync(League league, CancellationToken cancellationToken = default)
    {
        context.Leagues.Add(league);
        await context.SaveChangesAsync(cancellationToken);
        return league;
    }
}
