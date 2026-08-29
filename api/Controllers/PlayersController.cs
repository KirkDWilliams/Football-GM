using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    // Endpoints will be added when player features are implemented.
    private readonly PlayerSeasonRepository _seasonRepo;
    private readonly PlayerRepository _playerRepo;
    public PlayersController(
        PlayerSeasonRepository playerSeasonRepository,
        PlayerRepository playerRepository)
    {
        _seasonRepo = playerSeasonRepository;
        _playerRepo = playerRepository;
    }

    [Authorize]
    [HttpGet("player-season-stats")]
    [ProducesResponseType(typeof(PlayerStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerStatsResponse>> GetPlayerSeasonStats(string playerId, string gameId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(playerId))
            throw new ArgumentException("Player Id unknown!");

        var playerSeasonStats = await _seasonRepo.GetPlayerSeasonStatsAsync(playerId)
        ?? throw new ArgumentException("Season information unknown!");
        
        return Ok(new PlayerStatsResponse(playerSeasonStats));
    }

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
}
