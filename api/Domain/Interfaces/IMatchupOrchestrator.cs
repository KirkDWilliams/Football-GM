using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Domain.Interfaces
{
    public interface IMatchupOrchestrator
    {
        Task<List<Matchup>?> GetMatchups(int league, int week, CancellationToken cancellationToken);
    }
}
