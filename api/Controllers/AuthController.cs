using System.Security.Claims;
using FootballGm.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

/// <summary>
/// User registration, sign-in, and current-user lookup.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Create a new account and return a JWT.
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
            new RegisterRequest(body.Email, body.Password, body.DisplayName),
            cancellationToken);

        if (result.Error is not null)
        {
            return MapError(result.Error);
        }

        return CreatedAtAction(nameof(Me), ToResponse(result.Success!));
    }

    /// <summary>
    /// Sign in with email and password; returns a JWT on success.
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
            new LoginRequest(body.Email, body.Password),
            cancellationToken);

        if (result.Error is not null)
        {
            return MapError(result.Error);
        }

        return Ok(ToResponse(result.Success!));
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
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var user = await authService.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    private ActionResult MapError(AuthError error) =>
        error.Code switch
        {
            AuthErrorCode.Validation => BadRequest(new { error = error.Message }),
            AuthErrorCode.Conflict => Conflict(new { error = error.Message }),
            AuthErrorCode.InvalidCredentials => Unauthorized(new { error = error.Message }),
            _ => BadRequest(new { error = error.Message }),
        };

    private static AuthResponse ToResponse(AuthSuccess success) =>
        new(
            success.Token.AccessToken,
            success.Token.TokenType,
            success.Token.ExpiresAt,
            success.User);
}

public record RegisterBody(string Email, string Password, string DisplayName);

public record LoginBody(string Email, string Password);

public record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    UserDto User);

/// <summary>
/// Local constants so controllers do not depend on System.IdentityModel.Tokens.Jwt types.
/// </summary>
file static class JwtClaimNames
{
    public const string Sub = "sub";
}
