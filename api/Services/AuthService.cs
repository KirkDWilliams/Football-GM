using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using FootballGm.Api.Auth;
using FootballGm.Api.Data.Entity;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FootballGm.Api.Services;

public class AuthService(
    IAuthRepository authRepository,
    IPasswordHasher<User> hasher,
    ITokenService tokenService,
    IRefreshTokenMaintenance refreshTokenMaintenance,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private const int MinPasswordLength = 8;

    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = ValidateRegister(request);
        if (validationError is not null) return AuthResult.Fail(validationError);

        var email = NormalizeEmail(request.Email);
        var existing = await authRepository.GetUserByEmailAsync(email, cancellationToken);
        if (existing is not null)
            return AuthResult.Fail(new AuthError(AuthErrorCode.Conflict, "An account with this email already exists."));

        var user = new User
        {
            Id = Guid.NewGuid().ToString("N"),
            Email = email,
            DisplayName = request.DisplayName.Trim(),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        await authRepository.AddUserAsync(user, cancellationToken);

        return AuthResult.Ok(await IssueSessionAsync(user, request.DeviceName, cancellationToken));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = ValidateLogin(request);
        if (validationError is not null) return AuthResult.Fail(validationError);

        var user = await authRepository.GetUserByEmailAsync(NormalizeEmail(request.Email), cancellationToken);
        // Same generic failure whether the email is missing or the password is wrong.
        if (user is null || !TryVerifyPassword(user, request.Password, out var rehashNeeded))
            return AuthResult.Fail(InvalidCredentials());

        if (rehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, request.Password);
            await authRepository.SaveChangesAsync(cancellationToken);
        }

        return AuthResult.Ok(await IssueSessionAsync(user, request.DeviceName, cancellationToken));
    }

    public async Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = ValidateRefreshToken(request.RefreshToken);
        if (validationError is not null) return AuthResult.Fail(validationError);

        var existing = await authRepository.GetRefreshTokenWithUserByHashAsync(
            HashPresentedToken(request.RefreshToken),
            cancellationToken);

        if (existing is null || !existing.IsActive) return AuthResult.Fail(InvalidRefreshToken());

        var now = DateTimeOffset.UtcNow;
        var (opaqueRefresh, refreshEntity) = CreateRefreshTokenEntity(existing.UserId, existing.DeviceName, now);

        existing.RevokedAtUtc = now;
        existing.ReplacedByTokenId = refreshEntity.Id;
        authRepository.AddRefreshToken(refreshEntity);
        // Rotation is 1-for-1; still enforce cap in case of prior over-limit sessions.
        await refreshTokenMaintenance.EnforceActiveSessionLimitAsync(existing.UserId, cancellationToken);
        await authRepository.SaveChangesAsync(cancellationToken);

        return AuthResult.Ok(CreateSuccess(existing.User, opaqueRefresh, refreshEntity.ExpiresAtUtc));
    }

    public async Task<LogoutResult> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = ValidateRefreshToken(request.RefreshToken);
        if (validationError is not null) return LogoutResult.Fail(validationError);

        var existing = await authRepository.GetRefreshTokenByHashAsync(
            HashPresentedToken(request.RefreshToken!),
            cancellationToken);

        // Idempotent: missing or already revoked still counts as logged out.
        if (existing is null || existing.RevokedAtUtc is not null) return LogoutResult.Ok();

        existing.RevokedAtUtc = DateTimeOffset.UtcNow;
        await authRepository.SaveChangesAsync(cancellationToken);
        return LogoutResult.Ok();
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) return ChangePasswordResult.Fail(InvalidCredentials());

        var validationError = ValidateChangePassword(request);
        if (validationError is not null) return ChangePasswordResult.Fail(validationError);

        var user = await authRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user is null) return ChangePasswordResult.Fail(InvalidCredentials());

        if (!TryVerifyPassword(user, request.CurrentPassword, out _))
            return ChangePasswordResult.Fail(new AuthError(
                AuthErrorCode.InvalidCredentials,
                "Current password is incorrect."));

        if (request.CurrentPassword == request.NewPassword)
            return ChangePasswordResult.Fail(new AuthError(
                AuthErrorCode.Validation,
                "New password must be different from the current password."));

        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        // Force every device to sign in again with the new password.
        await refreshTokenMaintenance.RevokeAllForUserAsync(user.Id, cancellationToken);
        await authRepository.SaveChangesAsync(cancellationToken);

        return ChangePasswordResult.Ok();
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;

        var user = await authRepository.GetUserByIdReadOnlyAsync(userId, cancellationToken);
        return user is null ? null : ToDto(user);
    }

    private async Task<AuthSuccess> IssueSessionAsync(
        User user,
        string? deviceName,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var (opaqueRefresh, refreshEntity) = CreateRefreshTokenEntity(user.Id, deviceName, now);

        authRepository.AddRefreshToken(refreshEntity);
        // Keep at most MaxActiveRefreshTokensPerUser concurrent devices/sessions.
        await refreshTokenMaintenance.EnforceActiveSessionLimitAsync(user.Id, cancellationToken);
        await authRepository.SaveChangesAsync(cancellationToken);

        return CreateSuccess(user, opaqueRefresh, refreshEntity.ExpiresAtUtc);
    }

    private (string OpaqueToken, RefreshToken Entity) CreateRefreshTokenEntity(
        string userId,
        string? deviceName,
        DateTimeOffset now)
    {
        var opaque = RefreshTokenHasher.GenerateOpaqueToken();
        var entity = new RefreshToken
        {
            Id = Guid.NewGuid().ToString("N"),
            UserId = userId,
            TokenHash = RefreshTokenHasher.Hash(opaque),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            DeviceName = string.IsNullOrWhiteSpace(deviceName) ? null : deviceName.Trim()
        };

        return (opaque, entity);
    }

    private AuthSuccess CreateSuccess(User user, string opaqueRefresh, DateTimeOffset refreshExpiresAt)
    {
        var access = tokenService.CreateToken(user.Id, user.DisplayName);
        return new AuthSuccess(access, opaqueRefresh, refreshExpiresAt, ToDto(user));
    }

    private bool TryVerifyPassword(User user, string password, out bool rehashNeeded)
    {
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        rehashNeeded = result == PasswordVerificationResult.SuccessRehashNeeded;
        return result != PasswordVerificationResult.Failed;
    }

    private static UserDto ToDto(User user) => new(user.Id, user.Email, user.DisplayName);

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string HashPresentedToken(string refreshToken) =>
        RefreshTokenHasher.Hash(refreshToken.Trim());

    private static AuthError InvalidCredentials() =>
        new(AuthErrorCode.InvalidCredentials, "Invalid email or password.");

    private static AuthError InvalidRefreshToken() =>
        new(AuthErrorCode.InvalidRefreshToken, "Invalid or expired refresh token.");

    private static AuthError? ValidateRegister(RegisterRequest request)
    {
        var emailError = ValidateEmail(request.Email);
        if (emailError is not null) return emailError;

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < MinPasswordLength)
            return new AuthError(
                AuthErrorCode.Validation,
                $"Password must be at least {MinPasswordLength} characters.");

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            return new AuthError(AuthErrorCode.Validation, "Display name is required.");

        if (request.DisplayName.Trim().Length > 100)
            return new AuthError(AuthErrorCode.Validation, "Display name must be 100 characters or fewer.");

        return null;
    }

    private static AuthError? ValidateLogin(LoginRequest request)
    {
        var emailError = ValidateEmail(request.Email);
        if (emailError is not null) return emailError;

        if (string.IsNullOrWhiteSpace(request.Password))
            return new AuthError(AuthErrorCode.Validation, "Password is required.");

        return null;
    }

    private static AuthError? ValidateChangePassword(ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            return new AuthError(AuthErrorCode.Validation, "Current password is required.");

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < MinPasswordLength)
            return new AuthError(
                AuthErrorCode.Validation,
                $"New password must be at least {MinPasswordLength} characters.");

        return null;
    }

    private static AuthError? ValidateRefreshToken(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return new AuthError(AuthErrorCode.Validation, "Refresh token is required.");

        return null;
    }

    private static AuthError? ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            return new AuthError(AuthErrorCode.Validation, "A valid email is required.");

        return null;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var trimmed = email.Trim();
            _ = new MailAddress(trimmed);
            return new EmailAddressAttribute().IsValid(trimmed);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
