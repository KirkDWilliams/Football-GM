using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class ContractRepository
{
    private readonly AppDbContext _context;

    public ContractRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetContractByPlayerIdAsync(string playerId)
    {
        var contractId = await _context.TeamPlayers
            .Where(tp => tp.PlayerId == playerId)
            .Select(tp => tp.ContractId)
            .FirstOrDefaultAsync();

        if (contractId == 0)
            return null;

        return await _context.Contracts
            .Where(c => c.ContractId == contractId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Contract>> GetContractsByTeamIdAsync(int teamId)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.TeamId == teamId)
            .Include(tp => tp.Contract)
            .Select(tp => tp.Contract)
            .ToListAsync();
    }

    public async Task<bool> CreateContract(Contract contract)
    {
        try
        { 
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateContract(Contract contract)
    {
        var existingContract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.ContractId == contract.ContractId);
        if (existingContract == null)
            return false;

        // save over the existing contract with this new contract
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Contract> TradePlayerAsync(int departingTeamId, int receivingTeamId, string playerId)
    {
        var currentContract = await GetContractByPlayerIdAsync(playerId)
            ?? throw new Exception("Player does not have a contract.");

        var teamPlayer = await _context.TeamPlayers
            .FirstOrDefaultAsync(tp => tp.TeamId == departingTeamId &&
                                       tp.PlayerId == playerId &&
                                       tp.ContractId == currentContract.ContractId)
            ?? throw new InvalidOperationException("Player not found on departing team.");

        teamPlayer.TeamId = receivingTeamId;
        _context.TeamPlayers.Update(teamPlayer);

        await _context.SaveChangesAsync();

        return currentContract;
    }

    public async Task<bool> RunCompletionOfContractsAsync(int currentWeek)
    {
        var contracts = await _context.Contracts
            .Where(c => c.EndWeek == currentWeek)
            .ToListAsync();

        foreach (var contract in contracts)
        {
            var teamPlayer = await _context.TeamPlayers.FirstOrDefaultAsync(tp => tp.ContractId == contract.ContractId)
                ?? throw new InvalidOperationException($"No TeamPlayers entry found for ContractId {contract.ContractId}");

            _context.TeamPlayers.Remove(teamPlayer);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
