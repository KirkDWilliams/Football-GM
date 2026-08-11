using System.Security.Cryptography;
using System.Text;

namespace FootballGm.Api.Auth;

/// <summary>
/// Creates opaque refresh tokens and one-way hashes for server-side storage.
/// </summary>
public static class RefreshTokenHasher
{
    private const int TokenByteLength = 32;

    /// <summary>Generates a URL-safe opaque token for the client.</summary>
    public static string GenerateOpaqueToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenByteLength);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>SHA-256 hex hash used as the stored lookup key.</summary>
    public static string Hash(string opaqueToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(opaqueToken);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(opaqueToken));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
