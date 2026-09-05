using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface ILeagueRepository
{
    Task<League> AddAsync(League league, CancellationToken cancellationToken = default);
    Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<League?> GetByCodeAsync(string leagueCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByJoinCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> IsMemberAsync(int leagueId, string userId, CancellationToken cancellationToken);
    Task<LeagueMember> AddMemberAsync(LeagueMember leagueMember, CancellationToken cancellationToken);
}
