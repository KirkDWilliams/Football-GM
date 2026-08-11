using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using FootballGm.Api.Auth;
using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballGm.Api.Services;

public class AuthService(
    AppDbContext db,
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
        if (validationError is not null)
        {
            return AuthResult.Fail(validationError);
        }

        var email = NormalizeEmail(request.Email);
        var displayName = request.DisplayName.Trim();

        var emailTaken = await db.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailTaken)
        {
            return AuthResult.Fail(new AuthError(AuthErrorCode.Conflict, "An account with this email already exists."));
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString("N"),
            Email = email,
            DisplayName = displayName,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };

        user.PasswordHash = hasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return AuthResult.Ok(await IssueSessionAsync(user, request.DeviceName, cancellationToken));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = ValidateLogin(request);
        if (validationError is not null)
        {
            return AuthResult.Fail(validationError);
        }

        var email = NormalizeEmail(request.Email);
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Same generic failure whether the email is missing or the password is wrong.
        if (user is null)
        {
            return AuthResult.Fail(InvalidCredentials());
        }

        var verifyResult = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return AuthResult.Fail(InvalidCredentials());
        }

        // Rehash if the hasher algorithm was upgraded since the password was stored.
        if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, request.Password);
            await db.SaveChangesAsync(cancellationToken);
        }

        return AuthResult.Ok(await IssueSessionAsync(user, request.DeviceName, cancellationToken));
    }

    public async Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return AuthResult.Fail(new AuthError(
                AuthErrorCode.Validation,
                "Refresh token is required."));
        }

        var tokenHash = RefreshTokenHasher.Hash(request.RefreshToken.Trim());
        var existing = await db.RefreshTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            return AuthResult.Fail(InvalidRefreshToken());
        }

        var now = DateTimeOffset.UtcNow;
        var (opaqueRefresh, refreshEntity) = CreateRefreshTokenEntity(existing.UserId, existing.DeviceName, now);

        existing.RevokedAtUtc = now;
        existing.ReplacedByTokenId = refreshEntity.Id;

        db.RefreshTokens.Add(refreshEntity);
        // Rotation is 1-for-1; still enforce cap in case of prior over-limit sessions.
        await refreshTokenMaintenance.EnforceActiveSessionLimitAsync(existing.UserId, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var access = tokenService.CreateToken(existing.User.Id, existing.User.DisplayName);
        return AuthResult.Ok(new AuthSuccess(
            access,
            opaqueRefresh,
            refreshEntity.ExpiresAtUtc,
            ToDto(existing.User)));
    }

    public async Task<LogoutResult> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return LogoutResult.Fail(new AuthError(
                AuthErrorCode.Validation,
                "Refresh token is required."));
        }

        var tokenHash = RefreshTokenHasher.Hash(request.RefreshToken.Trim());
        var existing = await db.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        // Idempotent: missing or already revoked still counts as logged out.
        if (existing is null || existing.RevokedAtUtc is not null)
        {
            return LogoutResult.Ok();
        }

        existing.RevokedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return LogoutResult.Ok();
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user is null ? null : ToDto(user);
    }

    private async Task<AuthSuccess> IssueSessionAsync(
        User user,
        string? deviceName,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var (opaqueRefresh, refreshEntity) = CreateRefreshTokenEntity(user.Id, deviceName, now);

        db.RefreshTokens.Add(refreshEntity);
        // Keep at most MaxActiveRefreshTokensPerUser concurrent devices/sessions.
        await refreshTokenMaintenance.EnforceActiveSessionLimitAsync(user.Id, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var access = tokenService.CreateToken(user.Id, user.DisplayName);
        return new AuthSuccess(access, opaqueRefresh, refreshEntity.ExpiresAtUtc, ToDto(user));
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
            DeviceName = string.IsNullOrWhiteSpace(deviceName) ? null : deviceName.Trim(),
        };

        return (opaque, entity);
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Email, user.DisplayName);

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static AuthError InvalidCredentials() =>
        new(AuthErrorCode.InvalidCredentials, "Invalid email or password.");

    private static AuthError InvalidRefreshToken() =>
        new(AuthErrorCode.InvalidRefreshToken, "Invalid or expired refresh token.");

    private static AuthError? ValidateRegister(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
        {
            return new AuthError(AuthErrorCode.Validation, "A valid email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < MinPasswordLength)
        {
            return new AuthError(
                AuthErrorCode.Validation,
                $"Password must be at least {MinPasswordLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return new AuthError(AuthErrorCode.Validation, "Display name is required.");
        }

        if (request.DisplayName.Trim().Length > 100)
        {
            return new AuthError(AuthErrorCode.Validation, "Display name must be 100 characters or fewer.");
        }

        return null;
    }

    private static AuthError? ValidateLogin(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
        {
            return new AuthError(AuthErrorCode.Validation, "A valid email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return new AuthError(AuthErrorCode.Validation, "Password is required.");
        }

        return null;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email.Trim());
            return new EmailAddressAttribute().IsValid(email.Trim());
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
