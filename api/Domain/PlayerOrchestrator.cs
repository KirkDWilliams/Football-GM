using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Helpers;
using FootballGm.Api.Infrastructure.Interfaces;
using FootballGm.Api.Services.GameAnalysis;
using PlayerGame = FootballGm.Api.Data.Entity.Ingested.PlayerGame;
using PlayerSeason = FootballGm.Api.Data.Entity.Ingested.PlayerSeason;
using Player = FootballGm.Api.Data.Models.Player;
using Rule = FootballGm.Api.Data.Entity.Contrived.Rule;

namespace FootballGm.Api.Domain;

public class PlayerOrchestrator(
    IPlayerRepository playerRepository,
    ILeagueRepository leagueRepository,
    IScoreCalculator calculator) : IPlayerOrchestrator
{
    public async Task<Player> GetPlayer(
        string playerId,
        IReadOnlyCollection<StatSetKind> statSets,
        int? leagueId = null,
        string? gameId = null,
        CancellationToken cancellationToken = default)
    {
        var requested = statSets.Distinct().ToList();
        if (requested.Count == 0)
            return await GetPlayerAlone(playerId);

        if (leagueId is null or <= 0)
            throw new ArgumentException("LeagueId is required when requesting stats.", nameof(leagueId));

        if (requested.Contains(StatSetKind.PreviousWeek) && string.IsNullOrWhiteSpace(gameId))
            throw new ArgumentException("GameId is required when requesting previous week stats.", nameof(gameId));

        var playerTask = playerRepository.GetPlayerByIdAsync(playerId);
        var leagueTask = leagueRepository.GetByIdAsync(leagueId.Value, cancellationToken);
        var gameTask = requested.Contains(StatSetKind.PreviousWeek)
            ? playerRepository.GetPlayerGameStatsAsync(playerId, gameId!)
            : Task.FromResult<PlayerGame?>(null);
        var seasonTask = requested.Contains(StatSetKind.Season)
            ? playerRepository.GetPlayerSeasonStatsAsync(playerId, WeekHelper.CurrentSeason)
            : Task.FromResult<PlayerSeason?>(null);
        var recentGamesTask = requested.Contains(StatSetKind.RecentThreeGames)
            ? playerRepository.GetRecentPlayerGamesAsync(playerId, ScoreCalculator.RecentGameWindow)
            : Task.FromResult<List<PlayerGame>>([]);

        await Task.WhenAll(playerTask, leagueTask, gameTask, seasonTask, recentGamesTask);

        var playerEntity = await playerTask;
        var league = await leagueTask;
        var game = await gameTask;
        var season = await seasonTask;
        var recentGames = await recentGamesTask;

        ArgumentNullException.ThrowIfNull(playerEntity);
        ArgumentNullException.ThrowIfNull(league);

        var rules = league.Settings.Rules;
        var stats = requested.Select(kind => kind switch
        {
            StatSetKind.PreviousWeek => ScorePreviousWeek(game, rules),
            StatSetKind.Season => ScoreSeason(season, rules),
            StatSetKind.RecentThreeGames => ScoreRecentThreeGames(recentGames, rules),
            _ => throw new ArgumentOutOfRangeException(nameof(statSets), kind, "Unknown stat set.")
        }).ToList();

        return Player.FromEntity(playerEntity) with { Stats = stats };
    }

    private async Task<Player> GetPlayerAlone(string playerId)
    {
        var player = await playerRepository.GetPlayerByIdAsync(playerId);
        ArgumentNullException.ThrowIfNull(player);
        return Player.FromEntity(player);
    }

    private StatSet ScorePreviousWeek(PlayerGame? game, List<Rule> rules)
    {
        ArgumentNullException.ThrowIfNull(game);

        return StatSet.From(StatSetKind.PreviousWeek, calculator.Calculate(StatLine.From(game), rules));
    }

    private StatSet ScoreSeason(PlayerSeason? season, List<Rule> rules) =>
        StatSet.From(
            StatSetKind.Season,
            season is null ? [] : calculator.CalculateSeason(season, rules));

    private StatSet ScoreRecentThreeGames(IReadOnlyList<PlayerGame> games, List<Rule> rules) =>
        StatSet.From(StatSetKind.RecentThreeGames, calculator.CalculateRecentThreeGames(games, rules));
}
