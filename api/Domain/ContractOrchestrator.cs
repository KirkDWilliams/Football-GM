using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Infrastructure.Interfaces;
using Entities = FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Domain;

public class ContractOrchestrator(IContractRepository contractRepository) : IContractOrchestrator
{
    public async Task<List<Data.Models.Contract>> GetTeamContracts(
        int leagueId,
        int teamId,
        CancellationToken cancellationToken = default)
    {
        var contracts = await contractRepository.GetByTeamAsync(leagueId, teamId, cancellationToken);
        return contracts.Select(Data.Models.Contract.FromEntity).ToList();
    }

    public async Task<Data.Models.Contract?> GetContract(
        int leagueId,
        int teamId,
        string playerId,
        CancellationToken cancellationToken = default)
    {
        var contract = await contractRepository.GetByPlayerAsync(leagueId, teamId, playerId, cancellationToken);
        return contract is null ? null : Data.Models.Contract.FromEntity(contract);
    }

    public async Task<List<Data.Models.Contract>> CreateContractsForTeam(
        int leagueId,
        Team team,
        DraftOutcome draftOutcome,
        CancellationToken cancellationToken)
    {
        var contractsSaved = new List<Data.Models.Contract>();

        foreach (var playerContract in draftOutcome.DraftedPlayers)
        {
            var created = await contractRepository.AddAsync(
                leagueId,
                team.TeamId,
                playerContract.Key,
                ToEntity(playerContract.Value),
                cancellationToken);

            if (created is null)
                continue;

            contractsSaved.Add(Data.Models.Contract.FromEntity(created));
        }

        return contractsSaved;
    }

    public async Task<Data.Models.Contract?> CreateContract(
        int leagueId,
        int teamId,
        string playerId,
        Data.Models.Contract contract,
        CancellationToken cancellationToken = default)
    {
        var created = await contractRepository.AddAsync(
            leagueId,
            teamId,
            playerId,
            ToEntity(contract),
            cancellationToken);

        return created is null ? null : Data.Models.Contract.FromEntity(created);
    }

    public async Task<bool> ExtendContract(
        Data.Models.Contract contract,
        CancellationToken cancellationToken = default)
    {
        var updated = await contractRepository.UpdateAsync(ToEntity(contract), cancellationToken);
        return updated is not null;
    }

    public async Task<bool> DropContract(
        Data.Models.Contract contract,
        CancellationToken cancellationToken = default)
    {
        var terminated = ToEntity(contract);
        terminated.Salary = 0;
        terminated.GiftedCapSpace = 0;

        var updated = await contractRepository.UpdateAsync(terminated, cancellationToken);
        return updated is not null;
    }

    private static Entities.Contract ToEntity(Data.Models.Contract contract)
    {
        return new Entities.Contract
        {
            ContractId = contract.ContractId,
            StartWeek = contract.StartWeek,
            EndWeek = contract.EndWeek,
            SigningBonus = contract.SigningBonus,
            Salary = contract.Salary,
            GiftedCapSpace = contract.GiftedCapSpace
        };
    }
}
