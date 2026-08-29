using FootballGm.Api.Data.Entity.Ingested;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Services.GameAnalysis;
using Rule = FootballGm.Api.Data.Entity.Contrived.Rule;
using ScoringWeightRule = FootballGm.Api.Data.Entity.Contrived.ScoringWeightRule;

namespace FootballGm.Api.Tests;

public class ScoreCalculatorTests
{
    private readonly ScoreCalculator _calculator = new();
    private readonly List<Rule> _passingYardRules =
    [
        new ScoringWeightRule { Stat = StatType.PassingYards, Weight = 0.04m },
        new ScoringWeightRule { Stat = StatType.PassingTouchdowns, Weight = 4m }
    ];

    [Fact]
    public void CalculateSeason_scores_season_totals()
    {
        var season = new PlayerSeason
        {
            PlayerId = "00-0033873",
            Season = 2026,
            PassingYards = 300,
            PassingTouchdowns = 2
        };

        var scores = _calculator.CalculateSeason(season, _passingYardRules);

        Assert.Equal(20m, Total(scores));
        Assert.Equal(12m, ScoreFor(scores, StatType.PassingYards));
        Assert.Equal(8m, ScoreFor(scores, StatType.PassingTouchdowns));
    }

    [Fact]
    public void CalculateRecentThreeGames_sums_the_three_most_recent_games()
    {
        var games = new List<PlayerGame>
        {
            Game(passingYards: 100, passingTouchdowns: 1),
            Game(passingYards: 150, passingTouchdowns: 0),
            Game(passingYards: 50, passingTouchdowns: 1)
        };

        var scores = _calculator.CalculateRecentThreeGames(games, _passingYardRules);

        Assert.Equal(20m, Total(scores));
    }

    [Fact]
    public void CalculateRecentThreeGames_uses_only_the_first_three_games()
    {
        var games = new List<PlayerGame>
        {
            Game(passingYards: 100),
            Game(passingYards: 100),
            Game(passingYards: 100),
            Game(passingYards: 400)
        };

        var scores = _calculator.CalculateRecentThreeGames(games, _passingYardRules);

        Assert.Equal(12m, Total(scores));
    }

    [Fact]
    public void CalculateRecentThreeGames_scores_two_games_when_that_is_all_that_exists()
    {
        var games = new List<PlayerGame>
        {
            Game(passingYards: 100, passingTouchdowns: 1),
            Game(passingYards: 50)
        };

        var scores = _calculator.CalculateRecentThreeGames(games, _passingYardRules);

        Assert.Equal(10m, Total(scores));
    }

    [Fact]
    public void CalculateRecentThreeGames_scores_one_game_at_the_start_of_the_season()
    {
        var games = new List<PlayerGame>
        {
            Game(passingYards: 250, passingTouchdowns: 2)
        };

        var scores = _calculator.CalculateRecentThreeGames(games, _passingYardRules);

        Assert.Equal(18m, Total(scores));
    }

    [Fact]
    public void CalculateRecentThreeGames_returns_no_scores_when_no_games_have_been_played()
    {
        var scores = _calculator.CalculateRecentThreeGames([], _passingYardRules);

        Assert.Empty(scores);
        Assert.Equal(0m, Total(scores));
    }

    [Fact]
    public void StatLine_from_games_adds_kick_makes_across_games()
    {
        var games = new List<PlayerGame>
        {
            new() { FieldGoalsMade = "32,41" },
            new() { FieldGoalsMade = "50" }
        };

        var line = StatLine.From(games);

        Assert.Equal(3m, line[StatType.FieldGoalsMade]);
    }

    private static PlayerGame Game(short passingYards = 0, short passingTouchdowns = 0) => new()
    {
        PassingYards = passingYards,
        PassingTouchdowns = passingTouchdowns
    };

    private static decimal Total(IEnumerable<StatScore> scores) => scores.Sum(s => s.Value);

    private static decimal ScoreFor(IEnumerable<StatScore> scores, StatType stat) =>
        scores.Single(s => s.StatType == stat).Value;
}
