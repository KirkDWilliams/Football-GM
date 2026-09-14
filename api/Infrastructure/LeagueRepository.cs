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

    public Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return LeaguesWithSettings()
            .FirstOrDefaultAsync(l => l.LeagueId == leagueId, cancellationToken);
    }

    public Task<League?> GetByCodeAsync(string leagueCode, CancellationToken cancellationToken = default)
    {
        return LeaguesWithSettings()
            .FirstOrDefaultAsync(l => l.JoinCode == leagueCode, cancellationToken);
    }

    public Task<bool> ExistsByJoinCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return context.Leagues.AnyAsync(l => l.JoinCode == code, cancellationToken);
    }

    public Task<bool> IsMemberAsync(
        int leagueId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return context.LeagueMembers
            .AnyAsync(m => m.LeagueId == leagueId && m.UserId == userId, cancellationToken);
    }

    public async Task<LeagueMember> AddMemberAsync(
        LeagueMember leagueMember,
        CancellationToken cancellationToken = default)
    {
        context.LeagueMembers.Add(leagueMember);
        await context.SaveChangesAsync(cancellationToken);
        return leagueMember;
    }

    public async Task<IReadOnlyList<LeagueMembership>> ListForUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var members = await context.LeagueMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId)
            .Include(member => member.League)
                .ThenInclude(league => league.Settings)
                    .ThenInclude(settings => settings.Rules)
            .ToListAsync(cancellationToken);

        return
        [
            .. members.Select(member => new LeagueMembership(member.League, member.Role))
        ];
    }

    public async Task<LeagueMembership?> GetMembershipAsync(
        int leagueId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var member = await context.LeagueMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                membership => membership.LeagueId == leagueId && membership.UserId == userId,
                cancellationToken);

        if (member is null)
            return null;

        var league = await GetByIdAsync(leagueId, cancellationToken);
        if (league is null)
            return null;

        return new LeagueMembership(league, member.Role);
    }

    private IQueryable<League> LeaguesWithSettings()
    {
        return context.Leagues
            .AsNoTracking()
            .Include(l => l.Settings)
                .ThenInclude(s => s.Rules);
    }
}
