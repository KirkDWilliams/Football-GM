using FootballGm.Api.Data.Models;
using Rule = FootballGm.Api.Data.Entity.Contrived.Rule;
using ScoringWeightRule = FootballGm.Api.Data.Entity.Contrived.ScoringWeightRule;
using BonusRule = FootballGm.Api.Data.Entity.Contrived.BonusRule;

namespace FootballGm.Api.Services.GameAnalysis;

public interface IScoreCalculator
{
    List<StatScore> Calculate(StatLine stats, List<Rule> rules);
}

public class ScoreCalculator : IScoreCalculator
{
    public List<StatScore> Calculate(StatLine stats, List<Rule> rules)
    {
        var scores = new List<StatScore>();

        foreach (var rule in rules)
        {
            var statType = rule.Stat;

            switch (rule)
            {
                case ScoringWeightRule swr:
                    scores.Add(new StatScore { StatType = statType, Value = swr.Weight * stats[statType] });
                    continue;
                case BonusRule br when stats[statType] > br.Threshold:
                    scores.Add(new StatScore { StatType = statType, Value = br.Points });
                    continue;
            }
        }

        return scores;
    }
}
