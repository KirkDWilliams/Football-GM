using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Data.Models;
using Rule = FootballGm.Api.Data.Entity.Contrived.Rule;
using ScoringWeightRule = FootballGm.Api.Data.Entity.Contrived.ScoringWeightRule;
using BonusRule = FootballGm.Api.Data.Entity.Contrived.BonusRule;

namespace FootballGm.Api.Services.GameAnalysis;

public interface IScoreCalculator
{
    List<StatScore> Calculate(StatLine stats, List<Rule> rules);
    List<StatScore> CalculateSeason(PlayerSeason season, List<Rule> rules);
    List<StatScore> CalculateRecentThreeGames(IReadOnlyList<PlayerGame> games, List<Rule> rules);
}

public class ScoreCalculator : IScoreCalculator
{
    public const int RecentGameWindow = 3;

    public List<StatScore> Calculate(StatLine stats, List<Rule> rules)
    {
        var scores = new List<StatScore>();

        foreach (var rule in rules)
        {
            var statType = rule.Stat;

            switch (rule)
            {
                case ScoringWeightRule swr when stats[statType] > 0:
                    scores.Add(new StatScore { StatType = statType, Value = swr.Weight * stats[statType] });
                    continue;
                case BonusRule br when stats[statType] > br.Threshold:
                    scores.Add(new StatScore { StatType = statType, Value = br.Points });
                    continue;
            }
        }

        return scores;
    }

    public List<StatScore> CalculateSeason(PlayerSeason season, List<Rule> rules)
    {
        ArgumentNullException.ThrowIfNull(season);

        return Calculate(StatLine.From(season), rules);
    }

    public List<StatScore> CalculateRecentThreeGames(IReadOnlyList<PlayerGame> games, List<Rule> rules)
    {
        ArgumentNullException.ThrowIfNull(games);

        if (games.Count == 0)
            return [];

        var window = games.Count <= RecentGameWindow
            ? games
            : games.Take(RecentGameWindow).ToList();

        return Calculate(StatLine.From(window), rules);
    }
}
