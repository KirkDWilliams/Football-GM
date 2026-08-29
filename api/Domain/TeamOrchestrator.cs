using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain;

public interface ITeamOrchestrator
{
    Task<Data.Models.Budget> GetBudget(int teamId, CancellationToken cancelToken);
    Task<bool> UpdateBudget(Data.Models.Budget budget, CancellationToken cancelToken);
}

public class TeamOrchestrator : ITeamOrchestrator
{
    private readonly IBudgetRepository _budgetRepository;
    public TeamOrchestrator(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Data.Models.Budget> GetBudget(int teamId, CancellationToken cancelToken)
    {
        var budget = await _budgetRepository.GetTeamBudgetAsync(teamId, cancelToken);

        return Data.Models.Budget.FromEntity(budget);
    }

    public async Task<bool> UpdateBudget(Data.Models.Budget budget, CancellationToken cancelToken)
    {
        return await _budgetRepository.UpdateBudgetAsync(budget, cancelToken);
    }
}
