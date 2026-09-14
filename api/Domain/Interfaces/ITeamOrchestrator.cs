using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Domain.Interfaces;

public interface ITeamOrchestrator
{
    Task<Data.Models.Budget?> GetBudget(int teamId, CancellationToken cancellationToken);
    Task<bool> UpdateBudget(Data.Models.Budget budget, CancellationToken cancellationToken);
    Task<Team> CreateTeamInLeague(int leagueId, DraftOutcome draftOutcome, CancellationToken cancellationToken);
}
