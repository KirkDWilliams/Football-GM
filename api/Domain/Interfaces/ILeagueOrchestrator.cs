using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Domain.Interfaces;

public interface ILeagueOrchestrator
{
    Task<League> CreateLeague(string userId, League league, CancellationToken cancellationToken);
    Task<JoinLeagueResult> JoinLeague(string userId, string leagueCode, CancellationToken cancellationToken);
}
