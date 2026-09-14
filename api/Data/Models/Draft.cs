using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public class Draft
{
    public int LeagueId { get; set; }

    public DraftStatus Status { get; set; }

    public static Entity.Contrived.Draft ToEntity(int leagueId, DraftStatus status) => new()
    {
        LeagueId = leagueId,
        Status = status
    };
}
