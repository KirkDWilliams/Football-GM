using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain
{
    public class TradeOrchestrator
    {
        private readonly ContractRepository _contractRepo;
        private readonly TeamRepository _teamRepo;
        private readonly BudgetRepository _budgetRepo;
        private readonly BudgetEvaluator _budgetEvaluator;

        public TradeOrchestrator(
            ContractRepository contractRepository,
            TeamRepository teamRepository,
            BudgetRepository budgetRepository,
            BudgetEvaluator budgetEvaluator)
        {
            _contractRepo = contractRepository;
            _teamRepo = teamRepository;
            _budgetRepo = budgetRepository;
            _budgetEvaluator = budgetEvaluator;
        }

        public async Task<Contract> TradePlayer(int departingTeamId, int receivingTeamId, string playerId)
        {
            var contractTask = _contractRepo.GetContractByPlayerIdAsync(playerId);
            var teamPlayerTask = _teamRepo.GetTeamPlayerByPlayerId(playerId);
            var departingBudgetTask = _budgetRepo.GetTeamBudgetAsync(departingTeamId);
            var receivingBudgetTask = _budgetRepo.GetTeamBudgetAsync(receivingTeamId);

            await Task.WhenAll(contractTask, teamPlayerTask, departingBudgetTask, receivingBudgetTask);

            var currentContract = await contractTask;
            var teamPlayer = await teamPlayerTask;
            var departingTeamBudget = await departingBudgetTask;
            var receivingTeamBudget = await receivingBudgetTask;



            /*teamPlayer.TeamId = receivingTeamId;
            _context.TeamPlayers.Update(teamPlayer);
            currBudget.FutureObligations += (Decimal.Divide(priorContract.SigningBonus, priorContract.EndWeek - priorContract.StartWeek + 1) * (priorContract.EndWeek - WeekHelper.GetCurrentWeek()));

            await _context.SaveChangesAsync();*/

            return currentContract;
        }
    }
}
