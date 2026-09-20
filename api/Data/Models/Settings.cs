using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models
{
    public class Settings
    {
        public int SettingsId { get; init; }
        public int LeagueId { get; init; }
        public float WeeklyCapSpace { get; set; } = 100f;
        public required List<Position> EligiblePositions { get; init; }
        public required List<Rule> Rules { get; init; }
        public bool IsImmutable { get; private set; } = false;
    }
}
