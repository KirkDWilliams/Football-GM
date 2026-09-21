using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using static FootballGm.Api.Infrastructure.LeagueQueryExtensions;

namespace FootballGm.Api.Infrastructure;

public class LeagueRepository(AppDbContext context) : ILeagueRepository
{
    public async Task<League> AddAsync(League league, CancellationToken cancellationToken = default)
    {
        context.Leagues.Add(league);
        await context.SaveChangesAsync(cancellationToken);
        return league;
    }

    public Task<League?> GetByIdAsync(
        int leagueId,
        LeagueIncludes includes = LeagueIncludes.Settings,
        CancellationToken cancellationToken = default)
    {
        var query = context.Leagues.AsNoTracking().AsSplitQuery();

        if (includes.HasFlag(LeagueIncludes.Settings))
            query = query.WithSettings();

        if (includes.HasFlag(LeagueIncludes.Teams))
            query = query.WithTeams();

        if (includes.HasFlag(LeagueIncludes.Members))
            query = query.WithMembers();

        return query.FirstOrDefaultAsync(l => l.LeagueId == leagueId, cancellationToken);
    }

    public Task<League?> GetByCodeAsync(string leagueCode, CancellationToken cancellationToken = default)
    {
        return context.Leagues.AsReadOnly().WithSettings().WithTeams()
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

        return members
            .Select(member => new LeagueMembership(member.League, member.Role))
            .ToList();
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

        var league = await GetByIdAsync(leagueId, LeagueIncludes.Settings, cancellationToken);
        return league is null
            ? null
            : new LeagueMembership(league, member.Role);
    }

    public async Task<IReadOnlyList<LeagueMember>> ListMembersAsync(
        int leagueId,
        CancellationToken cancellationToken = default)
    {
        return await context.LeagueMembers
            .AsNoTracking()
            .Include(member => member.User)
            .Where(member => member.LeagueId == leagueId)
            .OrderBy(member => member.JoinedAtUtc)
            .ToListAsync(cancellationToken);
    }
}

public static class LeagueQueryExtensions
{
    public static IQueryable<League> WithSettings(this IQueryable<League> query)
    {
        return query.Include(l => l.Settings)
            .ThenInclude(s => s.Rules);
    }

    public static IQueryable<League> WithTeams(this IQueryable<League> query)
    {
        return query.Include(l => l.Teams)
            .ThenInclude(t => t.User);
    }

    public static IQueryable<League> WithMembers(this IQueryable<League> query)
    {
        return query.Include(l => l.Members)
            .ThenInclude(m => m.User);
    }

    public static IQueryable<League> AsReadOnly(this IQueryable<League> query)
    {
        return query.AsNoTracking().AsSplitQuery();
    }

    [Flags]
    public enum LeagueIncludes
    {
        None = 1,
        Settings = 2,
        Teams = 3,
        Members = 4,
        All = Settings | Teams | Members,
    }
}
