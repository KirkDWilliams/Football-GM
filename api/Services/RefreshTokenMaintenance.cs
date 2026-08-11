using FootballGm.Api.Auth;
using FootballGm.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballGm.Api.Services;

public class RefreshTokenMaintenance(
    AppDbContext db,
    IOptions<JwtOptions> jwtOptions) : IRefreshTokenMaintenance
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public async Task EnforceActiveSessionLimitAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var maxActive = Math.Max(1, _options.MaxActiveRefreshTokensPerUser);
        var now = DateTimeOffset.UtcNow;

        // Load this user's tokens so Local includes DB rows + any pending Added/Modified entities.
        await db.RefreshTokens
            .Where(t => t.UserId == userId)
            .LoadAsync(cancellationToken);

        var active = db.RefreshTokens.Local
            .Where(t =>
                t.UserId == userId
                && t.RevokedAtUtc is null
                && t.ExpiresAtUtc > now
                && db.Entry(t).State != EntityState.Deleted)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ThenByDescending(t => t.Id)
            .ToList();

        if (active.Count <= maxActive)
        {
            return;
        }

        // Keep the newest MaxActive sessions; revoke the rest (oldest devices fall off).
        foreach (var excess in active.Skip(maxActive))
        {
            excess.RevokedAtUtc = now;
        }
    }

    public async Task<int> CleanupAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var retentionDays = Math.Max(0, _options.RefreshTokenCleanupRetentionDays);
        var revokeCutoff = now.AddDays(-retentionDays);

        // Expired: no longer usable. Revoked past retention: history only, safe to drop.
        var dead = await db.RefreshTokens
            .Where(t =>
                t.ExpiresAtUtc <= now
                || (t.RevokedAtUtc != null && t.RevokedAtUtc <= revokeCutoff))
            .ToListAsync(cancellationToken);

        if (dead.Count == 0)
        {
            return 0;
        }

        db.RefreshTokens.RemoveRange(dead);
        await db.SaveChangesAsync(cancellationToken);
        return dead.Count;
    }
}
