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
            return await GetPlayerAlone(playerId, cancellationToken);

        if (leagueId is null or <= 0)
            throw new ArgumentException("LeagueId is required when requesting stats.", nameof(leagueId));

        if (requested.Contains(StatSetKind.PreviousWeek) && string.IsNullOrWhiteSpace(gameId))
            throw new ArgumentException("GameId is required when requesting previous week stats.", nameof(gameId));

        var playerEntity = await playerRepository.GetPlayerByIdAsync(playerId, cancellationToken);
        ArgumentNullException.ThrowIfNull(playerEntity);

        var league = await leagueRepository.GetByIdAsync(leagueId.Value, cancellationToken);
        ArgumentNullException.ThrowIfNull(league);

        PlayerGame? game = null;
        PlayerSeason? season = null;
        List<PlayerGame> recentGames = [];
        if (requested.Contains(StatSetKind.PreviousWeek))
            game = await playerRepository.GetPlayerGameStatsAsync(playerId, gameId!, cancellationToken);

        if (requested.Contains(StatSetKind.Season))
            season = await playerRepository.GetPlayerSeasonStatsAsync(
                playerId,
                WeekHelper.CurrentSeason,
                cancellationToken);

        if (requested.Contains(StatSetKind.RecentThreeGames))
            recentGames = await playerRepository.GetRecentPlayerGamesAsync(
                playerId,
                ScoreCalculator.RecentGameWindow,
                cancellationToken);

        var rules = league.Settings.Rules;
        var stats = requested
            .Select(kind => kind switch
            {
                StatSetKind.PreviousWeek => ScorePreviousWeek(game, rules),
                StatSetKind.Season => ScoreSeason(season, rules),
                StatSetKind.RecentThreeGames => ScoreRecentThreeGames(recentGames, rules),
                _ => throw new ArgumentOutOfRangeException(nameof(statSets), kind, "Unknown stat set.")
            })
            .OfType<StatSet>()
            .ToList();

        return Player.FromEntity(playerEntity) with { Stats = stats };
    }

    private async Task<Player> GetPlayerAlone(string playerId, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetPlayerByIdAsync(playerId, cancellationToken);
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

    private StatSet? ScoreRecentThreeGames(IReadOnlyList<PlayerGame> games, List<Rule> rules)
    {
        var scores = calculator.CalculateRecentThreeGames(games, rules);
        if (scores.Count == 0)
            return null;

        return StatSet.From(StatSetKind.RecentThreeGames, scores);
    }
}
