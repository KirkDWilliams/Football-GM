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

    public int ExpirationMinutes { get; set; } = 60;
}
