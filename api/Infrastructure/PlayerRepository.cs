using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Ingested;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class PlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetPlayerByIdAsync(string playerId)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.PlayerId == playerId);
    }

    public async Task<PlayerGame?> GetPlayerGameStatsAsync(string playerId, string gameId)
    {
        return await _context.PlayerGame
            .FirstOrDefaultAsync(pg => pg.PlayerId == playerId &&
                                       pg.GameId == gameId);
    }

    public async Task<List<Player>> GetPlayersByTeamIdAsync(int teamId)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.TeamId == teamId)
            .Include(tp => tp.Player)
            .Select(tp => tp.Player)
            .ToListAsync();
    }

        public async Task<List<PlayerGame>> GetTeamPlayersGameStatsAsync(int teamId)
        {
            var playerIds = await _context.TeamPlayers
                .Where(tp => tp.TeamId == teamId)
                .Select(tp => tp.PlayerId)
                .ToListAsync();

        return await _context.PlayerGame
            .Where(pg => playerIds.Contains(pg.PlayerId))
            .ToListAsync();
    }

        public async Task<List<Player>> GetFreeAgentsAsync(int leagueId)
        {
            return await _context.Players
                .Include(p => p.TeamPlayers)
                .Where(p => !p.TeamPlayers.Any(tp => tp.Team.LeagueId == leagueId))
                .ToListAsync();
        }

        public async Task<List<PlayerGame>> GetFreeAgentsGameStats(int leagueId)
        {
            var playerIdsWithoutTeam = await _context.Players
                .Include(p => p.TeamPlayers)
                .Where(p => !p.TeamPlayers.Any(tp => tp.Team.LeagueId == leagueId))
                .Select(p => p.PlayerId)
                .ToListAsync();

        return await _context.PlayerGame
            .Where(pg => playerIdsWithoutTeam.Contains(pg.PlayerId))
            .ToListAsync();
    }

    public async Task<List<Player>> GetPlayersByLeagueIdAsync(int leagueId)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.Team.LeagueId == leagueId)
            .Include(tp => tp.Player)
            .Select(tp => tp.Player)
            .ToListAsync();
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await _context.Players.ToListAsync();
    }

    public async Task<List<PlayerGame>> GetAllPlayerGameStatsAsync()
    {
        return await _context.PlayerGame.ToListAsync();
    }
}
