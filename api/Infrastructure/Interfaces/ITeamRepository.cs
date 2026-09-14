using FootballGm.Api.Data.Entity.Associations;
using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface ITeamRepository
{
    Task<List<Team>> GetByLeagueIdAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<TeamPlayers?> GetTeamPlayerByPlayerIdAsync(string playerId, CancellationToken cancellationToken = default);
    Task<List<TeamPlayers>> GetTeamPlayersByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default);
    Task<Team?> UpdateAsync(Team team, CancellationToken cancellationToken = default);
}
