using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Domain;

public class TradeOrchestrator(
    IContractRepository contractRepository,
    ITeamRepository teamRepository,
    IBudgetRepository budgetRepository,
    BudgetEvaluator budgetEvaluator)
{
    private readonly IContractRepository _contractRepository = contractRepository;
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly IBudgetRepository _budgetRepository = budgetRepository;
    private readonly BudgetEvaluator _budgetEvaluator = budgetEvaluator;
}
