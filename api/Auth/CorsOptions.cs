namespace FootballGm.Api.Auth;

/// <summary>
/// Browser CORS settings for non-Development environments.
/// Bound from the "Cors" configuration section.
/// </summary>
public class CorsOptions
{
    public const string SectionName = "Cors";

    /// <summary>
    /// Explicit allowed origins (e.g. hosted Flutter web). Empty means no browser origins are allowed.
    /// Development uses a built-in localhost allow-list instead of this list.
    /// </summary>
    public string[] AllowedOrigins { get; set; } = [];
}
