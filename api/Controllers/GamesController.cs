using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

/// <summary>
/// Game (matchup) resource. Named "Games" to align with the Flutter client domain model.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly PlayerRepository _playerRepo;
    public GamesController(PlayerRepository playerRepository)
    {
        _playerRepo = playerRepository;
    }

    // Endpoints will be added when game features are implemented.
    // GetGamesByWeek

    [Authorize]
    [HttpGet("player-game-stats")]
    [ProducesResponseType(typeof(PlayerStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerStatsResponse>> GetPlayerGameStats(string playerId, string gameId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(playerId))
            throw new ArgumentException("Player Id unknown!");

        var playerStats = await _playerRepo.GetPlayerGameStatsAsync(playerId, gameId)
            ?? throw new ArgumentException("Player stats not found!");

        return Ok(new PlayerStatsResponse(playerStats));
    }

    // GetGamesStats(string playerId)

    // GetGameStats(string teamId)
}
