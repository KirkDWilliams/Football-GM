using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FootballGm.Api.Auth;

public interface ITokenService
{
    /// <summary>
    /// Creates a signed JWT for the given subject. Does not look up or create user accounts.
    /// </summary>
    TokenResponse CreateToken(string subject, string? displayName = null);
}

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    public TokenResponse CreateToken(string subject, string? displayName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(options.Value.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, subject)
        };

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, displayName));
            claims.Add(new Claim(ClaimTypes.Name, displayName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.Value.Issuer,
            options.Value.Audience,
            claims,
            DateTime.UtcNow,
            expiresAt.UtcDateTime,
            credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResponse(accessToken, "Bearer", expiresAt);
    }
}

public record TokenResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
