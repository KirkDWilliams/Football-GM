using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class BudgetRepository
{
    private AppDbContext _context;
    public BudgetRepository(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Budget> GetTeamBudgetAsync(int teamId)
    {
        return await _context.Budgets.FirstOrDefaultAsync(b => b.TeamId == teamId)
            ?? throw new Exception($"Budget does not exist for teamId {teamId}");
    }

    public async Task<List<Budget>> GetAllTeamBudgetsAsync(int leagueId)
    {
        var teamIds = await GetTeamIdsInLeagueAsync(leagueId);

        return await _context.Budgets
            .Where(b => teamIds.Contains(b.TeamId))
            .ToListAsync()
            ?? throw new Exception($"Budgets do not exist for leagueId {leagueId}");
    }

    private async Task<List<int>> GetTeamIdsInLeagueAsync(int leagueId)
    {
        return await _context.Teams
            .Where(t => t.LeagueId == leagueId)
            .Select(t => t.TeamId)
            .ToListAsync()
        ?? throw new Exception("No teams found for the given leagueId");
    }
}
