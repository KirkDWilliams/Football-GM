using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain;

public class BudgetEvaluator
{
    private readonly BudgetRepository _budgetRepo;
    private readonly ContractRepository _contractRep;
    private readonly TeamRepository _teamRepo;

    public BudgetEvaluator(
        BudgetRepository budgetRepository,
        ContractRepository contractRepository,
        TeamRepository teamRepository)
    {
        _budgetRepo = budgetRepository;
        _contractRep = contractRepository;
        _teamRepo = teamRepository;
    }

   /* public async Task<bool> ExactObligationsFromTeamBudgetsAsync(int leagueId)
    {
        var teams = await _teamRepo.GetTeamsByLeagueId(leagueId);

        foreach (var team in teams)
        {
            var teamBudgetTask = _budgetRepo.GetTeamBudgetAsync(team.TeamId);
            var teamContractsTask = _contractRep.GetContractsByTeamIdAsync(team.TeamId);

            _ = Task.WhenAll(teamBudgetTask, teamContractsTask);

            var budget = await teamBudgetTask;
            var contracts = await teamContractsTask;

            var capObligations = decimal.Zero;

           foreach (var contract in contracts)
            {
                var poop = contract.
            }


            var poop = new Budget
            {
                TeamId = shit.TeamId,
                CurrentObligations = shit.FutureObligations,
                FutureObligations = CalculateObligations()
            }
        }

        return true;
    }*/
}
