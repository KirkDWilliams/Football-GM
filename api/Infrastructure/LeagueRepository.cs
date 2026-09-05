using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class LeagueRepository(AppDbContext context) : ILeagueRepository
{
    public async Task<League> AddAsync(League league, CancellationToken cancellationToken = default)
    {
        context.Leagues.Add(league);
        await context.SaveChangesAsync(cancellationToken);
        return league;
    }

    public async Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return await context.Leagues
            .Include(l => l.Settings)
                .ThenInclude(s => s.Rules)
            .FirstOrDefaultAsync(l => l.LeagueId == leagueId, cancellationToken);
    }

    public async Task<League?> GetByCodeAsync(string leagueCode, CancellationToken cancellationToken = default)
    {
        return await context.Leagues
            .Include(l => l.Settings)
                .ThenInclude(s => s.Rules)
            .FirstOrDefaultAsync(l => l.JoinCode == leagueCode, cancellationToken);
    }

    public async Task<bool> ExistsByJoinCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await context.Leagues
            .AnyAsync(l => l.JoinCode == code, cancellationToken);
    }

    public async Task<bool> IsMemberAsync(
        int leagueId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await context.LeagueMembers
            .AnyAsync(m => m.LeagueId == leagueId && m.UserId == userId, cancellationToken);
    }

    public async Task<LeagueMember> AddMemberAsync(
        LeagueMember leagueMember,
        CancellationToken cancellationToken)
    {
        context.LeagueMembers.Add(leagueMember);
        await context.SaveChangesAsync(cancellationToken);
        return leagueMember;
    }
}
