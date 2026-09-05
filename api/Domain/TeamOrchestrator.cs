using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Domain;

public class TeamOrchestrator(
    IBudgetRepository budgetRepository,
    ITeamRepository teamRepository) : ITeamOrchestrator
{
    public async Task<Data.Models.Budget?> GetBudget(int teamId, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetByTeamIdAsync(teamId, cancellationToken);
        return budget is null ? null : Data.Models.Budget.FromEntity(budget);
    }

    public Task<bool> UpdateBudget(Data.Models.Budget budget, CancellationToken cancellationToken)
    {
        return budgetRepository.UpdateAsync(budget.TeamId, budget.PaymentSchedule, cancellationToken);
    }

    public Task<Team> CreateTeamInLeague(
        int leagueId,
        DraftOutcome draftOutcome,
        CancellationToken cancellationToken)
    {
        var team = new Team
        {
            LeagueId = leagueId,
            UserId = draftOutcome.User.Id,
            Name = draftOutcome.TeamName
        };

        return teamRepository.AddAsync(team, cancellationToken);
    }
}
