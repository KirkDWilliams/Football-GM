namespace FootballGm.Api.Auth;

/// <summary>
/// JWT signing and validation settings. Bound from the "Jwt" configuration section.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Symmetric signing key. Must be at least 32 characters for HS256.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>
    /// Access JWT lifetime in minutes. Prefer a short value; clients use refresh tokens for long sessions.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Opaque refresh token lifetime in days (stored hashed server-side).
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 30;

    /// <summary>
    /// Maximum concurrent active refresh sessions per user (devices/apps).
    /// When exceeded, the oldest active sessions are revoked.
    /// </summary>
    public int MaxActiveRefreshTokensPerUser { get; set; } = 10;

    /// <summary>
    /// How long to keep revoked refresh rows before deletion (days).
    /// Expired rows are always eligible for cleanup.
    /// </summary>
    public int RefreshTokenCleanupRetentionDays { get; set; } = 7;

    /// <summary>
    /// How often the background cleanup job runs (hours). Minimum 1.
    /// </summary>
    public int RefreshTokenCleanupIntervalHours { get; set; } = 6;
}
