using FootballGm.Api.Auth;

namespace FootballGm.Api.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
}

public record RegisterRequest(string Email, string Password, string DisplayName);

public record LoginRequest(string Email, string Password);

public record UserDto(string Id, string Email, string DisplayName);

public record AuthSuccess(TokenResponse Token, UserDto User);

/// <summary>
/// Result of register/login. Either <see cref="Success"/> is set, or <see cref="Error"/> describes the failure.
/// </summary>
public record AuthResult(AuthSuccess? Success, AuthError? Error)
{
    public static AuthResult Ok(AuthSuccess success) => new(success, null);

    public static AuthResult Fail(AuthError error) => new(null, error);
}

public record AuthError(AuthErrorCode Code, string Message);

public enum AuthErrorCode
{
    Validation,
    Conflict,
    InvalidCredentials,
}
