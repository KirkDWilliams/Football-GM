using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<List<Budget>> GetByLeagueIdAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int teamId, decimal[] paymentSchedule, CancellationToken cancellationToken = default);
}
