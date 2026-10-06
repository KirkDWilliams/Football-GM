using System.Text.Json.Serialization;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Utility;
using Entities = FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Data.Models;

public sealed class DraftSnapshot
{
    public required int DraftId { get; init; }
    public required int LeagueId { get; init; }

    [JsonConverter(typeof(CamelCaseEnumConverter<DraftStatus>))]
    public required DraftStatus Status { get; init; }

    public string? CurrentNominatorUserId { get; init; }

    public required List<string> NominationOrder { get; init; }

    public required List<DraftMemberSnapshot> Members { get; init; }

    public static DraftSnapshot From(
        Entities.Draft draft,
        IEnumerable<Entities.LeagueMember> members) => new()
    {
        DraftId = draft.Id,
        LeagueId = draft.LeagueId,
        Status = draft.Status,
        CurrentNominatorUserId = draft.CurrentNominatorUserId,
        NominationOrder = [.. draft.NominationOrder],
        Members =
        [
            .. OrderedByJoin(members).Select(member => new DraftMemberSnapshot
            {
                UserId = member.UserId,
                DisplayName = member.User.DisplayName
            })
        ]
    };

    /// <summary>
    /// Earliest join first. User id breaks ties. Start freezes this order.
    /// </summary>
    public static IEnumerable<Entities.LeagueMember> OrderedByJoin(
        IEnumerable<Entities.LeagueMember> members) =>
        members.OrderBy(member => member.JoinedAtUtc).ThenBy(member => member.UserId);
}

public sealed class DraftMemberSnapshot
{
    public required string UserId { get; init; }
    public required string DisplayName { get; init; }
}
