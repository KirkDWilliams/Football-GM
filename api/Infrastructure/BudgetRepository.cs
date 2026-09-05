using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class BudgetRepository(AppDbContext context) : IBudgetRepository
{
    public Task<Budget?> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return context.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.TeamId == teamId, cancellationToken);
    }

    public Task<List<Budget>> GetByLeagueIdAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return context.Budgets
            .AsNoTracking()
            .Where(b => b.LeagueId == leagueId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        int teamId,
        decimal[] paymentSchedule,
        CancellationToken cancellationToken = default)
    {
        var existing = await context.Budgets
            .FirstOrDefaultAsync(b => b.TeamId == teamId, cancellationToken);

        if (existing is null) return false;

        existing.PaymentSchedule = paymentSchedule;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
