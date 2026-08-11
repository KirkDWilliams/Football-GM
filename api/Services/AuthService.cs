using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using FootballGm.Api.Auth;
using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Services;

public class AuthService(AppDbContext db, IPasswordHasher<User> hasher, ITokenService tokenService) : IAuthService
{
    private const int MinPasswordLength = 8;

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

        return AuthResult.Ok(BuildSuccess(user));
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

        return AuthResult.Ok(BuildSuccess(user));
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

    private AuthSuccess BuildSuccess(User user)
    {
        var token = tokenService.CreateToken(user.Id, user.DisplayName);
        return new AuthSuccess(token, ToDto(user));
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Email, user.DisplayName);

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static AuthError InvalidCredentials() =>
        new(AuthErrorCode.InvalidCredentials, "Invalid email or password.");

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
