using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Helpers;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class PlayerRepository(AppDbContext context) : IPlayerRepository
{
    public Task<Player?> GetPlayerByIdAsync(string playerId, CancellationToken cancellationToken = default)
    {
        return context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PlayerId == playerId, cancellationToken);
    }

    public Task<PlayerGame?> GetPlayerGameStatsAsync(
        string playerId,
        string gameId,
        CancellationToken cancellationToken = default)
    {
        return context.PlayerGame
            .AsNoTracking()
            .FirstOrDefaultAsync(pg => pg.PlayerId == playerId && pg.GameId == gameId, cancellationToken);
    }

    public Task<List<PlayerGame>> GetRecentPlayerGamesAsync(
        string playerId,
        int count,
        CancellationToken cancellationToken = default)
    {
        var seasonPrefix = $"{WeekHelper.CurrentSeason}_";

        return context.PlayerGame
            .AsNoTracking()
            .Where(pg => pg.PlayerId == playerId && pg.GameId.StartsWith(seasonPrefix))
            .OrderByDescending(pg => pg.GameId)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<PlayerSeason?> GetPlayerSeasonStatsAsync(
        string playerId,
        short season,
        CancellationToken cancellationToken = default)
    {
        return context.PlayerSeason
            .AsNoTracking()
            .FirstOrDefaultAsync(ps => ps.PlayerId == playerId && ps.Season == season, cancellationToken);
    }

    public Task<List<Player>> GetPlayersByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .Where(tp => tp.TeamId == teamId)
            .Select(tp => tp.Player)
            .ToListAsync(cancellationToken);
    }

    public Task<List<PlayerGame>> GetTeamPlayersGameStatsAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .Where(tp => tp.TeamId == teamId)
            .Join(
                context.PlayerGame,
                tp => tp.PlayerId,
                pg => pg.PlayerId,
                (_, pg) => pg)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Player>> GetFreeAgentsAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return context.Players
            .AsNoTracking()
            .Where(p => !p.TeamPlayers.Any(tp => tp.Team.LeagueId == leagueId))
            .ToListAsync(cancellationToken);
    }

    public Task<List<PlayerGame>> GetFreeAgentsGameStatsAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return context.Players
            .AsNoTracking()
            .Where(p => !p.TeamPlayers.Any(tp => tp.Team.LeagueId == leagueId))
            .Join(
                context.PlayerGame,
                p => p.PlayerId,
                pg => pg.PlayerId,
                (_, pg) => pg)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Player>> GetPlayersByLeagueIdAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .Where(tp => tp.Team.LeagueId == leagueId)
            .Select(tp => tp.Player)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Player>> GetAllPlayersAsync(CancellationToken cancellationToken = default)
    {
        return context.Players.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<List<PlayerGame>> GetAllPlayerGameStatsAsync(CancellationToken cancellationToken = default)
    {
        return context.PlayerGame.AsNoTracking().ToListAsync(cancellationToken);
    }
}
