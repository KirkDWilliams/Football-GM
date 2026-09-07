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
        new ScoringWeightRule { Stat = StatType.PassAttempts, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.PassCompletions, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.PassingYards, Weight = 0.04f },
        new ScoringWeightRule { Stat = StatType.PassingTouchdowns, Weight = 4f },
        new ScoringWeightRule { Stat = StatType.RushingAttempts, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.RushingYards, Weight = 0.1f },
        new ScoringWeightRule { Stat = StatType.RushingFirstDowns, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.RushingTouchdowns, Weight = 6f },
        new ScoringWeightRule { Stat = StatType.Receptions, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.ReceivingYards, Weight = 0.1f },
        new ScoringWeightRule { Stat = StatType.ReceivingTouchdowns, Weight = 6f },
        new ScoringWeightRule { Stat = StatType.Interceptions, Weight = -2f },
        new ScoringWeightRule { Stat = StatType.Fumbles, Weight = -2f },
        new ScoringWeightRule { Stat = StatType.Sacks, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.FieldGoalsMade, Weight = 3f },
        new ScoringWeightRule { Stat = StatType.FieldGoalsMissed, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.ExtraPointsMade, Weight = 1f },
        new ScoringWeightRule { Stat = StatType.ExtraPointsAttempted, Weight = 0f },
        new ScoringWeightRule { Stat = StatType.PassingTwoPointConversions, Weight = 2f },
        new ScoringWeightRule { Stat = StatType.RushingTwoPointConversions, Weight = 2f },
        new ScoringWeightRule { Stat = StatType.ReceivingTwoPointConversions, Weight = 2f },
        new ScoringWeightRule { Stat = StatType.ReturnedTouchdowns, Weight = 6f }
    ];
}

public class ScoringWeightRule : Rule
{
    public ScoringWeightRule()
    {
        RuleType = RuleType.ScoringWeight;
    }

    public float Weight { get; set; }
}

public class BonusRule : Rule
{
    public BonusRule()
    {
        RuleType = RuleType.Bonus;
    }

    public float Threshold { get; set; }
    public float Points { get; set; }
}
