using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Settings
{
    [Key]
    public int SettingsId { get; init; }
    public int LeagueId { get; init; }
    public decimal WeeklyCapSpace { get; init; } = 100M;
    public required List<Position> EligiblePositions { get; init; }
    public required List<Rule> Rules { get; init; }
}
