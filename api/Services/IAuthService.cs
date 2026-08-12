using FootballGm.Api.Auth;

namespace FootballGm.Api.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default);

    Task<LogoutResult> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);

    Task<ChangePasswordResult> ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
}

public record RegisterRequest(string Email, string Password, string DisplayName, string? DeviceName = null);

public record LoginRequest(string Email, string Password, string? DeviceName = null);

public record RefreshRequest(string RefreshToken);

public record LogoutRequest(string? RefreshToken);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record UserDto(string Id, string Email, string DisplayName);

public record AuthSuccess(
    TokenResponse AccessToken,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt,
    UserDto User);

/// <summary>
/// Result of register/login/refresh. Either <see cref="Success"/> is set, or <see cref="Error"/> describes the failure.
/// </summary>
public record AuthResult(AuthSuccess? Success, AuthError? Error)
{
    public static AuthResult Ok(AuthSuccess success) => new(success, null);

    public static AuthResult Fail(AuthError error) => new(null, error);
}

public record LogoutResult(bool Succeeded, AuthError? Error)
{
    public static LogoutResult Ok() => new(true, null);

    public static LogoutResult Fail(AuthError error) => new(false, error);
}

public record ChangePasswordResult(bool Succeeded, AuthError? Error)
{
    public static ChangePasswordResult Ok() => new(true, null);

    public static ChangePasswordResult Fail(AuthError error) => new(false, error);
}

public record AuthError(AuthErrorCode Code, string Message);

public enum AuthErrorCode
{
    Validation,
    Conflict,
    InvalidCredentials,
    InvalidRefreshToken,
}
