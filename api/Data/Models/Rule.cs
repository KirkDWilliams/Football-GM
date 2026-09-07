using System.Text.Json.Serialization;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ScoringWeightRule), "scoringWeight")]
[JsonDerivedType(typeof(BonusRule), "bonus")]
public class Rule
{
    public RuleType RuleType { get; set; }
    public StatType Stat { get; set; }

    public static bool UsesDefaultScoringWeights(IEnumerable<Rule> rules)
    {
        var expected = CreateDefaultScoringWeights()
            .OfType<ScoringWeightRule>()
            .Select(rule => (rule.Stat, rule.Weight))
            .OrderBy(rule => rule.Stat);

        var actual = rules
            .OfType<ScoringWeightRule>()
            .Select(rule => (rule.Stat, rule.Weight))
            .OrderBy(rule => rule.Stat);

        return expected.SequenceEqual(actual);
    }

    public static List<Rule> CreateDefaultScoringWeights() =>
    [
        new ScoringWeightRule { Stat = StatType.PassAttempts, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.PassCompletions, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.PassingYards, Weight = 0.04m },
        new ScoringWeightRule { Stat = StatType.PassingTouchdowns, Weight = 4m },
        new ScoringWeightRule { Stat = StatType.RushingAttempts, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.RushingYards, Weight = 0.1m },
        new ScoringWeightRule { Stat = StatType.RushingFirstDowns, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.RushingTouchdowns, Weight = 6m },
        new ScoringWeightRule { Stat = StatType.Receptions, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.ReceivingYards, Weight = 0.1m },
        new ScoringWeightRule { Stat = StatType.ReceivingTouchdowns, Weight = 6m },
        new ScoringWeightRule { Stat = StatType.Interceptions, Weight = -2m },
        new ScoringWeightRule { Stat = StatType.Fumbles, Weight = -2m },
        new ScoringWeightRule { Stat = StatType.Sacks, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.FieldGoalsMade, Weight = 3m },
        new ScoringWeightRule { Stat = StatType.FieldGoalsMissed, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.ExtraPointsMade, Weight = 1m },
        new ScoringWeightRule { Stat = StatType.ExtraPointsAttempted, Weight = 0m },
        new ScoringWeightRule { Stat = StatType.PassingTwoPointConversions, Weight = 2m },
        new ScoringWeightRule { Stat = StatType.RushingTwoPointConversions, Weight = 2m },
        new ScoringWeightRule { Stat = StatType.ReceivingTwoPointConversions, Weight = 2m },
        new ScoringWeightRule { Stat = StatType.ReturnedTouchdowns, Weight = 6m }
    ];
}

public class ScoringWeightRule : Rule
{
    public ScoringWeightRule()
    {
        RuleType = RuleType.ScoringWeight;
    }

    public decimal Weight { get; set; }
}

public class BonusRule : Rule
{
    public BonusRule()
    {
        RuleType = RuleType.Bonus;
    }

    public decimal Threshold { get; set; }
    public decimal Points { get; set; }
}
