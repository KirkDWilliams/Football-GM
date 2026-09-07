using System.Text.Json.Serialization;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Serialization;
using Entities = FootballGm.Api.Data.Entity.Contrived;

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

    public static LeagueSummary From(League league, LeagueMemberRole role) => new()
    {
        LeagueId = league.LeagueId,
        Name = league.Name,
        JoinCode = league.JoinCode,
        Role = role,
        Scoring = Rule.UsesDefaultScoringWeights(league.Rules)
            ? ScoringKind.Standard
            : ScoringKind.Custom
    };

    public static LeagueSummary From(Entities.League entity, LeagueMemberRole role) =>
        From(League.FromEntity(entity), role);
}
