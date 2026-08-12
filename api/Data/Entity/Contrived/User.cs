using FootballGm.Api.Data.Entity.Associations;
using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived;

/// <summary>
/// Application user account. Password is stored only as a one-way hash.
/// </summary>
public class User
{
    [Key]
    public string Id { get; set; } = string.Empty;

    /// <summary>Normalized (trimmed + lowercased) email used for login.</summary>
    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    /// <summary>One-way password hash. Never return this in API responses.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public List<LeagueTeams> LeagueTeams { get; set; } = [];
}
