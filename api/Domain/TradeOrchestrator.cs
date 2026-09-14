using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Domain;

public class TradeOrchestrator(
    IContractRepository contractRepository,
    ITeamRepository teamRepository,
    IBudgetRepository budgetRepository)
{
}
