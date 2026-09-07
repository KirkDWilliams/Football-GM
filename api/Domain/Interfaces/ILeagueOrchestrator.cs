using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain;

namespace FootballGm.Api.Domain.Interfaces;

public interface ILeagueOrchestrator
{
    Task<League> CreateLeague(string userId, League league, CancellationToken cancellationToken);
    Task<JoinLeagueResult> JoinLeague(string userId, string leagueCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeagueSummary>> GetMyLeagues(string userId, CancellationToken cancellationToken);
    Task<GetLeagueResult> GetLeague(string userId, int leagueId, CancellationToken cancellationToken);
}
