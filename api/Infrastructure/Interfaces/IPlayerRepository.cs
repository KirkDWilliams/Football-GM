using FootballGm.Api.Data.Entity.Ingested;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetPlayerByIdAsync(string playerId, CancellationToken cancellationToken = default);
    Task<PlayerGame?> GetPlayerGameStatsAsync(string playerId, string gameId, CancellationToken cancellationToken = default);
    Task<List<PlayerGame>> GetRecentPlayerGamesAsync(string playerId, int count, CancellationToken cancellationToken = default);
    Task<PlayerSeason?> GetPlayerSeasonStatsAsync(string playerId, short season, CancellationToken cancellationToken = default);
    Task<List<Player>> GetPlayersByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<List<Player>> GetPlayersByLeagueIdAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<List<PlayerGame>> GetTeamPlayersGameStatsAsync(int teamId, CancellationToken cancellationToken = default);
    Task<List<Player>> GetFreeAgentsAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<List<PlayerGame>> GetFreeAgentsGameStatsAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<List<Player>> GetAllPlayersAsync(CancellationToken cancellationToken = default);
    Task<List<PlayerGame>> GetAllPlayerGameStatsAsync(CancellationToken cancellationToken = default);
}
