using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class ContractRepository
{
    private readonly AppDbContext _context;

    public ContractRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contract> GetContractByPlayerIdAsync(string playerId)
    {
        var contractId = await _context.TeamPlayers
            .Where(tp => tp.PlayerId == playerId)
            .Select(tp => tp.ContractId)
            .FirstOrDefaultAsync();

        return await _context.Contracts
            .Where(c => c.ContractId == contractId)
            .FirstOrDefaultAsync()
            ?? throw new Exception("No contract exists for the given playerId.");
    }

    public async Task<List<Contract>> GetContractsByTeamIdAsync(int teamId)
    {
        return await _context.TeamPlayers
            .Where(tp => tp.TeamId == teamId)
            .Include(tp => tp.Contract)
            .Select(tp => tp.Contract)
            .ToListAsync();
    }

    public async Task<bool> SaveContract(Contract contract)
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

        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteContractsAsync(List<Contract> contracts)
    {
        try
        {
            _context.Contracts.RemoveRange(contracts);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    //This method will live elsewhere ... I got a little carried away with ideas and bored making repo methods.
    public async Task<bool> RunCompletionOfContractsAsync()
    {
        try
        {
            var contracts = await _context.Contracts
                .Where(c => c.EndWeek == WeekHelper.CurrentWeek).ToListAsync();

            foreach (var contract in contracts)
            {
                var teamPlayer = await _context.TeamPlayers
                    .FirstOrDefaultAsync(tp => tp.ContractId == contract.ContractId)
                    ?? throw new InvalidOperationException($"No TeamPlayers entry found for ContractId {contract.ContractId}");

                _context.TeamPlayers.Remove(teamPlayer);
            }

            await DeleteContractsAsync(contracts);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
