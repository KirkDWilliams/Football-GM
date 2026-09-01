using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Associations;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public interface IContractRepository
{
    Task<Data.Entity.Contrived.Contract> GetContractByPlayerIdAsync(int leagueId, int teamId, string playerId, CancellationToken cancellationToken = default);
    Task<List<Data.Entity.Contrived.Contract>> GetContractsByTeamIdAsync(int leagueId, int teamId, CancellationToken cancellationToken = default);
    Task<bool> CreateContractAsync(int leagueId, int teamId, string playerId, Data.Models.Contract contract, CancellationToken cancellationToken = default);
    Task<bool> UpdateContractAsync(Data.Models.Contract contract, CancellationToken cancellationToken = default);
    Task<bool> DeleteContractsAsync(List<Data.Models.Contract> contracts, CancellationToken cancellationToken = default);
    //TODO: this needs to live elsewhere-> Task<bool> RunCompletionOfContractsAsync(CancellationToken cancellationToken = default);
}

public class ContractRepository : IContractRepository
{
    private readonly AppDbContext _context;

    public ContractRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Data.Entity.Contrived.Contract> GetContractByPlayerIdAsync(int leagueId, int teamId, string playerId, CancellationToken cancellationToken = default)
    {
        var contractId = await _context.TeamPlayers
            .Where(tp => tp.LeagueId == leagueId && tp.TeamId == teamId && tp.PlayerId == playerId)
            .Select(tp => tp.ContractId)
            .FirstOrDefaultAsync(cancellationToken);

        return await _context.Contracts
            .Where(c => c.ContractId == contractId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new Exception("No contract exists for the given playerId.");
    }

    public async Task<List<Data.Entity.Contrived.Contract>> GetContractsByTeamIdAsync(int leagueId, int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.LeagueId == leagueId && tp.TeamId == teamId)
            .Include(tp => tp.Contract)
            .Select(tp => tp.Contract)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CreateContractAsync(int leagueId, int teamId, string playerId, Data.Models.Contract contract, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Contracts.Add(CreateEntity(contract));

            var teamPlayer = new TeamPlayers
            {
                LeagueId = leagueId,
                TeamId = teamId,
                PlayerId = playerId,
                ContractId = contract.ContractId
            };

            _context.TeamPlayers.Add(teamPlayer);
            var changed = await _context.SaveChangesAsync(cancellationToken);

            return changed > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateContractAsync(Data.Models.Contract contract, CancellationToken cancellationToken = default)
    {
        var existingContract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.ContractId == contract.ContractId, cancellationToken);

        if (existingContract == null)
            return false;

        _context.Contracts.Update(CreateEntity(contract));
        var changed = await _context.SaveChangesAsync(cancellationToken);
        return changed > 0;
    }

    public async Task<bool> DeleteContractsAsync(List<Data.Models.Contract> contracts, CancellationToken cancellationToken = default)
    {
        try
        {
            var contractsToDelete = new List<Data.Entity.Contrived.Contract>();

            foreach (var contract in contracts)
            {
                contractsToDelete.Add(CreateEntity(contract));
            }

            _context.Contracts.RemoveRange(contractsToDelete);
            var changed = await _context.SaveChangesAsync(cancellationToken);
            return changed > 0;
        }
        catch
        {
            return false;
        }
    }

    // this belongs elsewhere for the automation
/*    public async Task<bool> RunCompletionOfContractsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var contracts = await _context.Contracts
                .Where(c => c.EndWeek == WeekHelper.CurrentWeek).ToListAsync(cancellationToken);

            foreach (var contract in contracts)
            {
                var teamPlayer = await _context.TeamPlayers
                    .FirstOrDefaultAsync(tp => tp.ContractId == contract.ContractId, cancellationToken)
                    ?? throw new InvalidOperationException($"No TeamPlayers entry found for ContractId {contract.ContractId}");

                _context.TeamPlayers.Remove(teamPlayer);
            }

            await DeleteContractsAsync(contracts, cancellationToken);
            var changed = await _context.SaveChangesAsync(cancellationToken);

            return changed > 0;
        }
        catch
        {
            return false;
        };
    }*/

    private Data.Entity.Contrived.Contract CreateEntity(Data.Models.Contract contract)
    {
        return new Data.Entity.Contrived.Contract
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
