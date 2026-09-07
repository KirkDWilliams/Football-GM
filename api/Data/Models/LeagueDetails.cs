using System.Text.Json.Serialization;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Serialization;

namespace FootballGm.Api.Data.Models;

public sealed class LeagueDetails
{
    public required int LeagueId { get; init; }
    public required string Name { get; init; }
    public required string JoinCode { get; init; }
    public required decimal WeeklyCapSpace { get; init; }
    public required List<Rule> Rules { get; init; }
    public required List<Position> Positions { get; init; }

    [JsonConverter(typeof(CamelCaseEnumConverter<LeagueMemberRole>))]
    public required LeagueMemberRole Role { get; init; }

    public static LeagueDetails From(League league, LeagueMemberRole role) => new()
    {
        LeagueId = league.LeagueId,
        Name = league.Name,
        JoinCode = league.JoinCode,
        WeeklyCapSpace = league.WeeklyCapSpace,
        Rules = league.Rules,
        Positions = league.Positions,
        Role = role
    };
}
