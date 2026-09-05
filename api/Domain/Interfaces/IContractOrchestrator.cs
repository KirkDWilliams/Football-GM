using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Domain.Interfaces;

public interface IContractOrchestrator
{
    Task<List<Data.Models.Contract>> GetTeamContracts(
        int leagueId,
        int teamId,
        CancellationToken cancellationToken = default);

    Task<Data.Models.Contract?> GetContract(
        int leagueId,
        int teamId,
        string playerId,
        CancellationToken cancellationToken = default);

    Task<List<Data.Models.Contract>> CreateContractsForTeam(
        int leagueId,
        Team team,
        DraftOutcome draftOutcome,
        CancellationToken cancellationToken);

    Task<Data.Models.Contract?> CreateContract(
        int leagueId,
        int teamId,
        string playerId,
        Data.Models.Contract contract,
        CancellationToken cancellationToken = default);

    Task<bool> ExtendContract(Data.Models.Contract contract, CancellationToken cancellationToken = default);

    Task<bool> DropContract(Data.Models.Contract contract, CancellationToken cancellationToken = default);
}
