using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Services.GameAnalysis;

public interface IGamePerformanceEvaluator
{
    Task<decimal> ComputePlayerPerformance(string playerId, string gameId);
}

public class GamePerformanceEvaluator(IPlayerRepository repository) : IGamePerformanceEvaluator
{
    public async Task<decimal> ComputePlayerPerformance(string playerId, string gameId)
    {
        var game = await repository.GetPlayerGameStatsAsync(playerId, gameId);

        if (game is null) return decimal.Zero;

        return decimal.Zero;
    }
}
