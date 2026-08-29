using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Entity.Contrived;

public abstract class Rule
{
    [Key]
    public long RuleId { get; init; }
    public int SettingsId { get; init; }
    public RuleType RuleType { get; init; }
    public StatType Stat { get; init; }
}

public class ScoringWeightRule : Rule
{
    public ScoringWeightRule()
    {
        RuleType = RuleType.ScoringWeight;
    }

    public decimal Weight { get; init; }
}

public class BonusRule : Rule
{
    public BonusRule()
    {
        RuleType = RuleType.Bonus;
    }

    public decimal Threshold { get; init; }
    public decimal Points { get; init; }
}
