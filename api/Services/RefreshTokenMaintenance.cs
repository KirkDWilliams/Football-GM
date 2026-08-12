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

    public async Task RevokeAllForUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var now = DateTimeOffset.UtcNow;

        var active = await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var token in active)
        {
            token.RevokedAtUtc = now;
        }
    }

    public async Task<int> CleanupAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var retentionDays = Math.Max(0, _options.RefreshTokenCleanupRetentionDays);
        var revokeCutoff = now.AddDays(-retentionDays);

        // SQLite + nullable DateTimeOffset OR predicates often fail translation; evaluate in memory.
        // Table stays small thanks to caps + periodic cleanup.
        var all = await db.RefreshTokens.AsNoTracking().ToListAsync(cancellationToken);
        var deadIds = all
            .Where(t =>
                t.ExpiresAtUtc <= now
                || (t.RevokedAtUtc is not null && t.RevokedAtUtc <= revokeCutoff))
            .Select(t => t.Id)
            .ToList();

        if (deadIds.Count == 0)
        {
            return 0;
        }

        var dead = await db.RefreshTokens
            .Where(t => deadIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        db.RefreshTokens.RemoveRange(dead);
        await db.SaveChangesAsync(cancellationToken);
        return dead.Count;
    }
}
