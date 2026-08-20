using FootballGm.Api.Data;
using Microsoft.EntityFrameworkCore;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Infrastructure;

public class TeamRepository
{
    private AppDbContext _context;

    public TeamRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<List<Team>> GetTeamsByLeagueId(int leagueId)
    {
        return await _context.Teams
            .Where(t => t.LeagueId == leagueId).ToListAsync()
        ?? throw new Exception("No teams found for the given leagueId.");
    }

    public async Task<TeamPlayers> GetTeamPlayerByPlayerId(string playerId)
    {
        return await _context.TeamPlayers.FirstOrDefaultAsync(tp => tp.PlayerId == playerId)
        ?? throw new Exception("No player exists for the given playerId.");
    }

    public async Task<List<TeamPlayers>> GetTeamPlayersByTeamId(int teamId)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.TeamId == teamId).ToListAsync()
        ?? throw new Exception("No Players found belonging to the given teamId.");
    }

    public async Task AddTeamToLeagueAsync(Team team)
    {
        try
        {
            await _context.Teams.AddAsync(team);
        }
        catch
        {
            throw new Exception("Failed to add new team to league.");
        }
    }
}
