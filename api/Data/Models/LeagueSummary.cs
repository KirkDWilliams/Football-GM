using System.Text.Json.Serialization;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Serialization;

namespace FootballGm.Api.Data.Models;

public sealed class LeagueSummary
{
    public required int LeagueId { get; init; }
    public required string Name { get; init; }
    public required string JoinCode { get; init; }

    [JsonConverter(typeof(CamelCaseEnumConverter<LeagueMemberRole>))]
    public required LeagueMemberRole Role { get; init; }

    [JsonConverter(typeof(CamelCaseEnumConverter<ScoringKind>))]
    public required ScoringKind Scoring { get; init; }
}
