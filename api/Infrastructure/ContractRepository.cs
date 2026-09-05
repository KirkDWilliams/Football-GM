using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Associations;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class ContractRepository(AppDbContext context) : IContractRepository
{
    public Task<Contract?> GetByPlayerAsync(
        int leagueId,
        int teamId,
        string playerId,
        CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .Where(tp => tp.LeagueId == leagueId && tp.TeamId == teamId && tp.PlayerId == playerId)
            .Select(tp => tp.Contract)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Contract>> GetByTeamAsync(
        int leagueId,
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return context.TeamPlayers
            .AsNoTracking()
            .Where(tp => tp.LeagueId == leagueId && tp.TeamId == teamId)
            .Select(tp => tp.Contract)
            .ToListAsync(cancellationToken);
    }

    public async Task<Contract?> AddAsync(
        int leagueId,
        int teamId,
        string playerId,
        Contract contract,
        CancellationToken cancellationToken = default)
    {
        var exists = await context.TeamPlayers.AnyAsync(
            tp => tp.LeagueId == leagueId && tp.TeamId == teamId && tp.PlayerId == playerId,
            cancellationToken);

        if (exists) return null;

        context.Contracts.Add(contract);
        context.TeamPlayers.Add(new TeamPlayers
        {
            LeagueId = leagueId,
            TeamId = teamId,
            PlayerId = playerId,
            Contract = contract
        });

        await context.SaveChangesAsync(cancellationToken);
        return contract;
    }

    public async Task<Contract?> UpdateAsync(Contract contract, CancellationToken cancellationToken = default)
    {
        var existing = await context.Contracts
            .FirstOrDefaultAsync(c => c.ContractId == contract.ContractId, cancellationToken);

        if (existing is null) return null;

        existing.StartWeek = contract.StartWeek;
        existing.EndWeek = contract.EndWeek;
        existing.SigningBonus = contract.SigningBonus;
        existing.Salary = contract.Salary;
        existing.GiftedCapSpace = contract.GiftedCapSpace;
        await context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(
        IReadOnlyCollection<int> contractIds,
        CancellationToken cancellationToken = default)
    {
        if (contractIds.Count == 0) return false;

        var existing = await context.Contracts
            .Where(c => contractIds.Contains(c.ContractId))
            .ToListAsync(cancellationToken);

        if (existing.Count == 0) return false;

        context.Contracts.RemoveRange(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
