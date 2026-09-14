using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface IContractRepository
{
    Task<Contract?> GetByPlayerAsync(
        int leagueId,
        int teamId,
        string playerId,
        CancellationToken cancellationToken = default);

    Task<List<Contract>> GetByTeamAsync(
        int leagueId,
        int teamId,
        CancellationToken cancellationToken = default);

    Task<Contract?> AddAsync(
        int leagueId,
        int teamId,
        string playerId,
        Contract contract,
        CancellationToken cancellationToken = default);

    Task<Contract?> UpdateAsync(Contract contract, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        IReadOnlyCollection<int> contractIds,
        CancellationToken cancellationToken = default);
}
