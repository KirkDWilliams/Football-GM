using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain;

public interface ITeamOrchestrator
{
    Task<Data.Models.Budget> GetBudget(int teamId, CancellationToken cancelToken);
    Task<bool> UpdateBudget(Data.Models.Budget budget, CancellationToken cancelToken);
    Task<List<Team>> CreateTeamsInLeague(int leagueId, List<DraftOutcome> draftOutcome, CancellationToken cancellationToken);
}

public class TeamOrchestrator : ITeamOrchestrator
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITeamRepository _teamRepository;
    public TeamOrchestrator(IBudgetRepository budgetRepository, ITeamRepository teamRepository)
    {
        _budgetRepository = budgetRepository;
        _teamRepository = teamRepository;
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

    public async Task<List<Team>> CreateTeamsInLeague(int leagueId, List<DraftOutcome> draftOutcomes, CancellationToken cancellationToken)
    {
        List<Team> leagueTeams = [];

        foreach (var draftOutcome in draftOutcomes)
        { 
            var team = new Team
            {
                LeagueId = leagueId,
                Name = draftOutcome.TeamName,
                User = draftOutcome.User
            };

            leagueTeams.Add(team);
        }

        return await _teamRepository.AddTeamsToLeagueAsync(leagueTeams, cancellationToken);

        //TODO: figure out why I thought there was more work to be done here...
        // I think it had to do with seeding the inactive players on the roster...
    }
}
