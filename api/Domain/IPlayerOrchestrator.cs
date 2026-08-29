using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Domain;

public interface IPlayerOrchestrator
{
    Task<Player> GetPlayer(
        string playerId,
        IReadOnlyCollection<StatSetKind> statSets,
        int? leagueId = null,
        string? gameId = null,
        CancellationToken cancellationToken = default);
}
