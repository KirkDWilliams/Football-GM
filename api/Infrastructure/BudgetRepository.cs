using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public interface IBudgetRepository
{
    Task<Budget> GetTeamBudgetAsync(int teamId, CancellationToken cancellationToken);

    Task<List<Budget>> GetLeagueBudgetsAsync(int leagueId, CancellationToken cancellationToken);

    Task<bool> UpdateBudgetAsync(Data.Models.Budget budget, CancellationToken cancellationToken);
}

public class BudgetRepository : IBudgetRepository
{
    private AppDbContext _context;
    public BudgetRepository(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Budget> GetTeamBudgetAsync(int teamId, CancellationToken cancellationToken)
    {
        return await _context.Budgets.FirstOrDefaultAsync(b => b.TeamId == teamId, cancellationToken)
            ?? throw new Exception($"Budget does not exist for teamId {teamId}");
    }

    public async Task<List<Budget>> GetLeagueBudgetsAsync(int leagueId, CancellationToken cancellationToken)
    {
        return await _context.Budgets
            .Where(b => b.LeagueId == leagueId)
            .ToListAsync(cancellationToken)
            ?? throw new Exception($"Budgets do not exist for leagueId {leagueId}");
    }

    public async Task<bool> UpdateBudgetAsync(Data.Models.Budget budget, CancellationToken cancellationToken)
    {
        var updatedBudget = new Budget { TeamId = budget.TeamId, PaymentSchedule = budget.PaymentSchedule };

        _context.Budgets.Update(updatedBudget);
        var changed = await _context.SaveChangesAsync(cancellationToken);

        return changed > 0;
    }
}
