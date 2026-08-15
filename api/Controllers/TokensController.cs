using System.Security.Claims;
using FootballGm.Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

/// <summary>
/// Token minting for development and a protected probe endpoint.
/// Does not implement login, registration, or user accounts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TokensController : ControllerBase
{
    private readonly IHostEnvironment _environment;
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService, IHostEnvironment environment)
    {
        _tokenService = tokenService;
        _environment = environment;
    }

    /// <summary>
    /// Development-only: issue a JWT for a given subject without authenticating a user store.
    /// Returns 404 outside Development so free minting is not exposed.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TokenResponse> Create([FromBody] CreateTokenRequest request)
    {
        if (!_environment.IsDevelopment()) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Subject)) return BadRequest(new { error = "subject is required" });

        var token = _tokenService.CreateToken(request.Subject.Trim(), request.DisplayName);
        return Ok(token);
    }

    /// <summary>
    /// Returns claims from the current bearer token. Useful to verify authorization is working.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(TokenIdentityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<TokenIdentityResponse> Me()
    {
        var subject =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtClaimNames.Sub);

        var displayName =
            User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue(JwtClaimNames.Name);

        return Ok(new TokenIdentityResponse(subject, displayName));
    }
}

public record CreateTokenRequest(string Subject, string? DisplayName = null);

public record TokenIdentityResponse(string? Subject, string? DisplayName);

/// <summary>Local constants so controllers do not depend on System.IdentityModel.Tokens.Jwt types.</summary>
file static class JwtClaimNames
{
    public const string Sub = "sub";
    public const string Name = "name";
}
