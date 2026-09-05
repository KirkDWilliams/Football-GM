using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Associations;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class TeamRepository(AppDbContext context) : ITeamRepository
{
    public Task<List<Team>> GetByLeagueIdAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return context.Teams
            .AsNoTracking()
            .Where(t => t.LeagueId == leagueId)
            .ToListAsync(cancellationToken);
    }

    public Task<TeamPlayers?> GetTeamPlayerByPlayerIdAsync(
        string playerId,
        CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .FirstOrDefaultAsync(tp => tp.PlayerId == playerId, cancellationToken);
    }

    public Task<List<TeamPlayers>> GetTeamPlayersByTeamIdAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .Where(tp => tp.TeamId == teamId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);
        return team;
    }

    public async Task<Team?> UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        var existing = await context.Teams
            .FirstOrDefaultAsync(t => t.TeamId == team.TeamId, cancellationToken);

        if (existing is null) return null;

        existing.Name = team.Name;
        existing.UserId = team.UserId;
        existing.LeagueId = team.LeagueId;
        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }
}
