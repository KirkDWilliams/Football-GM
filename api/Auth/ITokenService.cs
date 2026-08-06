namespace FootballGm.Api.Auth;

public interface ITokenService
{
    /// <summary>
    /// Creates a signed JWT for the given subject. Does not look up or create user accounts.
    /// </summary>
    TokenResponse CreateToken(string subject, string? displayName = null);
}

public record TokenResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
