using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Ingested;

public class Game
{
    [Key] public string GameId { get; init; } = string.Empty;

    public short Season { get; init; }
    public short Week { get; init; }
    public DateOnly Date { get; init; }
    public string HomeTeam { get; init; } = string.Empty;
    public string AwayTeam { get; init; } = string.Empty;
    public string HomeScore { get; init; } = "NA";
    public string AwayScore { get; init; } = "NA";
    public short? WindSpeed { get; init; } = 0;
    public short? Temperature { get; init; }
}
