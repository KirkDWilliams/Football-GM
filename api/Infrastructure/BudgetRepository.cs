using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public interface IBudgetRepository
{
    Task<Budget> GetTeamBudgetAsync(int teamId);

    Task<List<Budget>> GetTeamBudgetsAsync(int leagueId, List<int> teamIds);
}

public class BudgetRepository : IBudgetRepository
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

    public async Task<List<Budget>> GetTeamBudgetsAsync(int leagueId, List<int> teamIds)
    {
        return await _context.Budgets
            .Where(b => teamIds.Contains(b.TeamId))
            .ToListAsync()
            ?? throw new Exception($"Budgets do not exist for leagueId {leagueId}");
    }
}
