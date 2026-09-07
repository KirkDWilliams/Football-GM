using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure.Interfaces;
using FootballGm.Api.Services;
using FootballGm.Api.Domain.Interfaces;
using Entities = FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Domain;

public class LeagueOrchestrator(ILeagueRepository repository, ILeagueCodeService codeService) : ILeagueOrchestrator
{
    public async Task<League> CreateLeague(
        string userId,
        League league,
        CancellationToken cancellationToken)
    {
        var joinCode = await codeService.GenerateUniqueJoinCodeAsync(cancellationToken);
        var saved = await repository.AddAsync(league.ToEntity(joinCode, userId), cancellationToken);
        return League.FromEntity(saved);
    }

    public async Task<JoinLeagueResult> JoinLeague(
        string userId,
        string leagueCode,
        CancellationToken cancellationToken)
    {
        var league = await repository.GetByCodeAsync(leagueCode, cancellationToken);
        if (league is null)
            return new JoinLeagueResult(JoinLeagueStatus.NotFound);

        if (await repository.IsMemberAsync(league.LeagueId, userId, cancellationToken))
            return new JoinLeagueResult(JoinLeagueStatus.AlreadyMember, League.FromEntity(league));

        await repository.AddMemberAsync(
            Entities.LeagueMember.Create(userId, LeagueMemberRole.Member, league.LeagueId),
            cancellationToken);

        return new JoinLeagueResult(JoinLeagueStatus.Joined, League.FromEntity(league));
    }

    public async Task<IReadOnlyList<LeagueSummary>> GetMyLeagues(
        string userId,
        CancellationToken cancellationToken)
    {
        var memberships = await repository.ListForUserAsync(userId, cancellationToken);

        return
        [
            .. memberships.Select(membership =>
                LeagueSummary.From(membership.League, membership.Role))
        ];
    }

    public async Task<GetLeagueResult> GetLeague(
        string userId,
        int leagueId,
        CancellationToken cancellationToken)
    {
        var membership = await repository.GetMembershipAsync(leagueId, userId, cancellationToken);
        if (membership is null)
            return new GetLeagueResult(GetLeagueStatus.NotFound);

        return new GetLeagueResult(
            GetLeagueStatus.Found,
            LeagueDetails.From(membership.League, membership.Role));
    }
}

public enum JoinLeagueStatus
{
    Joined,
    NotFound,
    AlreadyMember
}

public sealed record JoinLeagueResult(JoinLeagueStatus Status, League? League = null);

public enum GetLeagueStatus
{
    Found,
    NotFound
}

public sealed record GetLeagueResult(GetLeagueStatus Status, LeagueDetails? Details = null);
