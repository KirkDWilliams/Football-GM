using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure.Interfaces;
using FootballGm.Api.Services.GameAnalysis;

namespace FootballGm.Api.Domain;

public interface IPlayerOrchestrator
{
    Task<Player> GetPlayer(
        string playerId,
        string gameId,
        int leagueId,
        CancellationToken cancellationToken);
}

public class PlayerOrchestrator(
    IPlayerRepository playerRepository,
    ILeagueRepository leagueRepository,
    IScoreCalculator calculator) : IPlayerOrchestrator
{
    public async Task<Player> GetPlayer(
        string playerId,
        string gameId,
        int leagueId,
        CancellationToken cancellationToken)
    {
        var playerEntity = await playerRepository.GetPlayerByIdAsync(playerId);
        var gameEntity = await playerRepository.GetPlayerGameStatsAsync(playerId, gameId);
        var leagueEntity = await leagueRepository.GetByIdAsync(leagueId, cancellationToken);

        ArgumentNullException.ThrowIfNull(playerEntity);
        ArgumentNullException.ThrowIfNull(gameEntity);
        ArgumentNullException.ThrowIfNull(leagueEntity);

        var scores = calculator.Calculate(
            StatLine.From(gameEntity),
            leagueEntity.Settings.Rules);

        return Player.FromEntity(playerEntity) with
        {
            PreviousWeekScores = scores,
            PreviousWeekScore = scores.Sum(s => s.Value)
        };
    }
}
