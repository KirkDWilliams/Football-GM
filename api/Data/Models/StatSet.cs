using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public record StatSet
{
    public required StatSetKind Kind { get; init; }
    public required List<StatScore> Scores { get; init; }
    public decimal Total => Scores.Sum(s => s.Value);

    public static StatSet From(StatSetKind kind, List<StatScore> scores) => new()
    {
        Kind = kind,
        Scores = scores
    };
}
