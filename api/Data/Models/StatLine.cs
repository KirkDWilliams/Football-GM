using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public sealed class StatLine
{
    private IReadOnlyDictionary<StatType, float> Values { get; }

    public float this[StatType stat] =>
        Values.GetValueOrDefault(stat);

    private StatLine(IReadOnlyDictionary<StatType, float> values)
    {
        Values = values;
    }

    public static StatLine From(PlayerGame game) => new(new Dictionary<StatType, float>
    {
        [StatType.PassAttempts]                 = game.PassAttempts,
        [StatType.PassCompletions]              = game.PassCompletions,
        [StatType.PassingYards]                 = game.PassingYards,
        [StatType.PassingTouchdowns]            = game.PassingTouchdowns,
        [StatType.RushingAttempts]              = game.RushAttempts,
        [StatType.RushingYards]                 = game.RushingYards,
        [StatType.RushingFirstDowns]            = game.RushingFirstDowns,
        [StatType.RushingTouchdowns]            = game.RushingTouchdowns,
        [StatType.Receptions]                   = game.Receptions,
        [StatType.ReceivingYards]               = game.ReceivingYards,
        [StatType.ReceivingTouchdowns]          = game.ReceivingTouchdowns,
        [StatType.Interceptions]                = game.Interceptions,
        [StatType.Fumbles]                      = game.Fumbles,
        [StatType.Sacks]                        = game.Sacks,
        [StatType.FieldGoalsMade]               = CountKickList(game.FieldGoalsMade),
        [StatType.FieldGoalsMissed]             = CountKickList(game.FieldGoalsMissed),
        [StatType.ExtraPointsMade]              = game.ExtraPointsMade,
        [StatType.ExtraPointsAttempted]         = game.ExtraPointsAttempted,
        [StatType.PassingTwoPointConversions]   = game.PassingTwoPointConversions,
        [StatType.RushingTwoPointConversions]   = game.RushingTwoPointConversions,
        [StatType.ReceivingTwoPointConversions] = game.ReceivingTwoPointConversions,
        [StatType.ReturnedTouchdowns]           = game.ReturnedTouchdowns
    });

    public static StatLine From(PlayerSeason season) => new(new Dictionary<StatType, float>
    {
        [StatType.PassAttempts]                 = season.PassAttempts,
        [StatType.PassCompletions]              = season.PassCompletions,
        [StatType.PassingYards]                 = season.PassingYards,
        [StatType.PassingTouchdowns]            = season.PassingTouchdowns,
        [StatType.RushingAttempts]              = season.RushAttempts,
        [StatType.RushingYards]                 = season.RushingYards,
        [StatType.RushingFirstDowns]            = season.RushingFirstDowns,
        [StatType.RushingTouchdowns]            = season.RushingTouchdowns,
        [StatType.Receptions]                   = season.Receptions,
        [StatType.ReceivingYards]               = season.ReceivingYards,
        [StatType.ReceivingTouchdowns]          = season.ReceivingTouchdowns,
        [StatType.Interceptions]                = season.Interceptions,
        [StatType.Fumbles]                      = season.Fumbles,
        [StatType.Sacks]                        = season.Sacks,
        [StatType.FieldGoalsMade]               = CountKickList(season.FieldGoalsMade),
        [StatType.FieldGoalsMissed]             = CountKickList(season.FieldGoalsMissed),
        [StatType.ExtraPointsMade]              = season.ExtraPointsMade,
        [StatType.ExtraPointsAttempted]         = season.ExtraPointsAttempted,
        [StatType.PassingTwoPointConversions]   = season.PassingTwoPointConversions,
        [StatType.RushingTwoPointConversions]   = season.RushingTwoPointConversions,
        [StatType.ReceivingTwoPointConversions] = season.ReceivingTwoPointConversions,
        [StatType.ReturnedTouchdowns]           = season.ReturnedTouchdowns
    });

    public static StatLine From(IEnumerable<PlayerGame> games)
    {
        ArgumentNullException.ThrowIfNull(games);

        var list = games as IReadOnlyList<PlayerGame> ?? games.ToList();
        if (list.Count == 0)
            return From(new PlayerGame());

        if (list.Count == 1)
            return From(list[0]);

        return new StatLine(new Dictionary<StatType, float>
        {
            [StatType.PassAttempts]                 = list.Sum(g => (float) g.PassAttempts),
            [StatType.PassCompletions]              = list.Sum(g => (float) g.PassCompletions),
            [StatType.PassingYards]                 = list.Sum(g => (float) g.PassingYards),
            [StatType.PassingTouchdowns]            = list.Sum(g => (float) g.PassingTouchdowns),
            [StatType.RushingAttempts]              = list.Sum(g => (float) g.RushAttempts),
            [StatType.RushingYards]                 = list.Sum(g => (float) g.RushingYards),
            [StatType.RushingFirstDowns]            = list.Sum(g => (float) g.RushingFirstDowns),
            [StatType.RushingTouchdowns]            = list.Sum(g => (float) g.RushingTouchdowns),
            [StatType.Receptions]                   = list.Sum(g => (float) g.Receptions),
            [StatType.ReceivingYards]               = list.Sum(g => (float) g.ReceivingYards),
            [StatType.ReceivingTouchdowns]          = list.Sum(g => (float) g.ReceivingTouchdowns),
            [StatType.Interceptions]                = list.Sum(g => (float) g.Interceptions),
            [StatType.Fumbles]                      = list.Sum(g => (float) g.Fumbles),
            [StatType.Sacks]                        = list.Sum(g => (float) g.Sacks),
            [StatType.FieldGoalsMade]               = list.Sum(g => CountKickList(g.FieldGoalsMade)),
            [StatType.FieldGoalsMissed]             = list.Sum(g => CountKickList(g.FieldGoalsMissed)),
            [StatType.ExtraPointsMade]              = list.Sum(g => (float) g.ExtraPointsMade),
            [StatType.ExtraPointsAttempted]         = list.Sum(g => (float) g.ExtraPointsAttempted),
            [StatType.PassingTwoPointConversions]   = list.Sum(g => (float) g.PassingTwoPointConversions),
            [StatType.RushingTwoPointConversions]   = list.Sum(g => (float) g.RushingTwoPointConversions),
            [StatType.ReceivingTwoPointConversions] = list.Sum(g => (float) g.ReceivingTwoPointConversions),
            [StatType.ReturnedTouchdowns]           = list.Sum(g => (float) g.ReturnedTouchdowns)
        });
    }

    private static float CountKickList(string list)
    {
        if (string.IsNullOrWhiteSpace(list))
            return 0f;

        return list
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Length;
    }
}
