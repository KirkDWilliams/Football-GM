using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlayerController(IPlayerOrchestrator orchestrator) : ControllerBase
{
    /// <summary>
    /// Get a player scored against a league's rules for a given game.
    /// </summary>
    [HttpGet("{playerId}")]
    [ProducesResponseType(typeof(Player), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Player>> GetPlayer(
        string playerId,
        [FromQuery] string gameId,
        [FromQuery] int leagueId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(playerId))
            return BadRequest(new { error = "PlayerId is required." });

        if (string.IsNullOrWhiteSpace(gameId))
            return BadRequest(new { error = "GameId is required." });

        if (leagueId <= 0)
            return BadRequest(new { error = "LeagueId is required." });

        try
        {
            var player = await orchestrator.GetPlayer(playerId, gameId, leagueId, cancellationToken);
            return Ok(player);
        }
        catch (ArgumentNullException)
        {
            return NotFound();
        }
    }
}
