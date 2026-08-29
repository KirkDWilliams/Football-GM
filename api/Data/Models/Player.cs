using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public record Player
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public NflTeam Team { get; init; }
    public Position Position { get; init; }
    public short JerseyNumber { get; set; }
    public short DraftYear { get; set; }

    public List<StatScore>? PreviousWeekScores { get; init; }
    public decimal? PreviousWeekScore { get; init; }

    public static Player FromEntity(Entity.Ingested.Player player) => new()
    {
        Id = player.PlayerId,
        Name = player.Name,
        Team = NflTeam.ArizonaCardinals, // TODO: Add NFL Team Mapping
        Position = Position.Kicker, // TODO: Add Position Mapping
        JerseyNumber = player.JerseyNumber,
        DraftYear = player.DraftYear
    };
}
