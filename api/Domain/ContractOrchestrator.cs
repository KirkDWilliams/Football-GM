using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain;

public class ContractOrchestrator
{
    private readonly IContractRepository _contractRepository;

    public ContractOrchestrator(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<List<Data.Models.Contract>> GetTeamContracts(int leagueId, int teamId, CancellationToken cancellationToken = default)
    {
        List<Data.Models.Contract> teamContracts = [];
        var contracts = await _contractRepository.GetContractsByTeamIdAsync(leagueId, teamId, cancellationToken);

        foreach(var contract in contracts)
        {
            teamContracts.Add(Data.Models.Contract.FromEntity(contract));
        }

        return teamContracts;
    }

    public async Task<Data.Models.Contract> GetContract(int leagueId, int teamId, string playerId, CancellationToken cancellationToken = default)
    {
        var contract = await _contractRepository.GetContractByPlayerIdAsync(leagueId, teamId, playerId, cancellationToken);
        return Data.Models.Contract.FromEntity(contract);
    }

    public async Task<bool> CreateContracts(int leagueId, List<Team> teams, List<DraftOutcome> draftOutcomes, CancellationToken cancellationToken)
    {
        foreach (var drafting in draftOutcomes)
        {
            var teamId = teams.First(t => t.User.Id == drafting.User.Id).TeamId;
            foreach (var playerContract in drafting.DraftedPlayers)
            {
                var created = await _contractRepository.CreateContractAsync(leagueId, teamId, playerContract.Key, playerContract.Value, cancellationToken);
                if (created is false)
                    return false;
            }
        }

        return true;
    }

    public async Task<bool> CreateContract(int leagueId, int teamId, string playerId, Data.Models.Contract contract, CancellationToken cancellationToken = default)
    {
        var result = await _contractRepository.CreateContractAsync(leagueId, teamId, playerId, contract, cancellationToken);
        return result;
    }

    public async Task<bool> ExtendContract(Data.Models.Contract contract, CancellationToken cancellationToken = default)
    {
        var result = await _contractRepository.UpdateContractAsync(contract, cancellationToken);
        return result;
    }

    public async Task<bool> DropContract(Data.Models.Contract contract, CancellationToken cancellationToken = default)
    {
        var terminatedContract = new Data.Models.Contract
        {
            Salary = 0,
            GiftedCapSpace = 0,
            SigningBonus = contract.SigningBonus,
            ContractId = contract.ContractId,
            EndWeek = contract.EndWeek,
            StartWeek = contract.StartWeek,
        };

        var result = await _contractRepository.UpdateContractAsync(terminatedContract, cancellationToken);
        return result;
    }
}
