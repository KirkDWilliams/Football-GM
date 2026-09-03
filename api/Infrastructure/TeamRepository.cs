using FootballGm.Api.Data;
using Microsoft.EntityFrameworkCore;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Infrastructure;

public interface ITeamRepository
{
    Task<List<Team>> GetTeamsByLeagueId(int leagueId, CancellationToken cancellationToken);
    Task<TeamPlayers> GetTeamPlayerByPlayerId(string playerId, CancellationToken cancellationToken);
    Task<List<TeamPlayers>> GetTeamPlayersByTeamId(int teamId, CancellationToken cancellationToken);
    Task<List<Team>> AddTeamsToLeagueAsync(List<Team> teams, CancellationToken cancellationToken);
    Task<Team> UpdateTeam(Team team, CancellationToken cancellationToken);
}

public class TeamRepository : ITeamRepository
{
    private AppDbContext _context;

    public TeamRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<List<Team>> GetTeamsByLeagueId(int leagueId, CancellationToken cancellationToken)
    {
        return await _context.Teams
            .Where(t => t.LeagueId == leagueId).ToListAsync(cancellationToken)
        ?? throw new Exception("No teams found for the given leagueId.");
    }

    public async Task<TeamPlayers> GetTeamPlayerByPlayerId(string playerId, CancellationToken cancellationToken)
    {
        return await _context.TeamPlayers.FirstOrDefaultAsync(tp => tp.PlayerId == playerId, cancellationToken)
        ?? throw new Exception("No player exists for the given playerId.");
    }

    public async Task<List<TeamPlayers>> GetTeamPlayersByTeamId(int teamId, CancellationToken cancellationToken)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.TeamId == teamId).ToListAsync(cancellationToken)
        ?? throw new Exception("No Players found belonging to the given teamId.");
    }

    public async Task<List<Team>> AddTeamsToLeagueAsync(List<Team> teams, CancellationToken cancellationToken)
    {
        try
        {
            _context.Teams.AddRange(teams);
            var changed = await _context.SaveChangesAsync(cancellationToken);

            if (changed != teams.Count)
                throw new Exception("Not all Teams were saved to league.");

            return teams;
        }
        catch
        {
            throw new Exception("Failed to add new team to league.");
        }
    }

    public async Task<Team> UpdateTeam(Team team, CancellationToken cancellationToken)
    {
        try
        {
            _context.Teams.Update(team);
            var changed = await _context.SaveChangesAsync(cancellationToken);

            if (changed == 0)
                throw new Exception("Team not updated.");

            return team;
        }
        catch
        {
            throw new Exception("Failed to add new team to league.");
        }
    }
}
