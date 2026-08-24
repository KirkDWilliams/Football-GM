using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class PlayerRepository(AppDbContext context) : IPlayerRepository
{
    public async Task<Player?> GetPlayerByIdAsync(string playerId)
    {
        return await context.Players
            .FirstOrDefaultAsync(p => p.PlayerId == playerId);
    }

    public async Task<PlayerGame?> GetPlayerGameStatsAsync(string playerId, string gameId)
    {
        return await context.PlayerGame
            .FirstOrDefaultAsync(pg => pg.PlayerId == playerId &&
                                       pg.GameId   == gameId);
    }

    public async Task<List<Player>> GetPlayersByTeamIdAsync(int teamId)
    {
        return await context.TeamPlayers
            .Where(tp => tp.TeamId == teamId)
            .Include(tp => tp.Player)
            .Select(tp => tp.Player)
            .ToListAsync();
    }

    public async Task<List<PlayerGame>> GetTeamPlayersGameStatsAsync(int teamId)
    {
        var playerIds = await context.TeamPlayers
            .Where(tp => tp.TeamId == teamId)
            .Select(tp => tp.PlayerId)
            .ToListAsync();

        return await context.PlayerGame
            .Where(pg => playerIds.Contains(pg.PlayerId))
            .ToListAsync();
    }

    public async Task<List<Player>> GetFreeAgentsAsync(int leagueId)
    {
        return await context.Players
            .Include(p => p.TeamPlayers)
            .Where(p => !p.TeamPlayers.Any(tp => tp.Team.LeagueId == leagueId))
            .ToListAsync();
    }

    public async Task<List<PlayerGame>> GetFreeAgentsGameStats(int leagueId)
    {
        var playerIdsWithoutTeam = await context.Players
            .Include(p => p.TeamPlayers)
            .Where(p => !p.TeamPlayers.Any(tp => tp.Team.LeagueId == leagueId))
            .Select(p => p.PlayerId)
            .ToListAsync();

        return await context.PlayerGame
        .Where(pg => playerIdsWithoutTeam.Contains(pg.PlayerId))
        .ToListAsync();
    }

    public async Task<List<Player>> GetPlayersByLeagueIdAsync(int leagueId)
    {
        return await context.TeamPlayers
            .Where(tp => tp.Team.LeagueId == leagueId)
            .Include(tp => tp.Player)
            .Select(tp => tp.Player)
            .ToListAsync();
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await context.Players.ToListAsync();
    }

    public async Task<List<PlayerGame>> GetAllPlayerGameStatsAsync()
    {
        return await context.PlayerGame.ToListAsync();
    }
}
