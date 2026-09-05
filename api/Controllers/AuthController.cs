using System.Security.Claims;
using FootballGm.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

/// <summary>
/// User registration, sign-in, token refresh, logout, and current-user lookup.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Create a new account and return access + refresh tokens.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterBody body,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(
            new RegisterRequest(body.Email, body.Password, body.DisplayName, body.DeviceName),
            cancellationToken);

        return ToActionResult(result, response => CreatedAtAction(nameof(Me), response));
    }

    /// <summary>
    /// Sign in with email and password; returns access + refresh tokens on success.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginBody body,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(
            new LoginRequest(body.Email, body.Password, body.DeviceName),
            cancellationToken);

        return ToActionResult(result, response => Ok(response));
    }

    /// <summary>
    /// Exchange a valid refresh token for a new access token and rotated refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh(
        [FromBody] RefreshBody body,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(
            new RefreshRequest(body.RefreshToken),
            cancellationToken);

        return ToActionResult(result, response => Ok(response));
    }

    /// <summary>
    /// Revoke a refresh token so it can no longer mint access tokens.
    /// Idempotent when the token is missing or already revoked.
    /// </summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutBody body,
        CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(
            new LogoutRequest(body.RefreshToken),
            cancellationToken);

        return NoContentOrError(result.Error);
    }

    /// <summary>
    /// Change the authenticated user's password and revoke all refresh sessions.
    /// Client must log in again afterward.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordBody body,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await authService.ChangePasswordAsync(
            userId,
            new ChangePasswordRequest(body.CurrentPassword, body.NewPassword),
            cancellationToken);

        return NoContentOrError(result.Error);
    }

    /// <summary>
    /// Returns the authenticated user from the database (requires Bearer JWT).
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> Me(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var user = await authService.GetUserByIdAsync(userId, cancellationToken);
        return user is null ? Unauthorized() : Ok(user);
    }

    private string? GetUserId()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtClaimNames.Sub);

        return string.IsNullOrWhiteSpace(userId) ? null : userId;
    }

    private ActionResult<AuthResponse> ToActionResult(
        AuthResult result,
        Func<AuthResponse, ActionResult<AuthResponse>> onSuccess)
    {
        return result.Error is not null
            ? MapError(result.Error)
            : onSuccess(ToResponse(result.Success!));
    }

    private ActionResult NoContentOrError(AuthError? error)
    {
        return error is not null ? MapError(error) : NoContent();
    }

    private ActionResult MapError(AuthError error)
    {
        return error.Code switch
        {
            AuthErrorCode.Validation => BadRequest(new { error = error.Message }),
            AuthErrorCode.Conflict => Conflict(new { error = error.Message }),
            AuthErrorCode.InvalidCredentials => Unauthorized(new { error = error.Message }),
            AuthErrorCode.InvalidRefreshToken => Unauthorized(new { error = error.Message }),
            _ => BadRequest(new { error = error.Message })
        };
    }

    private static AuthResponse ToResponse(AuthSuccess success)
    {
        return new AuthResponse(
            success.AccessToken.AccessToken,
            success.AccessToken.TokenType,
            success.AccessToken.ExpiresAt,
            success.RefreshToken,
            success.RefreshExpiresAt,
            success.User);
    }
}

public record RegisterBody(string Email, string Password, string DisplayName, string? DeviceName = null);

public record LoginBody(string Email, string Password, string? DeviceName = null);

public record RefreshBody(string RefreshToken);

public record LogoutBody(string RefreshToken);

public record ChangePasswordBody(string CurrentPassword, string NewPassword);

public record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt,
    UserDto User);

/// <summary>
/// Local constants so controllers do not depend on System.IdentityModel.Tokens.Jwt types.
/// </summary>
file static class JwtClaimNames
{
    public const string Sub = "sub";
}
