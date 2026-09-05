using FootballGm.Api.Data.Entity;
using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdReadOnlyAsync(string userId, CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenWithUserByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    void AddRefreshToken(RefreshToken token);

    /// <summary>
    /// Commits tracked changes. The caller owns the unit of work so session-limit
    /// maintenance can mutate the same context before this save.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
