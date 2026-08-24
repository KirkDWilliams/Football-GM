using FootballGm.Api.Data.Entity.Ingested;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetPlayerByIdAsync(string playerId);
    Task<PlayerGame?> GetPlayerGameStatsAsync(string playerId, string gameId);
    Task<List<Player>> GetPlayersByTeamIdAsync(int teamId);
    Task<List<Player>> GetPlayersByLeagueIdAsync(int leagueId);
    Task<List<Player>> GetAllPlayersAsync();
    Task<List<PlayerGame>> GetAllPlayerGameStatsAsync();
}
