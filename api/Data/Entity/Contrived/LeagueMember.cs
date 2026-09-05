using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Entity.Contrived;

public class LeagueMember
{
    public int LeagueId { get; init; }

    [MaxLength(32)]
    public required string UserId { get; init; }

    public LeagueMemberRole Role { get; set; } = LeagueMemberRole.Member;

    public DateTimeOffset JoinedAtUtc { get; init; }

    public League League { get; init; } = null!;
    public User User { get; init; } = null!;

    public static LeagueMember Create(string userId, LeagueMemberRole role, int leagueId = 0) => new()
    {
        LeagueId = leagueId,
        UserId = userId,
        Role = role,
        JoinedAtUtc = DateTimeOffset.UtcNow
    };
}
