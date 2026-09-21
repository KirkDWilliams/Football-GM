using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface IMatchupRepository
{
    Task<List<Matchup>> GetWeeklyMatchupsByLeagueId(int leagueId, int week, CancellationToken cancellationToken);
}
