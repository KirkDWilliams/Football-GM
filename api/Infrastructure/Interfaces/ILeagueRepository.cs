using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Infrastructure.Interfaces;

public sealed record LeagueMembership(League League, LeagueMemberRole Role);

public interface ILeagueRepository
{
    Task<League> AddAsync(League league, CancellationToken cancellationToken = default);
    Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<League?> GetByCodeAsync(string leagueCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByJoinCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(int leagueId, string userId, CancellationToken cancellationToken = default);
    Task<LeagueMember> AddMemberAsync(LeagueMember leagueMember, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeagueMembership>> ListForUserAsync(
        string userId,
        CancellationToken cancellationToken = default);
    Task<LeagueMembership?> GetMembershipAsync(
        int leagueId,
        string userId,
        CancellationToken cancellationToken = default);
}
