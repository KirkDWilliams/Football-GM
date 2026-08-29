using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public sealed class StatLine
{
    private IReadOnlyDictionary<StatType, decimal> Values { get; }

    public decimal this[StatType stat] =>
        Values.GetValueOrDefault(stat);

    private StatLine(IReadOnlyDictionary<StatType, decimal> values)
    {
        Values = values;
    }

    public static StatLine From(PlayerGame game) => new(new Dictionary<StatType, decimal>
    {
        [StatType.PassAttempts] = game.PassAttempts,
        [StatType.PassCompletions] = game.PassCompletions,
        [StatType.PassingYards] = game.PassingYards,
        [StatType.PassingTouchdowns] = game.PassingTouchdowns,
        [StatType.RushingAttempts] = game.RushAttempts,
        [StatType.RushingYards] = game.RushingYards,
        [StatType.RushingFirstDowns] = game.RushingFirstDowns,
        [StatType.RushingTouchdowns] = game.RushingTouchdowns,
        [StatType.Receptions] = game.Receptions,
        [StatType.ReceivingYards] = game.ReceivingYards,
        [StatType.ReceivingTouchdowns] = game.ReceivingTouchdowns,
        [StatType.Interceptions] = game.Interceptions,
        [StatType.Fumbles] = game.Fumbles,
        [StatType.Sacks] = game.Sacks,
        [StatType.FieldGoalsMade] = CountKickList(game.FieldGoalsMade),
        [StatType.FieldGoalsMissed] = CountKickList(game.FieldGoalsMissed),
        [StatType.ExtraPointsMade] = game.ExtraPointsMade,
        [StatType.ExtraPointsAttempted] = game.ExtraPointsAttempted,
        [StatType.PassingTwoPointConversions] = game.PassingTwoPointConversions,
        [StatType.RushingTwoPointConversions] = game.RushingTwoPointConversions,
        [StatType.ReceivingTwoPointConversions] = game.ReceivingTwoPointConversions,
        [StatType.ReturnedTouchdowns] = game.ReturnedTouchdowns
    });

    public static StatLine From(PlayerSeason season) => new(new Dictionary<StatType, decimal>
    {
        [StatType.PassAttempts] = season.PassAttempts,
        [StatType.PassCompletions] = season.PassCompletions,
        [StatType.PassingYards] = season.PassingYards,
        [StatType.PassingTouchdowns] = season.PassingTouchdowns,
        [StatType.RushingAttempts] = season.RushAttempts,
        [StatType.RushingYards] = season.RushingYards,
        [StatType.RushingFirstDowns] = season.RushingFirstDowns,
        [StatType.RushingTouchdowns] = season.RushingTouchdowns,
        [StatType.Receptions] = season.Receptions,
        [StatType.ReceivingYards] = season.ReceivingYards,
        [StatType.ReceivingTouchdowns] = season.ReceivingTouchdowns,
        [StatType.Interceptions] = season.Interceptions,
        [StatType.Fumbles] = season.Fumbles,
        [StatType.Sacks] = season.Sacks,
        [StatType.FieldGoalsMade] = CountKickList(season.FieldGoalsMade),
        [StatType.FieldGoalsMissed] = CountKickList(season.FieldGoalsMissed),
        [StatType.ExtraPointsMade] = season.ExtraPointsMade,
        [StatType.ExtraPointsAttempted] = season.ExtraPointsAttempted,
        [StatType.PassingTwoPointConversions] = season.PassingTwoPointConversions,
        [StatType.RushingTwoPointConversions] = season.RushingTwoPointConversions,
        [StatType.ReceivingTwoPointConversions] = season.ReceivingTwoPointConversions,
        [StatType.ReturnedTouchdowns] = season.ReturnedTouchdowns
    });

    private static decimal CountKickList(string list)
    {
        if (string.IsNullOrWhiteSpace(list))
            return 0;

        return list
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Length;
    }
}
