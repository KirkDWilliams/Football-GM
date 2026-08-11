namespace FootballGm.Api.Data.Entity;

/// <summary>
/// Server-side refresh session. Only a one-way hash of the opaque token is stored.
/// </summary>
public class RefreshToken
{
    public string Id { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    /// <summary>SHA-256 hash (hex) of the opaque refresh token sent to the client.</summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset ExpiresAtUtc { get; set; }

    /// <summary>When set, this refresh token can no longer be used.</summary>
    public DateTimeOffset? RevokedAtUtc { get; set; }

    /// <summary>Id of the token that replaced this one after rotation, if any.</summary>
    public string? ReplacedByTokenId { get; set; }

    /// <summary>Optional client-supplied device label.</summary>
    public string? DeviceName { get; set; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTimeOffset.UtcNow;
}
