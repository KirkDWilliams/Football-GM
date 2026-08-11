namespace FootballGm.Api.Services;

/// <summary>
/// Housekeeping for refresh sessions: per-user caps and deletion of dead rows.
/// </summary>
public interface IRefreshTokenMaintenance
{
    /// <summary>
    /// If the user has more active refresh sessions than configured, revokes the oldest ones.
    /// Call after inserting a new session for that user (same unit of work / before or after SaveChanges).
    /// </summary>
    Task EnforceActiveSessionLimitAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired tokens and revoked tokens past the retention window.
    /// Returns the number of rows removed.
    /// </summary>
    Task<int> CleanupAsync(CancellationToken cancellationToken = default);
}
