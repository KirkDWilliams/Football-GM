using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain;

public class TradeOrchestrator(
    IContractRepository contractRepository,
    ITeamRepository teamRepository,
    IBudgetRepository budgetRepository)
{
}
