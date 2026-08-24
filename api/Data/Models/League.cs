using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public class League
{
    public int LeagueId { get; init; }
    public string Name { get; init; }
    public decimal WeeklyCapSpace { get; init; } = 100M;
    public List<Rule> Rules { get; init; }
    public List<Position> Positions { get; init; } =
    [
        Position.Quarterback,
        Position.RunningBack,
        Position.RunningBack,
        Position.WideReceiver,
        Position.WideReceiver,
        Position.TightEnd,
        Position.Kicker
    ];

    public League(string name, List<Position>? positions = null, List<Rule>? rules = null)
    {
        Name = name;
        Rules = rules is { Count: > 0 } ? rules : Rule.CreateDefaultScoringWeights();
        if (positions is { Count: > 0 })
            Positions = positions;
    }
}
