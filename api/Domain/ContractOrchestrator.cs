using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure;

namespace FootballGm.Api.Domain;

public class ContractOrchestrator
{
    private readonly IContractRepository _contractRepository;

    public ContractOrchestrator(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<List<Contract>> GetTeamContractsAsync(int leagueId, int teamId)
    {
        var contracts = await _contractRepository.GetContractsByTeamIdAsync(leagueId, teamId);
        return contracts;
    }

    public async Task<Contract> GetContractAsync(int leagueId, int teamId, string playerId)
    {
        var contract = await _contractRepository.GetContractByPlayerIdAsync(leagueId, teamId, playerId);
        return contract;
    }

    public async Task<bool> CreateContractAsync(int leagueId, int teamId, string playerId, Contract contract)
    {
        var result = await _contractRepository.CreateContract(leagueId, teamId, playerId, contract);
        return result;
    }

    public async Task<bool> ExtendContractAsync(Contract contract)
    {
        var result = await _contractRepository.UpdateContract(contract);
        return result;
    }

    public async Task<bool> DeleteContractAsync(Contract contract)
    {
        var result = await _contractRepository.DeleteContractsAsync([contract]);
        return result;
    }

    public async Task<bool> DeleteContractsAsync(List<Contract> contracts)
    {
        var result = await _contractRepository.DeleteContractsAsync(contracts);
        return result;
    }
}
